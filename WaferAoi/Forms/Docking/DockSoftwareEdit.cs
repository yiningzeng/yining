using HalconDotNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using YiNing.WafermapDisplay.WafermapControl;
using static WaferAoi.Tools.Utils;

namespace WaferAoi
{
    public partial class DockSoftwareEdit : DarkDocument
    {
        #region 飞拍矫正
        HTuple DetectModelId;
        double halfRow, halfCol;
        #endregion


        bool canSkip = false; // 指示是否可以跳过流程测试的时候使用
        InterLayerDraw ilFocus, ilRight, ilTop, ilBottom, ilChipModel, ilStep5Model, ilStep3Main, ilStep6Test;
        HDevProgramHelper hDevProgramHelper;
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(8));
        private MVCameraHelper mVCameraHelper;
        Config config = FsmHelper.GetConfig();
        private Axis ax, ay, az, ar;// = FsmHelper.GetConfig().Axes.Find(v => v.Remarks == "载盘X轴");
        private ProgramConfig programConfig;

        List<PointInfo> pointInfos;

        public DockSoftwareEdit()
        {
            InitializeComponent();
            HotKey.RegisterHotKey(this.Handle, 100, HotKey.KeyModifiers.Ctrl, Keys.D1);
            this.darkStepViewer1.OnStepChanged += DarkStepViewer1_OnStepChanged;
            hswcFocus.MouseWheel += hswcFocus.HSmartWindowControl_MouseWheel;
            hswcRight.MouseWheel += hswcRight.HSmartWindowControl_MouseWheel;
            hswcTop.MouseWheel += hswcTop.HSmartWindowControl_MouseWheel;
            hswcBottom.MouseWheel += hswcBottom.HSmartWindowControl_MouseWheel;
            hswcChipModel.MouseWheel += hswcChipModel.HSmartWindowControl_MouseWheel;
            hswcStep5Model.MouseWheel += hswcStep5Model.HSmartWindowControl_MouseWheel;

            hswcChipModel.HMouseMove += HswcChipModel_HMouseMove;
            hswcStep5Model.HMouseMove += HswcStep5Model_HMouseMove;
            hswcStep6Test.MouseWheel += hswcStep6Test.HSmartWindowControl_MouseWheel;
            hswcStep6Test.HMouseMove += HswcStep6Test_HMouseMove;
            IniData();
        }

        public override void Close()
        {
            var result = DarkMessageBox.ShowWarning(@"确定要退出么?", @"提醒", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;
            SetCameraCallBack(null);
            base.Close();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键   
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            canSkip = !canSkip;
                            string msg = canSkip ? "已开启测试模式" : "已关闭测试模式";
                            DarkMessageBox.ShowInformation(msg);
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        void IniData()
        {
            ilFocus = new InterLayerDraw(hswcFocus);
            ilRight = new InterLayerDraw(hswcRight);
            ilTop = new InterLayerDraw(hswcTop);
            ilBottom = new InterLayerDraw(hswcBottom);
            ilChipModel = new InterLayerDraw(hswcChipModel);
            ilStep5Model = new InterLayerDraw(hswcStep5Model);
            ilStep3Main = new InterLayerDraw(hswcStep3Main);
            ilStep6Test = new InterLayerDraw(hswcStep6Test);
            hDevProgramHelper = new HDevProgramHelper("圆心查找ver3.1.hdev");
            UpdateGetAxes();
            #region 总流程步骤条
            List<DarkStepViewerItem> list = new List<DarkStepViewerItem>();
            list.Add(new DarkStepViewerItem("1", "放置晶圆", 1, "请放置晶圆", null));
            list.Add(new DarkStepViewerItem("2", "角度矫正", 2, "矫正晶圆的角度", null));
            list.Add(new DarkStepViewerItem("3", "中心矫正", 3, "计算晶圆的中心", null));
            list.Add(new DarkStepViewerItem("4", "单芯片扫描", 4, "制作芯片定位模板", null));
            list.Add(new DarkStepViewerItem("5", "图谱生成", 5, "生成晶圆相应的图谱", null));
            list.Add(new DarkStepViewerItem("6", "检测程式制作", 6, "制作芯片的模板信息来检测", null));
            //list.Add(new DarkStepViewerItem("7", "测试程式", 7, "测试制作的模板信息的检测结果", null));
            //list.Add(new DarkStepViewerItem("8", "保存程式", 8, "保存程式", null));
            darkStepViewer1.ListDataSource = list;
            darkStepViewer1.CurrentStep = 1;
            #endregion

            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
            dlvwProgress.SetStartNum(0);
            MotorsControl.MovePoint2D(80, ax.StartPoint, ay.StartPoint, ax, ay);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 0);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 0);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 0);
            dsepMove.Visible = false;
        }

        public DockSoftwareEdit(string text, Image icon, ref MVCameraHelper mc, ProgramConfig pc = null) : this()
        {
            DockText = text;
            Icon = icon;
            programConfig = pc;
            mVCameraHelper = mc;
            pointInfos = programConfig.WaferSize == WaferSize.INCH6 ? config.Inch6SavePoints : config.Inch8SavePoints;
            SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step2);
            cmbObjectiveLense.SelectedIndex = 0;
            mVCameraHelper.RemoveAllEvent("CameraImageCallBack");
        }


        public void SetConfig(ProgramConfig pc)
        {
            programConfig = pc;
        }

        private void SetCameraCallBack(EventHandler<ImageArgs> call)
        {
            mVCameraHelper.RemoveAllEvent("CameraImageCallBack");
            mVCameraHelper.CameraImageCallBack += call;
        }
        protected virtual void DarkStepViewer1_OnStepChanged(object sender, EventArgs e)
        {
            darkTabControl1.SelectedIndex = darkStepViewer1.CurrentStep - 1;
            MotorsControl.stopCompare();
            switch (darkStepViewer1.CurrentStep)
            {
                case 1:
                    #region 第一步流程引导
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
                    dlvwProgress.SetStartNum(0);
                    MotorsControl.MovePoint2D(80, ax.StartPoint, ay.StartPoint, ax, ay);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 0);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 0);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 0);
                    dsepMove.Visible = false;
                    Debug.WriteLine(dlvwProgress.NowItem().Text);
                    #endregion
                    break;
                case 2:
                    dsepMove.Visible = true;
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("自动对焦"));
                    dlvwProgress.Items.Add(new DarkListItem("手动移动芯片到图片中间区域"));
                    dlvwProgress.Items.Add(new DarkListItem("角度校正"));
                    dlvwProgress.SetStartNum(0);
                    Debug.WriteLine(dlvwProgress.NowItem().Text);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 1);
                    if (programConfig.HaveRingPiece == HaveRingPiece.Yes)
                    {
                        if (programConfig.WaferSize == WaferSize.INCH6) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 1);
                        if (programConfig.WaferSize == WaferSize.INCH8) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 1);
                    }
                    SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step2);
                    break;
                case 3:
                    mVCameraHelper.ThreadPoolEnable = false;
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("开始"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找右边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找上边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找下边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("开始计算"));
                    dlvwProgress.SetStartNum(0);
                    dlvwProgress.Stop();
                    SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step3);
                    break;
                case 4:
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("手动移动芯片到图片中间区域"));
                    dlvwProgress.Items.Add(new DarkListItem("选择芯片的左上角"));
                    dlvwProgress.Items.Add(new DarkListItem("选择芯片的右下角"));
                    //dlvwProgress.Items.Add(new DarkListItem("扫描单个芯片"));
                    dlvwProgress.Items.Add(new DarkListItem("制作芯片模板"));
                    dlvwProgress.SetStartNum(0);
                    //dlvwProgress.Stop();
                    SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step4);
                    _ = Step4Ini();
                    break;
                case 5:
                    SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step5);
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("测量切割道的宽度"));
                    dlvwProgress.Items.Add(new DarkListItem("确定切割道位置"));
                    dlvwProgress.Items.Add(new DarkListItem("制作晶圆扫描模板"));
                    dlvwProgress.Items.Add(new DarkListItem("开始扫描整片晶圆"));
                    dlvwProgress.Items.Add(new DarkListItem("搜寻边沿芯片"));
                    dlvwProgress.Items.Add(new DarkListItem("正在扫描晶圆"));
                    dlvwProgress.Items.Add(new DarkListItem("生成图谱"));
                    //dlvwProgress.Items.Add(new DarkListItem("校对图谱"));
                    //dlvwProgress.Items.Add(new DarkListItem("保存"));
                    dlvwProgress.SetStartNum(0);
                    darkTextBox6.Text = programConfig.DieWidth.ToString();
                    darkTextBox7.Text = programConfig.DieHeight.ToString();
                    _ = Step5Ini();
#if DEBUG
                    //Die[,] data = new Die[41, 40];

                    //for (int x = 0; x < 41; x++)
                    //{
                    //    for (int y = 0; y < 40; y++)
                    //    {
                    //        //if (y > 18 && y < 21 && x >= ss && x < 41 - ss)
                    //        //{
                    //        //    data[x, y] = new Die() { ColorIndex = 1, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    //        //}
                    //        //else
                    //        data[x, y] = new Die() { ColorIndex = 0, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    //    }
                    //}

                    //JsonHelper.Serialize(data, programConfig.GetMappingFileName());
#endif
                    // Create sample dataset


                    //step5Wafermap.SelectRegionDiagonalDie = new Die[] { new Die() { XIndex = 10, YIndex = 10 }, new Die() { XIndex = 21, YIndex = 23 } };
                    if (File.Exists(programConfig.GetMappingFileName()))
                    {
                        step5Wafermap.Dataset = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
                        step5Wafermap.Colors = new Color[] { Color.Blue, Color.Chocolate };
                        step5Wafermap.Notchlocation = 90;
                    }
                    dlvwProgress.Next();
                    break;
                case 6:
                    btnNextStep.Text = "下一步";
                    dspProgress.Visible = true;
                    dsepMove.Visible = true;
                    if (File.Exists(programConfig.GetMappingFileName()))
                    {
                        step6Wafermap.Dataset = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
                        step6Wafermap.Colors = new Color[] { Color.Blue, Color.Chocolate };
                        step6Wafermap.Notchlocation = 90;
                    }
                    SetCameraCallBack(MVCameraHelper_CameraImageCallBack_Step6);
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("随机选择待训练的芯片"));
                    dlvwProgress.Items.Add(new DarkListItem("开始训练"));
                    dlvwProgress.Items.Add(new DarkListItem("移动视图"));
                    dlvwProgress.Items.Add(new DarkListItem("测试"));
                    dlvwProgress.Items.Add(new DarkListItem("保存程式"));
                    dlvwProgress.SetStartNum(0);
                    break;
                case 7:
                    dspProgress.Visible = false;
                    dsepMove.Visible = false;
                    btnNextStep.Text = "退出";
                    break;
            }
        }


        #region 运动控制

        private void UpdateGetAxes()
        {
            config = FsmHelper.GetConfig();
            ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
            ar = config.Axes.Find(v => v.Remarks == "载盘旋转轴");
        }
        /// <summary>
        /// 单个芯片移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveByDie_Click(object sender, EventArgs e)
        {
            if (programConfig.DieWidth ==0 && programConfig.DieHeight ==0 && programConfig.CutRoadWidth == 0)
            {
                DarkMessageBox.ShowError("请先完成相关的操作");
                return;
            }
            mVCameraHelper.ThreadPoolEnable = false;
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            //MotorsControl.startCompare();                     //MotorsControl.startCompare();
            MotorsControl.stopCompare();
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            int runDirection = -1; // 表示相机方向

            int axisNowPos = -1;
            double runInterval = 0;
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1 * runDirection; runInterval = programConfig.CutRoadWidth  + programConfig.DieHeight; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1 * runDirection; runInterval = programConfig.CutRoadWidth  + programConfig.DieHeight; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1 * runDirection; runInterval = programConfig.CutRoadWidth  + programConfig.DieWidth; break;//"晶圆载具Y轴状态";
                case "向右": axisId = 2; direction = -1 * runDirection; runInterval = programConfig.CutRoadWidth + programConfig.DieWidth; break;//晶圆载具Y轴状态";
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
            {
                axisNowPos = MotorsControl.GetOneEncPos(axisId);
                MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), 10, axisNowPos + Convert.ToInt32(runInterval * direction), true);
            }
            MotorsControl.stopCompare();
            if (darkStepViewer1.CurrentStep == 6) { dlvwProgress.SetStartNum(3); }
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
        }
        /// <summary>
        /// 运动控制停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            MotorsControl.stopCompare();
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1; break;//晶圆载具Y轴状态
                case "向右": axisId = 2; direction = -1; break;//晶圆载具Y轴状态
                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
                MotorsControl.StopAxis(axis.Id, 0);

            //最后再拍一张防止后面又有一点小走动
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
        }
        /// <summary>
        /// 运动控制，开始运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if(darkStepViewer1.CurrentStep == 6) { dlvwProgress.SetStartNum(2); }
            mVCameraHelper.ThreadPoolEnable = false;
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            MotorsControl.startCompare();
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            int runDirection = -1; // 表示相机方向
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1 * runDirection; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1 * runDirection; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1 * runDirection; break;//"晶圆载具Y轴状态";
                case "向右": axisId = 2; direction = -1 * runDirection; break;//晶圆载具Y轴状态";

                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
                MotorsControl.MoveJog(axis.Id, axis.JogPrm.Get(), Convert.ToDouble(dbupVel.Value), direction);
        }
        #endregion

        #region 第一步 放置晶圆
        private void btnMoveToPlace_Click(object sender, EventArgs e)
        {
          
        }

        private void tsInich_CheckedChanged(object sender, EventArgs e)
        {
            var Ts = sender as JCS.ToggleSwitch;
            switch (Ts.OnText)
            {
                case "裸片阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, Ts.Checked ? 1 : 0);
                    break;
                case "6寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, Ts.Checked ? 1 : 0);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, Ts.Checked ? 1 : 0);
                    break;
                case "8寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, Ts.Checked ? 1 : 0);
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, Ts.Checked ? 1 : 0);
                    break;
            }

            if (Ts.Checked)
            {
                dlvwProgress.SetStartNum(1);
                dlvwProgress.Next();
            }
            else
            {
                dlvwProgress.SetStartNum(0);
            }
        }
        #endregion

        #region 第二步 方向角整

        #region 对焦参数
        //自动对焦图像回调个数
        private int focusImgCallNum = -1; //从-1开始那是因为focusPos下标识0开始
        private List<int> focusPos = new List<int>();
        private int bestZPulse = 0; // 最好的z的高度
        private double bestZScore = 0; //最好的Z高度分值
        private bool isCoarse = true; // 粗略扫描
        #endregion
        private PointInfo startRotatePoint;
        private ConcurrentQueue<double> RotateRes = new ConcurrentQueue<double>();
        private void darkStep2Button_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "第二步按流程自动执行":
                    _ = AutoRun2Async();
                    break;
                case "自动对焦":
                    mVCameraHelper.ThreadPoolEnable = true;
                    btn.Enabled = false;
                    dlvwProgress.SetStartNum(0);
                    Debug.WriteLine(dlvwProgress.NowItem().Text);
                    _ = StartFocusingAsync();
                    break;
                case "角度矫正":
                    programConfig.BestZPulse = bestZPulse;
                    JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
                    if (!RotateRes.IsEmpty) { while (RotateRes.TryDequeue(out double item)) { } }
                    mVCameraHelper.ThreadPoolEnable = false;
                    dlvwProgress.SetStartNum(2);
                    _ = Rotate();
                    break;
                case "相机曝光":
                    mVCameraHelper.ShowSettingPage();
                    break;
            }
        }
        private void cmbObjectiveLense_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FsmHelper.ChangeLense(cmbObjectiveLense.SelectedIndex);
                programConfig.ObjectiveLense = (ObjectiveLense)cmbObjectiveLense.SelectedIndex;
                JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
            }
            catch (Exception er) { }
        }
        /// <summary>
        /// 相机回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MVCameraHelper_CameraImageCallBack_Step2(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "自动对焦":
                        if (!isCoarse) focusImgCallNum++; // 不是精细扫描不计数

                        object[] objectArray = new object[3];//这里的2就是改成你要传递几个参数
                        objectArray[0] = e;
                        objectArray[1] = focusImgCallNum;
                        objectArray[2] = isCoarse; 
                        object param = (object)objectArray;

                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            object[] objArr = (object[])obs;
                            ImageArgs imgArg = (ImageArgs)objArr[0];
                            int id = (int)objArr[1];
                            bool isCo = (bool)objArr[2];
                            // 将object转成数组
                            double tempScore = Utils.CalIntensity(imgArg.ImageHobject, 0.5);
                            if (bestZScore < tempScore)
                            {
                                bestZScore = tempScore;
                                bestZPulse = isCo ? imgArg.ZPulse : focusPos[id];
                                Debug.WriteLine("最优高度: " + bestZPulse + " 最高分数: " + bestZScore);
                                this.BeginInvoke(new Action<int>(v =>
                                {
                                    //tbBsetZPulse.Text = v.ToString();
                                    ilFocus.ShowImg(imgArg);
                                }), bestZPulse);
                            }
                            else
                            {
                                e.Dispose();
                            }

                            return false;
                        }, param));
                        break;
                    case "手动移动芯片到图片中间区域":
                        ilFocus.ShowImg(e);
                        break;
                    case "角度校正":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            HOperatorSet.GenEmptyObj(out HObject imageRotate);
                            hDevProgramHelper.ChipDeg(imgArg.ImageHobject, out imageRotate, out HTuple OrientationAngle, out HTuple Degree);
                            imageRotate.Dispose();
                            RotateRes.Enqueue(Degree.D);
                            double tempScore = Utils.CalIntensity(imgArg.ImageHobject, 0.5);
                            this.BeginInvoke(new Action<ImageArgs>(v =>
                            {
                                //HOperatorSet.GetImageSize(e.ImageHobject, out HTuple Iwidth, out HTuple Iheight);
                                HOperatorSet.SetPart(hswcFocus.HalconWindow, 0, 0, v.Height - 1, v.Width - 1);
                                HOperatorSet.ClearWindow(hswcFocus.HalconWindow);
                                HOperatorSet.DispObj(v.ImageHobject, hswcFocus.HalconWindow);
                                v.Dispose();
                            }), imgArg);
                            return false;
                        }, e));
                        //ilFocus.ShowImg(e);
                        break;
                    default:
                        ilFocus.ShowImg(e);
                        ilFocus.DrawCross(e.Height /2, e.Width / 2, e.Width);
                        break;
                }
            }
            catch (Exception er)
            {

            }
        }

        private async Task AutoRun2Async()
        {
            this.BeginInvoke(new Action(() => { dlvwProgress.SetStartNum(0); btnFocus.Enabled = false;btnRotate.Enabled = false; }));
            await StartFocusingAsync();
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() => dlvwProgress.Next()));
            await Rotate();
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() => dlvwProgress.Next()));
            this.BeginInvoke(new Action(() => { btnFocus.Enabled = true; btnRotate.Enabled = true; }));
        }

        /// <summary>
        /// 角度矫正
        /// </summary>
        /// <returns></returns>
        private async Task Rotate()
        {
            await Task.Run(() => {

                HOperatorSet.GenEmptyObj(out HObject imageRotate);
                hDevProgramHelper.ChipDeg(ilFocus.hImage, out imageRotate, out HTuple OrientationAngle, out HTuple Degree);
                //string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //ilFocus.SaveImg("bmp", @"D:\矫正图像\" + fileName + ".bmp");
                //if (imageRotate.IsInitialized()) HOperatorSet.WriteImage(imageRotate, "bmp", 0, @"D:\矫正图像\" + fileName + "-Rotate.bmp");
                imageRotate.Dispose();
                //MotorsControl.startCompare();
                MotorsControl.setCompareMode(ar.Id, ar.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(20); // 等差模式 
                MotorsControl.startCompare();
                int posNow = MotorsControl.GetREncPos(ar.Id);
                MotorsControl.MoveTrap(ar.Id, ar.TrapPrm.Get(), 5, posNow + Convert.ToInt32(Degree.D * 1000), true);
                MotorsControl.stopCompare();

                if (startRotatePoint == null) startRotatePoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                int offset = programConfig.WaferSize == WaferSize.INCH8 ? 60000 : 30000;

                #region X方向上校验
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1, 100);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(300); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, startRotatePoint.X + offset, true);
                MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, startRotatePoint.X - offset, true);
                MotorsControl.stopCompare();
                #endregion
                MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 80, startRotatePoint.X, true);
                #region Y方向上校验
                MotorsControl.setCompareMode(ay.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1, 100);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(300); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 50, startRotatePoint.Y + offset, true);
                MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 50, startRotatePoint.Y - offset, true);
                MotorsControl.stopCompare();
                #endregion
                MotorsControl.MovePoint2D(80, startRotatePoint.X, startRotatePoint.Y, ax, ay, true);
            });
            Task.WaitAll(tasklst.ToArray());
            List<double> rotateVal = new List<double>();
            while (RotateRes.TryDequeue(out double val))
            {
                rotateVal.Add(val);
            }
            double sum = 0;
            rotateVal.Sort((x, y) => y.CompareTo(x));
            for(int i = 3; i< rotateVal.Count - 3; i++)
            {
                sum += rotateVal[i];
            }
            double finalDegree = sum / (rotateVal.Count - 6);

            MotorsControl.setCompareMode(ar.Id, ar.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            MotorsControl.startCompare();
            int posNow2 = MotorsControl.GetREncPos(ar.Id);
            MotorsControl.MoveTrap(ar.Id, ar.TrapPrm.Get(), 5, posNow2 + Convert.ToInt32(finalDegree * 1000 - 50), true); // - 553.64555555 减553.64555555这是固定的变量
            MotorsControl.stopCompare();

            this.BeginInvoke(new Action(() => dlvwProgress.Next()));
        }
        /// <summary>
        /// 对焦FUNC
        /// </summary>
        /// <param name="e"></param>
        private async Task StartFocusingAsync()
        {
            isCoarse = true;
            focusImgCallNum = -1;
            focusPos.Clear();
            bestZScore = 0;
            bestZPulse = 0;
            //int zHeight = MotorsControl.GetZEncPos(az.Id);

            await Task.Run(() => {
                MotorsControl.stopCompare();
                PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
                MotorsControl.MovePoint2D(100, pointInfo.X, pointInfo.Y, ax, ay, true);
#if DEBUG
                MotorsControl.GoHome(az.Id, az.GoHomePar.Get(), out GSN.THomeStatus homeStatus);
#endif
                int interval = 300;
                MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(interval); // 等差模式 
                isCoarse = false;
                int maxH = 4000;
                int mixH = -10000;
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, maxH, true);

                focusPos.AddRange(MotorsControl.GetFlyPos(maxH, mixH, interval, -1));
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, mixH, true);
                MotorsControl.stopCompare();

                //int range = 3000;
                //int upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                //int downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;

                interval = 100;
                MotorsControl.setCompareData_Pso(interval); // 等差模式 
                int range = 1000;
                int upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                int downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;

                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, upH, true);
                focusPos.AddRange(MotorsControl.GetFlyPos(upH, downH, interval, -1));
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 3, downH, true);
                MotorsControl.stopCompare();


                interval = 10;
                MotorsControl.setCompareData_Pso(interval); // 等差模式
                range = 900;
                upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, upH, true);
                focusPos.AddRange(MotorsControl.GetFlyPos(upH, downH, interval, -1));
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 1, downH, true);
                MotorsControl.stopCompare();
                //MotorsControl.setCompareData_Pso(30); // 等差模式
                //range = 150;
                //upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                //downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
                //MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 0.5, upH, true);
                //MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 0.5, downH, true);
            });
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
                btnFocus.Enabled = true;
            }));
        }
        #endregion

        #region 第三步 晶圆定位
        private bool alalreadyEdge = false;
        //private int step3ImgCallNum = -1; //从-1开始那是因为focusPos下标识0开始
        //private List<PointInfo> step3Pos = new List<PointInfo>();
        private async Task TestMove(double xPos, double yPos)
        {
            darkSectionPanel13.Visible = true;
            await Task.Run(() => {
                MotorsControl.MovePoint2D(100,Convert.ToInt32(xPos), Convert.ToInt32(yPos), ax, ay, true);

                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(800); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.stopCompare();
            });
        }
        private void MVCameraHelper_CameraImageCallBack_Step3(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    //default:
                    //    ilTop.ShowImg(e);
                    //    break;
                    case "寻找右边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            if (hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90))
                            {
                                ilRight.hImage = imgArg.ImageHobject;
                                ilRight.XPulse = imgArg.XPulse;
                                ilRight.YPulse = imgArg.YPulse;
                                ilRight.AllCols = imgArg.Width;
                                ilRight.AllRows = imgArg.Height;
                                //alalreadyEdge = true;
                                //MotorsControl.stopCompare();
                                //MotorsControl.StopAxis(ax.Id, 1);
                                //MotorsControl.StopAxis(ay.Id, 1);
                                //this.BeginInvoke(new Action(() =>
                                //{
                                //    dlvwProgress.Stop();
                                //}));
                                //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                                PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                                if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\右.bmp");
                                JsonHelper.Serialize(pf, "D:/右.json");
                                Debug.WriteLine("找到右边沿，占比: " + ratio);
                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilRight.ShowImg(e);
                                //}), pf);
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return false;
                        }, e));
                        break;
                    case "寻找上边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            //if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\上边沿\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                            if (hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90))
                            {
                                ilTop.hImage = imgArg.ImageHobject;
                                ilTop.XPulse = imgArg.XPulse;
                                ilTop.YPulse = imgArg.YPulse;
                                ilTop.AllCols = imgArg.Width;
                                ilTop.AllRows = imgArg.Height;
                                //alalreadyEdge = true;
                                //MotorsControl.stopCompare();
                                //MotorsControl.StopAxis(ax.Id, 1);
                                //MotorsControl.StopAxis(ay.Id, 1);
                                //this.BeginInvoke(new Action(() =>
                                //{
                                //    dlvwProgress.Stop();
                                //}));
                                //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                                PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                                if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\上.bmp");
                                JsonHelper.Serialize(pf, "D:/上.json");
                                Debug.WriteLine("找到上边沿，占比: " + ratio);
                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilTop.ShowImg(e);
                                //}), pf); 
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return false;
                        }, e));
                        break;
                    case "寻找下边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            //if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\下边沿\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                            if (hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90))
                            {
                                ilBottom.hImage = imgArg.ImageHobject;
                                ilBottom.XPulse = imgArg.XPulse;
                                ilBottom.YPulse = imgArg.YPulse;
                                ilBottom.AllCols = imgArg.Width;
                                ilBottom.AllRows = imgArg.Height;
                                //alalreadyEdge = true;
                                //MotorsControl.stopCompare();
                                //MotorsControl.StopAxis(ax.Id, 1);
                                //MotorsControl.StopAxis(ay.Id, 1);
                                //this.BeginInvoke(new Action(() =>
                                //{
                                //    dlvwProgress.Stop();
                                //}));
                                //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                                PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                                if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\下.bmp");
                                JsonHelper.Serialize(pf, "D:/下.json");
                                Debug.WriteLine("找到下边沿，占比: " + ratio);
                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilBottom.ShowImg(e);
                                //}), pf);
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return false;
                        }, e));
                        break;
                    default:
                        HOperatorSet.WriteImage(e.ImageHobject, "bmp", 0, "D:/final.bmp");
                        ilStep3Main.ShowImg(e);
                        break;
                }

            }
            catch (Exception er)
            {

            }
        }
        /// <summary>
        /// 步骤三自动执行
        /// </summary>
        /// <returns></returns>
        private async Task AutoRun3Async()
        {
            alalreadyEdge = false;
            //step3ImgCallNum = -1;
            //step3Pos.Clear();
            this.BeginInvoke(new Action(() =>
            {
                darkButton3.Enabled = false;
                darkButton4.Enabled = false;
                darkButton5.Enabled = false;
                dlvwProgress.SetStartNum(0);
            }));
            this.BeginInvoke(new Action(() => { dlvwProgress.Next(); }));

            //await SearchAsync(pointInfos.Find(i => i.Remark == "右边沿搜索中心点"), int.Parse(tbRangeRight.Text));
            //Task.WaitAll(tasklst.ToArray());
            //                case "移动到右中心":
            //        _ = TestMove(double.Parse(tb1x.Text), double.Parse(tb1y.Text));
            //break;
            //    case "移动到上中心":
            //        _ = TestMove(double.Parse(tb2x.Text), double.Parse(tb2y.Text));
            //break;
            //    case "移动到下中心":
            //        _ = TestMove(double.Parse(tb3x.Text), double.Parse(tb3y.Text));
            //break;
            PointInfo p= pointInfos.Find(i => i.Remark == "右边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                 return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            if (!FindEdgeAndShow("edgepoint_right", ilRight, hswcRight)) { DarkMessageBox.ShowError("未找到边沿"); return; };

            alalreadyEdge = false;
            //await SearchAsync(pointInfos.Find(i => i.Remark == "上边沿搜索中心点"), int.Parse(tbRangeTop.Text));
            p = pointInfos.Find(i => i.Remark == "上边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            if (!FindEdgeAndShow("edgepoint", ilTop, hswcTop)) { DarkMessageBox.ShowError("未找到边沿"); return; };

            alalreadyEdge = false;
            //await SearchAsync(pointInfos.Find(i => i.Remark == "下边沿搜索中心点"), int.Parse(tbRangeBottom.Text));

            p = pointInfos.Find(i => i.Remark == "下边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            if (!FindEdgeAndShow("edgepoint_down", ilBottom, hswcBottom)) { DarkMessageBox.ShowError("未找到边沿"); return; };

            #region 计算圆心
            hDevProgramHelper.FindCenter(ilTop.Point_X, ilTop.Point_Y, ilBottom.Point_X, ilBottom.Point_Y, ilRight.Point_X, ilRight.Point_Y, out HTuple centerX, out HTuple centerY, out HTuple radius);
            programConfig.WaferCenter = new Point(Convert.ToInt32(centerX.D), Convert.ToInt32(centerY.D));
            programConfig.WaferRadius = radius.D;
            #endregion

            #region 移动到圆心
            MotorsControl.MovePoint2D(100, programConfig.WaferCenter.X, programConfig.WaferCenter.Y, ax, ay, true);
            #endregion

            this.BeginInvoke(new Action(() =>
            {
                darkButton3.Enabled = true;
                darkButton4.Enabled = true;
                darkButton5.Enabled = true;
                dlvwProgress.Next();
                tbWaferCenterX.Text = programConfig.WaferCenter.X.ToString();
                tbWaferCenterY.Text = programConfig.WaferCenter.Y.ToString();
                tbWaferRadius.Text = programConfig.WaferRadius.ToString();
            }));
        }

        /// <summary>
        /// 检测并显示边沿结果
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="hswc"></param>
        /// <returns></returns>
        private bool FindEdgeAndShow(string funcName, InterLayerDraw il, HSmartWindowControl hswc)
        {
            HOperatorSet.GenEmptyObj(out HObject obj);
            try
            {
                //HOperatorSet.WriteImage(il.hImage, "bmp", 0, "D:/aaaaaaaaaaaaaaaaaaaa.bmp");
                hDevProgramHelper.FindEdge(funcName, il.hImage, out obj, out HTuple pointX, out HTuple pointY, il.XPulse, il.YPulse, il.AllCols * config.ActualPixelLenght / programConfig.GetLenseMag(), il.AllRows * config.ActualPixelLenght / programConfig.GetLenseMag(), programConfig.GetLenseMag(), config.ActualPixelLenght, 0.4);
                HOperatorSet.DispObj(obj, hswc.HalconWindow);
                if (obj.IsInitialized())
                {
               
                    il.Point_X = pointX;
                    il.Point_Y = pointY;
                    JsonHelper.Serialize(new PointInfo() { X = Convert.ToInt32(il.Point_X.D), Y = Convert.ToInt32(il.Point_Y.D) }, "D:/"+funcName+".json");
                    this.BeginInvoke(new Action<HObject, HObject>((a, b) => {
                   
                        dlvwProgress.Next();
                        il.ShowImg(a, b, false);
                        b.Dispose();
                    }), il.hImage, obj);
                    return true;
                }
                else
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        darkButton3.Enabled = true;
                        darkButton4.Enabled = true;
                        darkButton5.Enabled = true;
                        dlvwProgress.Error();
                    }));
                    return false;
                }
            }
            catch (Exception er) { return false; }
            finally
            {
         
            }


        }
        /// <summary>
        /// 搜寻边沿
        /// </summary>
        /// <param name="pointInfo">搜寻范围中心信息</param>
        /// <param name="range">搜寻的正方形范围边长</param>
        /// <param name="flyInterval">相机飞拍的间隔</param>
        private async Task SearchAsync(PointInfo pointInfo, int range, int flyInterval = 1000)
        {
            MotorsControl.stopCompare();
            await Task.Run(() => {
                flyInterval = 200;
                int moveinterval = 3000; //这是移动的间隔，用来算行数
                int flyLines = range / moveinterval;
                int startY = pointInfo.Y - range / 2, startX = pointInfo.X - range / 2;
                int endY = pointInfo.Y + range / 2, endX = pointInfo.X + range / 2;

                #region 这里这两行一定不能删!!!!!,不能和全局的ax ay混合！！！！
                Axis axTemp = config.Axes.Find(v => v.Id == 2);
                Axis ayTemp = config.Axes.Find(v => v.Id == 1);
                #endregion
                MotorsControl.MovePoint2D(150, startX, startY, axTemp, ayTemp, true);
                //MotorsControl.startCompare();
                //MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 10, endY);
                //MotorsControl.stopCompare();
                if (pointInfo.Remark.Contains("右边沿"))
                {
                    axTemp = config.Axes.Find(v => v.Id == 1);
                    ayTemp = config.Axes.Find(v => v.Id == 2);
                }
                MotorsControl.setCompareMode(ayTemp.Id, ayTemp.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                #region 这里是个小范围的飞拍
                for (int i = 0; i < flyLines; i++)
                {
                    int location1 = 1;
                    if (i % 2 == 0)
                    {
                        location1 = pointInfo.Remark.Contains("右边沿") ? endX : endY;
                    }
                    else
                    {
                        location1 = pointInfo.Remark.Contains("右边沿") ? startX : startY;
                    }
                    //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                 
                
                    MotorsControl.startCompare();
                    MotorsControl.MoveTrap(ayTemp.Id, ayTemp.TrapPrm.Get(), 20, location1, true);
                    MotorsControl.stopCompare();
                    if (alalreadyEdge) break;
                    if (i < flyLines - 1)
                    {
                        if (pointInfo.Remark.Contains("右边沿"))
                        {
                            startY = startY + moveinterval;
                            MotorsControl.MoveTrap(axTemp.Id, axTemp.TrapPrm.Get(), 30, startY, true);
                        }
                        else
                        {
                            startX = startX + moveinterval;
                            MotorsControl.MoveTrap(axTemp.Id, axTemp.TrapPrm.Get(), 30, startX, true);
                        }
                    }
                }
            });
            #endregion
        }

        private void darkStep3Button_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "第三步按流程自动执行":
                    //mVCameraHelper.ThreadPoolEnable = true;
                    _ = AutoRun3Async();
                    break;
                case "寻找右边沿":
                    dlvwProgress.SetStartNum(1);
                    _ = SearchAsync(pointInfos.Find(i => i.Remark == "右边沿搜索中心点"), int.Parse(tbRangeRight.Text));
                    break;
                case "寻找上边沿":
                    dlvwProgress.SetStartNum(2);
                    _ = SearchAsync(pointInfos.Find(i => i.Remark == "上边沿搜索中心点"), int.Parse(tbRangeTop.Text));
                    break;
                case "寻找下边沿":
                    dlvwProgress.SetStartNum(3);
                    _ = SearchAsync(pointInfos.Find(i => i.Remark == "下边沿搜索中心点"), int.Parse(tbRangeBottom.Text));
                    break;
                case "移动到左边沿":
                    _ = TestMove(double.Parse(tbWaferCenterX.Text) - double.Parse(tbWaferRadius.Text), double.Parse(tbWaferCenterY.Text));
                    break;
                case "移动到右边沿":
                    _ = TestMove(double.Parse(tbWaferCenterX.Text) + double.Parse(tbWaferRadius.Text), double.Parse(tbWaferCenterY.Text));
                    break;
                case "移动到上边沿":
                    _ = TestMove(double.Parse(tbWaferCenterX.Text), double.Parse(tbWaferCenterY.Text) + double.Parse(tbWaferRadius.Text));
                    break;
                case "移动到下边沿":
                    _ = TestMove(double.Parse(tbWaferCenterX.Text), double.Parse(tbWaferCenterY.Text) - double.Parse(tbWaferRadius.Text));
                    break;
                case "移动到右中心":
                    _ = TestMove(double.Parse(tb1x.Text), double.Parse(tb1y.Text));
                    break;
                case "移动到上中心":
                    _ = TestMove(double.Parse(tb2x.Text), double.Parse(tb2y.Text));
                    break;
                case "移动到下中心":
                    _ = TestMove(double.Parse(tb3x.Text), double.Parse(tb3y.Text));
                    break;
            }
        }

        #endregion

        #region 第四步 扫描芯片
        private bool scanIsOneFoc = true;

        private PointInfo step4TopLeftPoint, step4BottomRightPoint;
        private Point step4TopLeftPointPixel, step4BottomRightPointPixel;
        private async Task Step4Ini()
        {
            await Task.Run(() =>
            {
                MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(100); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.MovePoint2D(100, programConfig.WaferCenter.X, programConfig.WaferCenter.Y, ax, ay, true);
                MotorsControl.stopCompare();
                MotorsControl.startCompare();
                MotorsControl.stopCompare();
            });
        }
        private void CreateChipModel()
        {
            //Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            //Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            //PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
            //MotorsControl.MovePoint2D(100, pointInfo.X, pointInfo.Y, ax, ay, true);
        }

        private void 画一个矩形框ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilChipModel.DrawRectange1();
        }
        private void 保存芯片区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilChipModel.GetROIImage(ilChipModel.selected_drawing_object, out HObject cropImg);
            //DialogCreateModel dialogCreateModel = new DialogCreateModel(cropImg, programConfig.GetChipModelRegionFileName(), programConfig.GetChipModelFileName(), "芯片模型制作");
            //dialogCreateModel.ShowDialog();
            //if (dialogCreateModel.DialogResult == DialogResult.OK)
            //{

            //    dlvwProgress.Next();
            //}
        }


        private void HswcChipModel_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                ilChipModel.X = Convert.ToInt32(e.X);
                ilChipModel.Y = Convert.ToInt32(e.Y);
            }
            catch (Exception er) { }
        }

        private void hswcChipModel_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "选择芯片的左上角":
                        PointInfo tempPoint = ilChipModel.DrawCrossRet();
                        #region 计算距离中心点的偏移
                        int offsetx = tempPoint.Col() - ilChipModel.AllCols / 2;
                        int offsety = tempPoint.Row() - ilChipModel.AllRows / 2;

                        double offsetXPulse = ProgramConfig.GetXPulseByPixel(tempPoint.Col() - ilChipModel.AllCols / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
                        double offsetYPulse = ProgramConfig.GetYPulseByPixel(tempPoint.Row() - ilChipModel.AllRows / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // 由于halcon的图像坐标和运动周相反，所以y要乘以-1
                        #endregion
                        step4TopLeftPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                        step4TopLeftPoint.X += Convert.ToInt32(offsetXPulse);
                        step4TopLeftPoint.Y += Convert.ToInt32(offsetYPulse);
                        tbStep4TopLeftX.Text = step4TopLeftPoint.X.ToString();
                        tbStep4TopLeftY.Text = step4TopLeftPoint.Y.ToString();
                        step4TopLeftPointPixel = new Point(tempPoint.X, tempPoint.Y);
                        //dlvwProgress.Next();
                        break;
                    case "选择芯片的右下角":
                        tempPoint = ilChipModel.DrawCrossRet();
                        #region 计算距离中心点的偏移
                        offsetXPulse = ProgramConfig.GetXPulseByPixel(tempPoint.Col() - ilChipModel.AllCols / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
                        offsetYPulse = ProgramConfig.GetYPulseByPixel(tempPoint.Row() - ilChipModel.AllRows / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // 由于halcon的图像坐标和运动周相反，所以y要乘以-1
                        #endregion
                        step4BottomRightPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                        step4BottomRightPoint.X += Convert.ToInt32(offsetXPulse);
                        step4BottomRightPoint.Y += Convert.ToInt32(offsetYPulse);
                        tbStep4BottomRightX.Text = step4BottomRightPoint.X.ToString();
                        tbStep4BottomRightY.Text = step4BottomRightPoint.Y.ToString();
                        step4BottomRightPointPixel = new Point(tempPoint.X, tempPoint.Y);
                        //dlvwProgress.Next();
                        break;
                }
            }
            catch (Exception er) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointInfo"></param>
        /// <param name="range"></param>
        /// <param name="flyInterval"></param>
        /// <returns></returns>
        private async Task Step4SearchAsync(PointInfo startPoint, PointInfo endPoint, int flyInterval = 5000)
        {
            MotorsControl.stopCompare();
            await Task.Run(() => {
                int width = endPoint.X - startPoint.X;
                int height = startPoint.Y - endPoint.Y;
                programConfig.DieWidth = width;
                programConfig.DieHeight = height;
                int flyLines = height / flyInterval; // 飞拍的行数
                int startY = startPoint.Y;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                Axis ay = config.Axes.Find(v => v.Id == 1);
                MotorsControl.MovePoint2D(50, startPoint.X, startPoint.Y, ax, ay, true);
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                if (flyLines == 0 && width / flyInterval == 0) //如果X和Y除以间距都是0的话那就在中心拍一张
                {
                    scanIsOneFoc = true;
                    MotorsControl.MovePoint2D(50, startPoint.X + width /2, startPoint.Y - height /2, ax, ay, true);
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                }
                else // 这里飞拍还要判断下XY方向
                {
                    scanIsOneFoc = false;
                    #region 这里是个小范围的飞拍
                    for (int i = 0; i < flyLines; i++)
                    {
                        int location1 = 1;
                        if (i % 2 == 0)
                        {
                            location1 = endPoint.X;
                        }
                        else
                        {
                            location1 = startPoint.X;
                        }
                        //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                        MotorsControl.startCompare();
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, location1, true);
                        MotorsControl.stopCompare();
                        if (i < flyLines - 1)
                        {
                            startY = startY + flyInterval;
                            MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 30, startY, true);
                        }
                    }
                    #endregion
                }
            });

        }

        /// <summary>
        /// 扫描单个芯片
        /// </summary>
        private async Task ScanOneChipAsync()
        {
            //需要先判断fov视野是否能够拍全， 默认1倍镜是5毫米吧， 5000微米
            await Step4SearchAsync(step4TopLeftPoint, step4BottomRightPoint, 5000);
            //this.BeginInvoke(new Action(() =>
            //{
            //    dlvwProgress.Next();
            //}));
        }

        private void MVCameraHelper_CameraImageCallBack_Step4(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "选择芯片的左上角":
                    case "选择芯片的右下角":
                        ilChipModel.ShowImg(e);
                        HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, 500, 0);
                        HOperatorSet.SetColor(hswcChipModel.HalconWindow, "red");
                        HOperatorSet.DispObj(hObjectCross, hswcChipModel.HalconWindow);
                        hObjectCross.Dispose();
                        break;
                    case "扫描单个芯片":
                        //HOperatorSet.WriteImage(e.ImageHobject, "bmp", 0, "D;/4.bmp");
                        ilChipModel.ShowImg(e); // 简单模拟是拼图的结果
                        break;

                    case "制作芯片模板":
                        dlvwProgress.Next();
                        if (scanIsOneFoc)
                        {
                            programConfig.DieWidth = step4BottomRightPoint.X - step4TopLeftPoint.X;
                            programConfig.DieHeight = step4TopLeftPoint.Y - step4BottomRightPoint.Y;
                            JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
                            HOperatorSet.WriteImage(ilChipModel.hImage, "bmp", 0, @"D:\QTWaferProgram\fofofofofof\main.bmp");
                            
                            DialogCreateModel dialogCreateModel = new DialogCreateModel(ilChipModel.hImage, programConfig, step4TopLeftPointPixel, step4BottomRightPointPixel);
                            dialogCreateModel.ShowDialog();
                            if (dialogCreateModel.DialogResult == DialogResult.OK)
                            {
                                dlvwProgress.Next();
                            }
                        }
                        break;
                    default:
                        ilChipModel.ShowImg(e);
                        ilChipModel.DrawCross(e.Height / 2, e.Width / 2);
                        break;
                }
            }
            catch (Exception er) { }
        }

        private void Step4_DarkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                //case "机械坐标左上角":
                //    step4TopLeftPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                //    tbStep4TopLeftX.Text = step4TopLeftPoint.X.ToString();
                //    tbStep4TopLeftY.Text = step4TopLeftPoint.Y.ToString();
                //    dlvwProgress.SetStartNum(0);
                //    dlvwProgress.Next();

                //    break;
                //case "机械坐标右下角":
                //    step4BottomRightPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                //    tbStep4BottomRightX.Text = step4BottomRightPoint.X.ToString();
                //    tbStep4BottomRightY.Text = step4BottomRightPoint.Y.ToString();
                //    dlvwProgress.SetStartNum(1);
                //    dlvwProgress.Next();
                //    _=ScanOneChipAsync();


                    //break;
                case "制作芯片模板":
                    dlvwProgress.SetStartNum(3);
                    //if (dlvwProgress.NowId() != 3) { DarkMessageBox.ShowError("请先按流程操作"); return; }
                    _ = ScanOneChipAsync();
                    break;
                case "框选芯片区域":
                    dlvwProgress.SetStartNum(3);
                    ilChipModel.selected_drawing_object.Dispose();
                    ilChipModel.DrawRectange1(ilChipModel.AllCols / 2, ilChipModel.AllRows / 2);
                    break;
                case "确认芯片对角":
                    ilChipModel.DrawCross(step4TopLeftPointPixel.Y, step4TopLeftPointPixel.X);
                    ilChipModel.DrawCross(step4BottomRightPointPixel.Y, step4BottomRightPointPixel.X);
                    break;
                case "移动到左上角点":
                    MotorsControl.MovePoint2D(20, Convert.ToInt32(tbStep4TopLeftX.Text), Convert.ToInt32(tbStep4TopLeftY.Text), ax, ay, true);
                    MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(20); // 等差模式 
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case "移动到右下角点":
                    MotorsControl.MovePoint2D(20, Convert.ToInt32(tbStep4BottomRightX.Text), Convert.ToInt32(tbStep4BottomRightY.Text), ax, ay, true);
                    MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(20); // 等差模式 
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case "确定左上角":
                    dlvwProgress.SetStartNum(1);
                    break;
                case "确定右下角":
                    dlvwProgress.SetStartNum(2);
                    break;
                case "扫描单个芯片":
                    if (step4TopLeftPoint == null || step4BottomRightPoint == null) { DarkMessageBox.ShowWarning("请先框选相应的两个角"); return; }
                    dlvwProgress.SetStartNum(2);
                    _ = ScanOneChipAsync();
                    break;
            }
        }
        #endregion

        #region 第五步 制作图谱
        private int photoInterval = -1; // 第五步这个飞拍的时候的间距，这个回实时改变
        private int yInterval = -1; //y轴移动的间距，这个回实时改变
        private bool goBack = false;
        private PointInfo prpreviousGoBackPoint;
        private bool stop = false;

        private HTuple crossModel; // 四个芯片的十字夹角模板
        private double crossRegionHalfRow, crossRegionHalfCol; // 模板的一半宽高
        private bool alalreadyFindCross = false; // 表示是否已经找到四个芯片的十字夹角
        private PointInfo crossPoint; // 找到的十字夹角坐标

        private int num = 0;
        private async Task Step5Ini()
        {
            await Task.Run(() =>
            {
                MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(100); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.MovePoint2D(100, programConfig.WaferCenter.X, programConfig.WaferCenter.Y, ax, ay, true);
                MotorsControl.stopCompare();
                MotorsControl.startCompare();
                MotorsControl.stopCompare();
            });
        }
        private void hswcStep5Model_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "确定切割道位置":
                        PointInfo tempPoint = new PointInfo() {X  = Convert.ToInt32(ilStep5Model.X), Y = Convert.ToInt32(ilStep5Model.Y) };
                        double row1 = tempPoint.Row() - programConfig.CutRoadWidthPixel / 2;
                        double col1 = tempPoint.Col() - 100;
                        double row2 = tempPoint.Row() + programConfig.CutRoadWidthPixel / 2;
                        double col2 = tempPoint.Col() + 100;
                        HOperatorSet.GenRectangle1(out HObject rect1, row1, col1, row2, col2);
                        row1 = tempPoint.Row() - 100;
                        col1 = tempPoint.Col() - programConfig.CutRoadWidthPixel / 2;
                        row2 = tempPoint.Row() + 100;
                        col2 = tempPoint.Col() + programConfig.CutRoadWidthPixel / 2;
                        HOperatorSet.GenRectangle1(out HObject rect2, row1, col1, row2, col2);
                        HOperatorSet.ConcatObj(rect1, rect2, out HObject rect);
                        HOperatorSet.SetColor(hswcStep5Model.HalconWindow, "green");
                        HOperatorSet.DispObj(rect, hswcStep5Model.HalconWindow);
                        rect1.Dispose();
                        rect2.Dispose();
                        rect.Dispose();


                        #region 计算距离中心点的偏移
                        int offsetx = tempPoint.Col() - ilChipModel.AllCols / 2;
                        int offsety = tempPoint.Row() - ilChipModel.AllRows / 2;

                        double offsetXPulse = ProgramConfig.GetXPulseByPixel(tempPoint.Col() - ilChipModel.AllCols / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
                        double offsetYPulse = ProgramConfig.GetYPulseByPixel(tempPoint.Row() - ilChipModel.AllRows / 2, config.ActualPixelLenght, programConfig.ObjectiveLense);// 由于halcon的图像坐标和运动周相反，所以y要乘以-1
                        #endregion
                        PointInfo nowPos = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                        nowPos.X += Convert.ToInt32(offsetXPulse);
                        nowPos.Y += Convert.ToInt32(offsetYPulse);

                        MotorsControl.MovePoint2D(20, nowPos.X, nowPos.Y, ax, ay);
                        MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                        MotorsControl.setCompareData_Pso(20); // 等差模式 
                        MotorsControl.startCompare();
                        MotorsControl.stopCompare();


                        break;
                    case "选择芯片的右下角":
                        tempPoint = ilChipModel.DrawCrossRet();
                        #region 计算距离中心点的偏移
                        offsetXPulse = ProgramConfig.GetXPulseByPixel(tempPoint.Col() - ilChipModel.AllCols / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
                        offsetYPulse = ProgramConfig.GetYPulseByPixel(tempPoint.Row() - ilChipModel.AllRows / 2, config.ActualPixelLenght, programConfig.ObjectiveLense); // 由于halcon的图像坐标和运动周相反，所以y要乘以-1
                        #endregion
                        step4BottomRightPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                        step4BottomRightPoint.X += Convert.ToInt32(offsetXPulse);
                        step4BottomRightPoint.Y += Convert.ToInt32(offsetYPulse);
                        tbStep4BottomRightX.Text = step4BottomRightPoint.X.ToString();
                        tbStep4BottomRightY.Text = step4BottomRightPoint.Y.ToString();
                        step4BottomRightPointPixel = new Point(tempPoint.X, tempPoint.Y);
                        //dlvwProgress.Next();
                        break;
                }
            }
            catch (Exception er) { }
        }
        private void HswcStep5Model_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                ilStep5Model.X = Convert.ToInt32(e.X);
                ilStep5Model.Y = Convert.ToInt32(e.Y);
            }
            catch (Exception er) { }
        }
        private void 测量距离ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlvwProgress.SetStartNum(0);
            hswcStep5Model.HMouseDoubleClick += HswcStep5Model_HMouseDoubleClick;
            ilStep5Model.Drawline();
        }

        private void HswcStep5Model_HMouseDoubleClick(object sender, HMouseEventArgs e)
        {
            double Row1 = ilStep5Model.line.GetDrawingObjectParams("row1");
            double Row2 = ilStep5Model.line.GetDrawingObjectParams("row2");
            double Col1 = ilStep5Model.line.GetDrawingObjectParams("column1");
            double Col2 = ilStep5Model.line.GetDrawingObjectParams("column2");
            HOperatorSet.DistancePp(0, Col1, 0, Col2, out HTuple distance);
            hswcStep5Model.HMouseDoubleClick -= HswcStep5Model_HMouseDoubleClick;
            ilStep5Model.line.Dispose();
            programConfig.CutRoadWidthPixel = distance.D;
            programConfig.CutRoadWidth = ProgramConfig.GetXPulseByPixel(Convert.ToInt32(distance.D), config.ActualPixelLenght, programConfig.ObjectiveLense);
            JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
            dlvwProgress.Next();
            MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(800); // 等差模式 
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
        }

        private void 画一个模板区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilStep5Model.ClearallTool();
            ilStep5Model.DrawRectange1();
            dlvwProgress.SetStartNum(2);
        }
        private void 保存矩形区域为模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HOperatorSet.GenEmptyObj(out HObject region);
            foreach (var obj in ilStep5Model.drawingObjects)
            {
                HOperatorSet.Union2(obj.GetDrawingObjectIconic(), region, out region);
            }
            HOperatorSet.Rgb1ToGray(ilStep5Model.hImage, out HObject grayImage);
            HOperatorSet.ReduceDomain(grayImage, region, out HObject imageReduced);
            HOperatorSet.CreateShapeModel(imageReduced, "auto", -0.18, 0.36, "auto", "auto", "use_polarity", "auto", "auto", out crossModel);
            HOperatorSet.WriteRegion(region, programConfig.GetWaferMarkModelRegionFileName());
            HOperatorSet.WriteShapeModel(crossModel, programConfig.GetWaferMarkModelFileName());

            HOperatorSet.SmallestRectangle1(region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
            crossRegionHalfRow = (row2.D - row1.D) / 2;
            crossRegionHalfCol = (col2.D - col1.D) / 2;

            grayImage.Dispose();
            imageReduced.Dispose();
            region.Dispose();
            ilStep5Model.ClearallTool();
        }

        private void Step5_DarkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "制作Mark点模板":
                    dlvwProgress.SetStartNum(2);
                    //DialogCreateModel dialogCreateModel = new DialogCreateModel(ilStep5Model.hImage, programConfig.GetWaferMarkModelRegionFileName(), programConfig.GetWaferMarkModelFileName(), "晶圆扫描模型制作");
                    //dialogCreateModel.ShowDialog();
                    //if (dialogCreateModel.DialogResult == DialogResult.OK)
                    //{
                    //    dlvwProgress.Next();
                    //}
                    break;
                case "扫描整个晶圆":
                    stop = false;
                    prpreviousGoBackPoint = null;
                    if (dlvwProgress.NowId() <2) { DarkMessageBox.ShowError("请先按流程操作"); return; }
                    num = 0;
                    mVCameraHelper.ReSetNum();
                    mVCameraHelper.testBitmap = false;
                    //HOperatorSet.ReadShapeModel(programConfig.GetFlyModelFileName(), out DetectModelId);
                    //HOperatorSet.ReadRegion(out HObject region, programConfig.GetChipModelRegionFileName());
                    //HOperatorSet.SmallestRectangle1(region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                    //halfRow = (row2.D - row1.D) / 2;
                    //halfCol = (col2.D - col1.D) / 2;
                    //region.Dispose();
                    dlvwProgress.SetStartNum(3);

                    _ = AutoStep5ScanWafer();
                    //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                    //_ = Step5SearchAsync(pf, Convert.ToInt32(programConfig.DieWidth), Convert.ToInt32(programConfig.DieHeight), Convert.ToInt32(programConfig.CutRoadWidth));
                    break;
                case "读取当前位置":
                    PointInfo po = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                    tb5X.Text = po.X.ToString();
                    tb5Y.Text = po.Y.ToString();
                    break;
                case "移动到上面的位置":
                    MotorsControl.MovePoint2D(100, Convert.ToInt32(tb5X.Text),Convert.ToInt32(tb5Y.Text), ax, ay, true);
                    break;
                case "关闭相机":
                    mVCameraHelper.CloseCameras();
                    break;
                case "打开相机":
                    mVCameraHelper.OpenCameras();
                    break;
                case "模拟边沿":
                    MotorsControl.StopAxis(ax.Id, 0);
                    break;
                case "停止扫描":
                    MotorsControl.StopAxis(ax.Id, 0);
                    stop = true;
                    break;
            }
        }

        private async Task AutoStep5ScanWafer()
        {
            // 第一步先搜索边沿查询存在十字 扫描整个晶圆制作图谱第一步， 确定上边沿的芯片十字角，和初始化一些数据
            await FindTopCross();
            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
            }));
            // 已经取到了第一个点 crossPoint，开始计算这个点左右的圆边的坐标
            // 首先计算左边园上的点
            //double dY = crossPoint.Y - programConfig.WaferRadius;

            //double da = Math.Sqrt(Math.Abs(programConfig.WaferRadius * programConfig.WaferRadius - dY * dY));
            ////园边上与半径垂直的的第一个点为p1,第二个点为p2
            //PointInfo p1 = new PointInfo() { X = Convert.ToInt32(programConfig.WaferCenter.X - da), Y = Convert.ToInt32(programConfig.WaferCenter.Y + dY) };
            //PointInfo p2 = new PointInfo() { X = Convert.ToInt32(programConfig.WaferCenter.X + da), Y = Convert.ToInt32(programConfig.WaferCenter.Y + dY) };

            //List<PointInfo> scanFlyPoints = new List<PointInfo>();

            //// 这里只是一个大概的值，后面所有的都要实时更新掉数值
            ////while(dY > programConfig.WaferCenter.Y - programConfig.WaferRadius)
            ////{
            ////    dY = 
            ////}
            //MotorsControl.MovePoint2D(100, p1.X, p1.Y, ax, ay, false);
            PointInfo topLeftPoint = new PointInfo(programConfig.WaferCenter.X - programConfig.WaferRadius, programConfig.WaferCenter.X + programConfig.WaferRadius);
            PointInfo bottomRightPoint = new PointInfo(programConfig.WaferCenter.X + programConfig.WaferRadius, programConfig.WaferCenter.X - programConfig.WaferRadius);
            photoInterval =Convert.ToInt32(programConfig.DieWidth + programConfig.CutRoadWidth / 2);
            await StrangeFlyAsync(topLeftPoint, bottomRightPoint, crossPoint);
            //实打实
            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
                Die[,] data = new Die[65, 65];
                for (int x = 0; x < 65; x++)
                {
                    for (int y = 0; y < 65; y++)
                    {
                        //if (y > 18 && y < 21 && x >= ss && x < 41 - ss)
                        //{
                        //    data[x, y] = new Die() { ColorIndex = 1, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                        //}
                        //else
                        data[x, y] = new Die() { ColorIndex = 0, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    }
                }
                JsonHelper.Serialize(data, programConfig.GetMappingFileName());
                step5Wafermap.Dataset = data;
            }));



        }


        /// <summary>
        /// 扫描整个晶圆制作图谱第一步， 确定上边沿的芯片十字角，和初始化一些数据
        /// </summary>
        /// <returns></returns>
        private async Task FindTopCross()
        {
            alalreadyFindCross = false;
            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
            }));
            int distance = Convert.ToInt32(programConfig.DieHeight * 2); // 扫描两个芯片的高度肯定会扫到一个
            PointInfo startPoint = new PointInfo() { X = programConfig.WaferCenter.X, Y = Convert.ToInt32(programConfig.WaferCenter.Y + programConfig.WaferRadius) };
            PointInfo endPoint = new PointInfo() { X = startPoint.X + distance, Y = startPoint.Y - distance };
            #region 开始搜寻上面的十字点
            int flyInterval = 200;
            MotorsControl.stopCompare();
            await Task.Run(() => {
                int step = Convert.ToInt32(programConfig.DieWidth / 2);
                int width = endPoint.X - startPoint.X;
                int height = startPoint.Y - endPoint.Y;
                //programConfig.DieWidth = width;
                //programConfig.DieHeight = height;
                int flyLines = width / step; // 飞拍的行数
                //int startY = startPoint.Y;
                int startX = startPoint.X;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                Axis ay = config.Axes.Find(v => v.Id == 1);
                MotorsControl.MovePoint2D(50, startPoint.X, startPoint.Y, ax, ay, true);
                MotorsControl.setCompareMode(ay.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                #region 这里是个小范围的飞拍
                for (int i = 0; i < flyLines; i++)
                {
                    int location1 = 1;
                    if (i % 2 == 0)
                    {
                        location1 = endPoint.Y;
                    }
                    else
                    {
                        location1 = startPoint.Y;
                    }
                    //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                    MotorsControl.startCompare();
                    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 20, location1, true);
                    MotorsControl.stopCompare();
                    if (alalreadyFindCross) break;
                    if (i < flyLines - 1)
                    {
                        startX = Convert.ToInt32(startX + programConfig.DieWidth / 2);
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, startX, true);
                    }
                }
                #endregion
            });
            Task.WaitAll(tasklst.ToArray());
            if (!alalreadyFindCross)
            {
                this.BeginInvoke(new Action(() =>
                {
                    dlvwProgress.Error();
                    DarkMessageBox.ShowError("未搜寻到边沿芯片，请重新制作模板搜寻");
                }));
            }

            #endregion
        }


        /// <summary>
        /// z字形状在晶圆内部游走，遇到边沿就返回走
        /// </summary>
        /// <param name="topLeftPoint">晶圆的最小外接矩形左上角</param>
        /// <param name="bottomRightPoint">晶圆的最小外接矩形右下角</param>
        /// <param name="markPoint">markPoint起始点一定在晶圆的最小外接矩形中间</param>
        /// <returns></returns>
        private async Task StrangeFlyAsync(PointInfo topLeftPoint, PointInfo bottomRightPoint, PointInfo markPoint)
        {
            await Task.Run(() =>
            {
                yInterval = markPoint.Y;
                MotorsControl.MovePoint2D(100, markPoint.X, markPoint.Y, ax, ay, true);
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(photoInterval); // 等差模式
                MotorsControl.startCompare();
                while (yInterval > bottomRightPoint.Y && !stop)
                {
                    if (goBack)
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, topLeftPoint.X, true);
                    else
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, bottomRightPoint.X, true);
                    yInterval -= Convert.ToInt32(programConfig.CutRoadWidth / 2 + programConfig.DieHeight);
                    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 20, yInterval, true);
                    goBack = !goBack;
                }
                MotorsControl.stopCompare();
            });
        }

        /// <summary>
        /// 飞拍
        /// </summary>
        /// <param name="markPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="dieWidth"></param>
        /// <param name="dieHeight"></param>
        /// <param name="cutRoadWidth"></param>
        /// <param name="fov"></param>
        /// <returns></returns>
        private async Task Step5SearchAsync(PointInfo markPoint, int dieWidth, int dieHeight, int cutRoadWidth, int fov = 5000)
        {
            MotorsControl.stopCompare();
            await Task.Run(() =>
            {
                config = FsmHelper.GetConfig();
                PointInfo startPoint, endPoint;
                //if(programConfig.WaferSize== WaferSize.INCH8)
                //{
                //    startPoint = config.Inch8SavePoints.Find(v => v.Remark.Equals("扫描左上角"));
                //    endPoint = config.Inch8SavePoints.Find(v => v.Remark.Equals("扫描右下角"));
                //}
                //else
                //{
                //    startPoint = config.Inch6SavePoints.Find(v => v.Remark.Equals("扫描左上角"));
                //    endPoint = config.Inch6SavePoints.Find(v => v.Remark.Equals("扫描右下角"));
                //}
                startPoint = new PointInfo() { X = Convert.ToInt32(programConfig.WaferCenter.X - programConfig.WaferRadius), Y = Convert.ToInt32(programConfig.WaferCenter.Y + programConfig.WaferRadius) };
                endPoint = new PointInfo() { X = Convert.ToInt32(programConfig.WaferCenter.X + programConfig.WaferRadius), Y = Convert.ToInt32(programConfig.WaferCenter.Y - programConfig.WaferRadius) };
                PointInfo temp = new PointInfo() { X = markPoint.X, Y = markPoint.Y, Z = markPoint.Z, Remark = markPoint.Remark };
                PointInfo temp2 = new PointInfo() { X = markPoint.X, Y = markPoint.Y, Z = markPoint.Z, Remark = markPoint.Remark };
                #region 计算最终的左上角
                while (temp.Y < startPoint.Y)
                {
                    temp.Y += dieHeight + cutRoadWidth / 2;
                }
                while (temp.X > startPoint.X)
                {
                    temp.X = temp.X - dieWidth - cutRoadWidth / 2;
                }
                startPoint.X = temp.X;
                startPoint.Y = temp.Y;
                #endregion
                int a = 0;
                #region 计算最终的右下角点
                while (temp2.Y > endPoint.Y)
                {
                    temp2.Y = temp2.Y - dieHeight - cutRoadWidth / 2;
                }
                while (temp2.X < endPoint.X)
                {
                    temp2.X = temp2.X + dieWidth + cutRoadWidth / 2;
                }
                endPoint.X = temp2.X;
                endPoint.Y = temp2.Y;
                #endregion

                int flyInterval = dieWidth + cutRoadWidth / 2;
                int width = endPoint.X - startPoint.X;
                int height = startPoint.Y - endPoint.Y;
                int flyLines = height / flyInterval; // 飞拍的行数
                int startY = startPoint.Y;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                Axis ay = config.Axes.Find(v => v.Id == 1);
                MotorsControl.MovePoint2D(100, startPoint.X, startPoint.Y, ax, ay, true);
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                if (flyLines == 0 && width / flyInterval == 0) //如果X和Y除以间距都是0的话那就在中心拍一张
                {
                    MotorsControl.MovePoint2D(100, startPoint.X + width / 2, startPoint.Y + height / 2, ax, ay, true);
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                }
                else // 这里飞拍还要判断下XY方向
                {
                    #region 这里是个小范围的飞拍
                    for (int i = 0; i < flyLines; i++)
                    {
                        //break;
                        int location1 = 1;
                        if (i % 2 == 0)
                        {
                            location1 = endPoint.X;
                        }
                        else
                        {
                            location1 = startPoint.X;
                        }
                        //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                        MotorsControl.startCompare();
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, location1, true);
                        MotorsControl.stopCompare();
                        if (i < flyLines - 1)
                        {
                            startY = startY - flyInterval;
                            MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 50, startY, true);
                            //Thread.Sleep(100);
                        }
                    }
                    #endregion
                }
            });
        }

        private void MVCameraHelper_CameraImageCallBack_Step5(object sender, ImageArgs e)
        {
            try
            {
                num++;
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "测量切割道的宽度":
                        ilStep5Model.ShowImg(e);
                        break;
                    case "确定切割道位置": 
                        ilStep5Model.ShowImg(e);
                        double row1 = e.Height / 2 - programConfig.CutRoadWidthPixel / 2;
                        double col1 = e.Width / 2 - 100;
                        double row2 = e.Height / 2 + programConfig.CutRoadWidthPixel / 2;
                        double col2 = e.Width / 2 + 100;
                        HOperatorSet.GenRectangle1(out HObject rect1, row1, col1, row2, col2);
                        row1 = e.Height / 2 - 100;
                        col1 = e.Width / 2 - programConfig.CutRoadWidthPixel / 2;
                        row2 = e.Height / 2 + 100;
                        col2 = e.Width / 2 + programConfig.CutRoadWidthPixel / 2;
                        HOperatorSet.GenRectangle1(out HObject rect2, row1, col1, row2, col2);
                        HOperatorSet.ConcatObj(rect1, rect2, out HObject rect);
                        HOperatorSet.SetColor(hswcStep5Model.HalconWindow, "green");
                        HOperatorSet.DispObj(rect, hswcStep5Model.HalconWindow);
                        rect1.Dispose();
                        rect2.Dispose();
                        rect.Dispose();
                        break;
                    case "制作芯片十字模板":
                        //ilChipModel.ShowImg(e);
                        //HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, 500, 0);
                        //HOperatorSet.SetColor(hswcChipModel.HalconWindow, "red");
                        //HOperatorSet.DispObj(hObjectCross, hswcChipModel.HalconWindow);
                        //hObjectCross.Dispose();
                        break;
                    case "开始扫描整片晶圆":
                        Debug.WriteLine(num+ " 我是飞拍矫正");
                        // 这里先保存上一步的模板信息
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;

                            sw.Restart();
                            HOperatorSet.Rgb1ToGray(imgArg.ImageHobject, out HObject grayImage);
                            sw.Stop();
                            Debug.WriteLine(num + "灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
                            sw.Restart();
                            HOperatorSet.FindShapeModel(grayImage, DetectModelId, -0.18, 0.36, 0.5, 1, 0.5, "least_squares", 4, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                            sw.Stop();
                            Debug.WriteLine(num + "FindModel-耗时(ms):" + sw.ElapsedMilliseconds);
                    
                            //read_region(aa, 'D:/QTWaferProgram/test/chipModelRegion.hobj')
                            //smallest_rectangle1(aa, roww1, coll1, roww2, coll2)
                            //halfw:= (coll2 - coll1) / 2
                            //halfh:= (roww2 - roww1) / 2
                            //gen_rectangle1(rect, Row3 - halfh, Column3 - halfw, Row3 + halfh, Column3 + halfw)
                            //reduce_domain(Image2, rect, ImageReduced)

                            //find_ncc_model (GrayImage1, ModelID, -0.39, 0.79, 0.8, 1, 0.5, 'true', 4, Row, Column, Angle, Score)
                            //HOperatorSet.FindNccModel(grayImage, DetectModelId, -0.18, 0.18, 0.8, 1, 0.3, "true", 4, out HTuple Row, out HTuple Column, out HTuple Angle, out HTuple Score);
                            grayImage.Dispose();
                            if (Row.Length > 0 && Col.Length > 0)
                            {
                                sw.Restart();
                                HOperatorSet.GenRectangle1(out HObject region, Row - halfRow, Col - halfCol, Row + halfRow, Col + halfCol);
                                sw.Stop();
                             
                                Debug.WriteLine(num + "GenRectangle1-耗时(ms):" + sw.ElapsedMilliseconds);
                                sw.Restart();
                                HOperatorSet.ReduceDomain(imgArg.ImageHobject, region, out HObject ress);
                                sw.Stop();
                                sw.Restart();
                                Debug.WriteLine(num + "ReduceDomain-耗时(ms):" + sw.ElapsedMilliseconds);
                                //HOperatorSet.WriteImage(ress, "jpg", 0, @"D:\芯片HOBJECT_modelres\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                                sw.Restart();
                                Debug.WriteLine(num + "WriteImage-耗时(ms):" + sw.ElapsedMilliseconds);
                                ress.Dispose();
                                region.Dispose();

                                //如果模板结果中心在图像左边就+，右边就减
                                double offsetX = Col.D - imgArg.Width / 2;
                                double offsetXPulse = Math.Abs(ProgramConfig.GetXPulseByPixel(Convert.ToInt32(offsetX), config.ActualPixelLenght, programConfig.ObjectiveLense));
                                if (offsetX > 0)
                                {
                                    offsetXPulse = -offsetXPulse;
                                }
                                MotorsControl.setCompareData_Pso_Offset(Convert.ToInt32(offsetXPulse));
                                sw.Stop();
                                Debug.WriteLine(num + " 我是飞拍矫正-----------已经矫正-耗时(ms):" + sw.ElapsedMilliseconds);
                                //   MotorsControl.setCompareData_Pso(int.Parse(dtebInterval.Text)); // 等差模式
                                //MotorsControl.setCompareData_Pso();
                            }
                            sw = null;
                            if (imgArg.ImageType == ImageArgs.ImageTypeEn.HOBJECT)
                            {
                                if (imgArg.ImageHobject != null && imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "jpg", 0, @"D:\芯片HOBJECT\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                            }
                            //else
                            //{
                            //    if (imgArg.ImageBitmap != null) imgArg.ImageBitmap.Save(@"D:\芯片BITMAP\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                            //}
                            imgArg.Dispose();
                            //{
                            //    ilStep5Model.ShowImg(val);
                            //}), e);
                            return false;
                        }, e));
                        break;
                    case "正在扫描晶圆":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            if (!hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 95, 100))
                            {
                                if (prpreviousGoBackPoint == null)
                                {
                                    prpreviousGoBackPoint = new PointInfo(imgArg.XPulse, imgArg.XPulse);
                                    MotorsControl.StopAxis(ax.Id, 0);
                                }
                                else
                                {
                                    if (Math.Abs(prpreviousGoBackPoint.X - imgArg.XPulse) > 30000)
                                    {
                                        MotorsControl.StopAxis(ax.Id, 0);
                                        prpreviousGoBackPoint = new PointInfo(imgArg.XPulse, imgArg.XPulse);
                                    }
                                }
                      
                            }
                            this.BeginInvoke(new Action<HObject>((ho) =>
                            {
                                ilStep5Model.ShowImg(ho);
                            }), imgArg.ImageHobject);
                            return false;
                        }, e));
                        break;
                    case "搜寻边沿芯片":
                        if (dlvwProgress.NowItem().TextColor == Color.Yellow)
                        {
                            ilStep5Model.ShowImg(e);
                            return;
                        }
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            if (alalreadyFindCross) return false;
                            ImageArgs imgArg = (ImageArgs)obs;
                            HOperatorSet.WriteImage(imgArg.ImageHobject, "jpg", 0, @"D:\芯片HOBJECT_cross\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                            HOperatorSet.Rgb1ToGray(imgArg.ImageHobject, out HObject grayImage);
                            HOperatorSet.FindShapeModel(grayImage, crossModel, -0.18, 0.36, 0.5, 1, 0.5, "least_squares", 4, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                            grayImage.Dispose();
                            if (Row.Length > 0 && Col.Length > 0)
                            {
                                alalreadyFindCross = true;
                                MotorsControl.stopCompare();
                                MotorsControl.StopAxis(ay.Id, 1);
                                this.BeginInvoke(new Action(() =>
                                {
                                    dlvwProgress.Stop();
                                  
                                }));
                                crossPoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                                HOperatorSet.GenRectangle1(out HObject region, Row - crossRegionHalfRow, Col - crossRegionHalfCol, Row + crossRegionHalfRow, Col + crossRegionHalfCol);

                                ////如果模板结果中心在图像左边就+，右边就减
                                //double offsetX = Col.D - imgArg.Width / 2;
                                //double offsetXPulse = Math.Abs(ProgramConfig.GetXPulseByPixel(Convert.ToInt32(offsetX), config.PixelLenght));
                                //int tempFlyInterval = -1;
                                //if (offsetX > 0)
                                //{
                                //    tempFlyInterval = Convert.ToInt32(flyInterval + offsetXPulse);
                                //}
                                //else
                                //{
                                //    tempFlyInterval = Convert.ToInt32(flyInterval - offsetXPulse);
                                //}
                                //MotorsControl.setCompareData_Pso(tempFlyInterval);
                                //sw.Stop();
                                //Debug.WriteLine(" 我是飞拍矫正----间距:" + tempFlyInterval + "-------已经矫正-耗时(ms):" + sw.ElapsedMilliseconds);

                                ilStep5Model.ShowImg(imgArg.ImageHobject, region);
                            }
                            imgArg.Dispose();
                            return false;
                        }, e));
                        break;
                }
            }
            catch (Exception er) { }
        }
        #endregion

        #region 第六步 训练程式
        private DialogWait dialogWait;
        private int trainNum = 0;
        private HTuple VariationModelID, ChipModelId;

        private void pgdStep6Par_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            HOperatorSet.PrepareVariationModel(VariationModelID, programConfig.ChipDetectPar.AbsThreshold, programConfig.ChipDetectPar.VarThreshold);
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
            JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
        }

        private HObject detectRegion;
        private void MVCameraHelper_CameraImageCallBack_Step6(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    default:
                        ilStep6Test.ShowImg(e);
                        break;
                    case "开始训练":
                        trainNum--;
                        HOperatorSet.ReadRegion(out detectRegion, programConfig.GetChipModelRegionFileName());
                        try
                        {
                            HOperatorSet.Rgb1ToGray(e.ImageHobject, out HObject grayImg);

                            HOperatorSet.TrainVariationModel(grayImg, VariationModelID);
                        }
                        catch (HalconException er) { }

                        if (trainNum == 0)
                        {
                            //write_variation_model
                            HOperatorSet.WriteVariationModel(VariationModelID, programConfig.GetChipVariationModelFileName());
                            if (dialogWait != null) dialogWait.Close();
                            JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
                            this.BeginInvoke(new Action(() =>
                            {
                                dlvwProgress.Next();
                                dlvwProgress.Next();
                            }));
                        }
                        //ilStep5Model.ShowImg(e);
                        //rgb1_to_gray(ImageStd, ImageStd)
                        //train_variation_model(ImageStd, VariationModelID)
                        break;
                    case "测试":
                        Stopwatch sw = new Stopwatch();
                        sw.Restart();
                        HOperatorSet.Rgb1ToGray(e.ImageHobject, out HObject grayImage);
                        Debug.WriteLine("灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
                        sw.Restart();
                        HOperatorSet.FindShapeModel(grayImage, ChipModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent, programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                        Debug.WriteLine("FindShapeModel-耗时(ms):" + sw.ElapsedMilliseconds);
                        if (Score1.Length > 0)
                        {
                            sw.Restart();
                            ImageTrans(grayImage, out HObject fu, Row, Col, Angle1, programConfig.ChipModelPar.ShapeModelCenterRow, programConfig.ChipModelPar.ShapeModelCenterCol, 0, out HTuple HomMat2D);
                            Debug.WriteLine("ImageTrans-耗时(ms):" + sw.ElapsedMilliseconds);
                            HOperatorSet.CropRectangle1(fu, out HObject detectImg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);


                            //HOperatorSet.WriteImage(trainImg, "jpg", 0, "D:/fu.jpg");
                            try
                            {//compare_variation_model
                                sw.Restart();
                                HOperatorSet.CompareVariationModel(detectImg, out HObject region, VariationModelID);
                                Debug.WriteLine("CompareVariationModel-耗时(ms):" + sw.ElapsedMilliseconds);
                                sw.Restart();
                                HOperatorSet.AffineTransImage(e.ImageHobject, out HObject finalimg, HomMat2D, "constant", "false");
                                Debug.WriteLine("AffineTransImage-耗时(ms):" + sw.ElapsedMilliseconds);
                                sw.Restart();
                                HOperatorSet.CropRectangle1(finalimg, out HObject finalCropimg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
                                Debug.WriteLine("CropRectangle1-耗时(ms):" + sw.ElapsedMilliseconds);
                                //                                connection(Region, ConnectedRegions)
                                //select_shape(ConnectedRegions, SelectedRegions, 'area', 'and', area_min, area_max)

                                HOperatorSet.Connection(region, out HObject ConnectedRegions);
                                HOperatorSet.SelectShape(ConnectedRegions, out HObject selectRegions, "area", "and", programConfig.ChipDetectPar.AreaMin, programConfig.ChipDetectPar.AreaMax);
                                ilStep6Test.ShowImg(finalCropimg, selectRegions);
                                finalimg.Dispose();
                            }
                            catch (HalconException er) { ilStep6Test.ShowImg(e); }
                            fu.Dispose();
                            detectImg.Dispose();

                            grayImage.Dispose();
                        }
                        else
                        {
                            ilStep6Test.ShowImg(e);
                        }
                        break;
                }
            }
            catch (Exception er) { }
        }

        private void HswcStep6Test_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                ilStep6Test.X = Convert.ToInt32(e.X);
                ilStep6Test.Y = Convert.ToInt32(e.Y);
            }
            catch (Exception er) { }
        }
        private void step6Wafermap_OnDieClick(object sender, Die e)
        {
            e.ColorIndex = 1;
            trainNum++;
        }

        private void Step6_DarkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "打开测试":
                    dlvwProgress.SetStartNum(3);
                    dlbStep6Tips.Text = "点击单个芯片移动来测试结果";
                    if (programConfig.ChipDetectPar == null)
                    {
                        programConfig.ChipDetectPar = new ChipDetect { AbsThreshold = 20, VarThreshold = 2, AreaMin = 10, AreaMax = 999999 };
                    }
                    if (ChipModelId == null) HOperatorSet.ReadShapeModel(programConfig.GetChipModelFileName(), out ChipModelId);
                    HOperatorSet.ReadVariationModel(programConfig.GetChipVariationModelFileName(), out VariationModelID);
                    HOperatorSet.PrepareVariationModel(VariationModelID, programConfig.ChipDetectPar.AbsThreshold, programConfig.ChipDetectPar.VarThreshold);
                    pgdStep6Par.SelectedObject = programConfig.ChipDetectPar;
                    dspStep6Test.Visible = true;
                    break;
                case "重新选择":
                    dlbStep6Tips.Text = "请尽可能的分散选择需要训练的芯片";
                    dspStep6Test.Visible = false;
                    dlvwProgress.SetStartNum(0);
                    trainNum = 0;
                    Die[,] data = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
                    step6Wafermap.Dataset = data;
                    break;
                case "开始训练":
                    if (programConfig.ChipDetectPar == null)
                    {
                        programConfig.ChipDetectPar = new ChipDetect { AbsThreshold = 20, VarThreshold = 2, AreaMin = 10, AreaMax = 999999 };
                    }
                    dlbStep6Tips.Text = "请尽可能的分散选择需要训练的芯片";
                    dspStep6Test.Visible = false;
                    dlvwProgress.SetStartNum(1);
                    dialogWait = new DialogWait("正在训练中......");
                    dialogWait.Show();
                    HOperatorSet.ReadVariationModel(programConfig.GetChipVariationModelFileName(), out VariationModelID);
                    HOperatorSet.ReadShapeModel(programConfig.GetChipModelFileName(), out ChipModelId);
                    string[] files = Directory.GetFiles("D:/train");
                    foreach (var f in files)
                    {
                        trainNum--;
                        HOperatorSet.ReadImage(out HObject img, f);
                        HOperatorSet.Rgb1ToGray(img, out HObject grayImage);
                        //img.Dispose();

                        HOperatorSet.FindShapeModel(grayImage, ChipModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent, programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                        if (Score1.Length > 0)
                        {
                            ImageTrans(grayImage, out HObject fu, Row, Col, Angle1, programConfig.ChipModelPar.ShapeModelCenterRow, programConfig.ChipModelPar.ShapeModelCenterCol, 0, out HTuple aaa);
                            HOperatorSet.CropRectangle1(fu, out HObject trainImg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
                            try
                            {
                                HOperatorSet.TrainVariationModel(trainImg, VariationModelID);
                            }
                            catch (HalconException er) { DarkMessageBox.ShowError("训练失败"); }
                        }
                        if (trainNum == 0) { }
                    }
                    HOperatorSet.WriteVariationModel(VariationModelID, programConfig.GetChipVariationModelFileName());
                    HOperatorSet.PrepareVariationModel(VariationModelID, programConfig.ChipDetectPar.AbsThreshold, programConfig.ChipDetectPar.VarThreshold);
                    if (dialogWait != null) dialogWait.Close();
                    JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
                    dlvwProgress.Next();
                    dlvwProgress.Next();
                    //read_variation_model('./ChipVariationModel', VariationModelID)
                    break;
            }
        }

        /*
            *矫正
            Angle5:=0
            *输入
            *GrayImage1 为采集的图像灰度图
            *Row Column Angle模板匹配的结果
            *Row5 Column5 Angle5 为原始模板的中心坐标，Angle5这里一般为0

            *输出
            *ImageAffineTrans 为输出的图像，可以跟原模板图像对比一下，感受一下
            *HoMat2D 这里为转换矩阵*/
        private void ImageTrans(HObject GrayImage1, out HObject ImageAffineTrans, HTuple Row, HTuple Column, HTuple Angle, HTuple Row5, HTuple Column5, HTuple Angle5, out HTuple HomMat2D)
        {
            HOperatorSet.GenEmptyObj(out ImageAffineTrans);
            HomMat2D = new HTuple();
            try
            {
                //vector_angle_to_rigid(Row, Column, Angle, Row5, Column5, Angle5, HomMat2D)
                HOperatorSet.VectorAngleToRigid(Row, Column, Angle, Row5, Column5, Angle5, out HomMat2D);
                //affine_trans_image(GrayImage1, ImageAffineTrans, HomMat2D, 'constant', 'false')
                HOperatorSet.AffineTransImage(GrayImage1, out ImageAffineTrans, HomMat2D, "constant", "false");
            }
            catch (HalconException er) { }
        }

        #endregion
        /// <summary>
        /// 上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreviousStep_Click(object sender, EventArgs e)
        {
            if (DarkMessageBox.ShowWarning("返回上一步可能会造成数据丢失，确定返回么？", "提醒", DarkDialogButton.YesNo) == DialogResult.Yes)
            {
                darkStepViewer1.PreviousStep();
            }
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextStep_Click(object sender, EventArgs e)
        {
            if (darkStepViewer1.CurrentStep == 1)
            {
//#if DEBUG
//                Console.WriteLine("调试模式已开启");
//#else
//                if (!tsInich6.Checked && !tsInich8.Checked)
//                {
//                    DarkMessageBox.ShowWarning("请先开启真空阀再进行下一步");
//                    return;
//                }
//#endif
            }
            else if (darkStepViewer1.CurrentStep == 7)
            {
                this.Close();
            }
#if DEBUG
            darkStepViewer1.Complete();
#else
            if (dlvwProgress.NowItem().Text.Equals("已全部完成") || canSkip || dlvwProgress.NowId() >= dlvwProgress.Items.Count - 1)
                darkStepViewer1.Complete();
            else DarkMessageBox.ShowWarning("请先按流程执行完毕再进行下一步");
#endif
        }

        #region contextMenuStrip
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            string tag = (sender as ContextMenuStrip).SourceControl.Tag.ToString();
            HSmartWindowControl tempControl = null;
            switch (tag)
            {
                case "焦距调整/矫正":
                    tempControl = hswcFocus;
                    break;
                case "晶圆右边沿":
                    tempControl = hswcRight;
                    break;
                case "晶圆上边沿":
                    tempControl = hswcTop;
                    break;
                case "晶圆下边沿":
                    tempControl = hswcBottom;
                    break;
                case "扫描芯片>制作芯片模板":
                    tempControl = hswcChipModel;
                    break;
            }
            contextMenuStrip1.Tag = tempControl;
        }

        private void 保存图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag != null)
            {
                HSmartWindowControl tempControl = contextMenuStrip1.Tag as HSmartWindowControl;
                HOperatorSet.DumpWindowImage(out HObject saveImg, tempControl.HalconWindow);
                SaveFileDialog sfd = new SaveFileDialog();
                //sfd.Filter = "文本文件(*.txt)|*.txt|所有文件|*.*";//设置文件类型
                sfd.FileName = tempControl.Tag.ToString();//设置默认文件名
                sfd.DefaultExt = "bmp";//设置默认格式（可以不设）
                sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    HOperatorSet.WriteImage(saveImg, "bmp", 0, sfd.FileName);
                }
            }
        }


        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag != null)
            {
                HSmartWindowControl tempControl = contextMenuStrip1.Tag as HSmartWindowControl;
                HOperatorSet.ClearWindow(tempControl.HalconWindow);
            }

        }


        private void 加载本地图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Tag != null)
            {
                HSmartWindowControl tempControl = contextMenuStrip1.Tag as HSmartWindowControl;
                string filePath;
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "请选择图片";
                fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
                fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = Path.GetFullPath(fileDialog.FileName);
                    HOperatorSet.ReadImage(out HObject hImage, filePath);
                    fileDialog.Dispose();
                    switch(tempControl.Tag.ToString()){
                        case "晶圆右边沿":
                            ilRight.ShowImg(hImage);
                            break;
                        case "晶圆上边沿":
                            ilTop.ShowImg(hImage);
                            break;
                        case "晶圆下边沿":
                            ilBottom.ShowImg(hImage);
                            break;
                    }
                }
                else
                {
                    fileDialog.Dispose();
                }
            }
        }

      
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="e"></param>
        /// <param name="tempControl"></param>
        private void ShowImg(ImageArgs e, HSmartWindowControl tempControl)
        {
            if (tempControl != null)
            {
                //HOperatorSet.GetImageSize(e.ImageHobject, out HTuple Iwidth, out HTuple Iheight);
                HOperatorSet.SetPart(tempControl.HalconWindow, 0, 0, e.Height - 1, e.Width - 1);
                HOperatorSet.ClearWindow(tempControl.HalconWindow);
                HOperatorSet.DispObj(e.ImageHobject, tempControl.HalconWindow);
                tempControl.SetFullImagePart();
                //HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, e.Width, 0);
                //HOperatorSet.DispObj(hObjectCross, tempControl.HalconWindow);
                //new HDevelopExport();
                e.ImageHobject.Dispose();
            }
        }
        private void ShowObj(HObject e, HSmartWindowControl tempControl)
        {
            if (tempControl != null)
            {
                //HOperatorSet.GetImageSize(e, out HTuple Iwidth, out HTuple Iheight);
                //HOperatorSet.SetPart(tempControl.HalconWindow, 0, 0, e.Height - 1, e.Width - 1);
                //HOperatorSet.ClearWindow(tempControl.HalconWindow);
                HOperatorSet.DispObj(e, tempControl.HalconWindow);
                //tempControl.SetFullImagePart();
                //HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, e.Width, 0);
                //HOperatorSet.DispObj(hObjectCross, tempControl.HalconWindow);
                //new HDevelopExport();
                e.Dispose();
            }
        }
#endregion
    }
}
