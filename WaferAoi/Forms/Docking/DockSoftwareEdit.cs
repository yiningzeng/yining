using HalconDotNet;
using System;
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
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using static WaferAoi.Tools.Utils;

namespace WaferAoi
{
    public partial class DockSoftwareEdit : DarkDocument
    {
        /// <summary>
        /// 晶圆的中心XY
        /// </summary>
        private int waferCenterX, waferCenterY;

        InterLayerDraw ilFocus, ilRight, ilTop, ilBottom, ilChipModel, ilStep5Model;
        HDevProgramHelper hDevProgramHelper;
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(7));
        #region 对焦参数
        private int bestZPulse = 0; // 最好的z的高度
        private double bestZScore = 0; //最好的Z高度分值
        #endregion
        private MVCameraHelper mVCameraHelper;
        Config config;
        private ProgramConfig programConfig;

        List<PointInfo> pointInfos;

        public DockSoftwareEdit()
        {
            InitializeComponent();
            
            this.darkStepViewer1.OnStepChanged += DarkStepViewer1_OnStepChanged;
            hswcFocus.MouseWheel += hswcFocus.HSmartWindowControl_MouseWheel;
            hswcRight.MouseWheel += hswcRight.HSmartWindowControl_MouseWheel;
            hswcTop.MouseWheel += hswcTop.HSmartWindowControl_MouseWheel;
            hswcBottom.MouseWheel += hswcBottom.HSmartWindowControl_MouseWheel;
            hswcChipModel.MouseWheel += hswcChipModel.HSmartWindowControl_MouseWheel;
            hswcStep5Model.MouseWheel += hswcStep5Model.HSmartWindowControl_MouseWheel;

            hswcStep5Model.HMouseMove += HswcStep5Model_HMouseMove;
            IniData();
        }

        void IniData()
        {
            ilFocus = new InterLayerDraw(hswcFocus);
            ilRight = new InterLayerDraw(hswcRight);
            ilTop = new InterLayerDraw(hswcTop);
            ilBottom = new InterLayerDraw(hswcBottom);
            ilChipModel = new InterLayerDraw(hswcChipModel);
            ilStep5Model = new InterLayerDraw(hswcStep5Model);
            hDevProgramHelper = new HDevProgramHelper("圆心查找ver2.0.hdev");
            config = FsmHelper.GetConfig();
            #region 总流程步骤条
            List<DarkStepViewerItem> list = new List<DarkStepViewerItem>();
            list.Add(new DarkStepViewerItem("1", "放置晶圆", 1, "请放置晶圆", null));
            list.Add(new DarkStepViewerItem("2", "角度矫正", 2, "矫正晶圆的角度", null));
            list.Add(new DarkStepViewerItem("3", "中心矫正", 3, "计算晶圆的中心", null));
            list.Add(new DarkStepViewerItem("4", "单芯片扫描", 4, "制作芯片定位模板", null));
            list.Add(new DarkStepViewerItem("5", "图谱生成", 5, "生成晶圆相应的图谱", null));
            list.Add(new DarkStepViewerItem("6", "制作程式", 6, "制作芯片的模板信息来检测", null));
            list.Add(new DarkStepViewerItem("7", "程式测试", 7, "测试制作的模板信息的检测结果", null));
            list.Add(new DarkStepViewerItem("8", "保存程式", 8, "保存程式", null));
            darkStepViewer1.ListDataSource = list;
            darkStepViewer1.CurrentStep = 1;
            #endregion

            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
            dlvwProgress.Items.Add(new DarkListItem("开启真空阀"));
            dlvwProgress.SetStartNum(0);
          }

        public DockSoftwareEdit(string text, Image icon, ref MVCameraHelper mc, ProgramConfig pc = null) : this()
        {
            DockText = text;
            Icon = icon;
            programConfig = pc;
            mVCameraHelper = mc;
            mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
            pointInfos = programConfig.WaferSize == WaferSize.INCH6 ? config.Inch6SavePoints : config.Inch8SavePoints;
        }

        #region 运动控制
        /// <summary>
        /// 运动控制停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            MotorsControl.startCompare();
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
        }
        /// <summary>
        /// 运动控制，开始运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            switch (dlvwProgress.NowItem().Text)
            {
                case "手动移动芯片到图片中间区域":

                    break;
            }
            Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(50); // 等差模式 
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

        /// <summary>
        /// 相机回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MVCameraHelper_CameraImageCallBack(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "自动对焦":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            double tempScore = Utils.CalIntensity(imgArg.ImageHobject, 0.5);
                            if (bestZScore < tempScore)
                            {
#if DEBUG
                                bestZScore = tempScore;
                                bestZPulse = imgArg.ZPulse;
                                Debug.WriteLine("最优高度: " + bestZPulse + " 最高分数: " + bestZScore);
#endif
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
                        }, e));
                        break;
                    case "手动移动芯片到图片中间区域":
                        ilFocus.ShowImg(e);
                        break;
                    case "角度校正":
                        ilFocus.ShowImg(e);
                        break;
                    case "寻找右边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            Thread.Sleep(50);
                            this.BeginInvoke(new Action(() =>
                            {
                                ShowImg(e, hswcRight);
                            }));
                            return false;
                        }, e));
                        break;
                    case "寻找上边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            Thread.Sleep(50);
                            this.BeginInvoke(new Action(() =>
                            {
                                ShowImg(e, hswcTop);
                            }));
                            return false;
                        }, e));
                        break;
                    case "寻找下边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            Thread.Sleep(50);
                            this.BeginInvoke(new Action(() =>
                            {
                                ShowImg(e, hswcBottom);
                            }));
                            return false;
                        }, e));
                        break;
                }

            }
            catch (Exception er)
            {

            }
        }


        public void SetConfig(ProgramConfig pc)
        {
            programConfig = pc;
        }

        protected virtual void DarkStepViewer1_OnStepChanged(object sender, EventArgs e)
        {
            darkTabControl1.SelectedIndex = darkStepViewer1.CurrentStep - 1;
            switch (darkStepViewer1.CurrentStep)
            {
                case 1:
                    #region 第一步流程引导
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
                    dlvwProgress.Items.Add(new DarkListItem("开启真空阀"));
                    dlvwProgress.SetStartNum(0);
                    #endregion
                    break;
                case 2:
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("自动对焦"));
                    dlvwProgress.Items.Add(new DarkListItem("手动移动芯片到图片中间区域"));
                    dlvwProgress.Items.Add(new DarkListItem("角度校正"));
                    dlvwProgress.SetStartNum(0);
                    break;
                case 3:
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("开始"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找右边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找上边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("寻找下边沿"));
                    dlvwProgress.Items.Add(new DarkListItem("开始计算"));
                    dlvwProgress.Items.Add(new DarkListItem("完成"));
                    dlvwProgress.SetStartNum(0);
                    dlvwProgress.Stop();
                    break;
                case 4:
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("框选芯片的左上角和右下角"));
                    dlvwProgress.Items.Add(new DarkListItem("扫描单个芯片"));
                    dlvwProgress.SetStartNum(0);
                    //dlvwProgress.Stop();

                    mVCameraHelper.CameraImageCallBack -= MVCameraHelper_CameraImageCallBack;
                    mVCameraHelper.CameraImageCallBack -= MVCameraHelper_CameraImageCallBack_Step5;
                    mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack_Step4;

                    Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
                    Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
                    MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(800); // 等差模式 
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case 5:
                    dlvwProgress.Items.Clear();
                    dlvwProgress.Items.Add(new DarkListItem("测量切割道的宽度"));
                    dlvwProgress.Items.Add(new DarkListItem("移动芯片的切割道到图像中心"));
                    dlvwProgress.Items.Add(new DarkListItem("制作芯片十字模板"));
                    dlvwProgress.Items.Add(new DarkListItem("扫描整片晶圆"));
                    dlvwProgress.SetStartNum(0);
                    mVCameraHelper.CameraImageCallBack -= MVCameraHelper_CameraImageCallBack;
                    mVCameraHelper.CameraImageCallBack -= MVCameraHelper_CameraImageCallBack_Step4;
                    mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack_Step5;
                    Axis az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
                    MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(800); // 等差模式 
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
            }
        }


        private void darkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "第二步按流程自动执行":
                    _ = AutoRun2Async();
                    break;
                case "自动对焦":
                    btn.Enabled = false;
                    dlvwProgress.SetStartNum(0);
                    _ = StartFocusingAsync();
                    break;
                case "角度矫正":
                    dlvwProgress.SetStartNum(2);
                    _ = Rotate();
                    break;
                case "相机曝光":
                    mVCameraHelper.ShowSettingPage();
                    break;


                case "第三步按流程自动执行":
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
            }
        }

        #region 第一步 放置晶圆
        private void btnMoveToPlace_Click(object sender, EventArgs e)
        {
            Axis ax = config.Axes.Find(v => v.Id == 2);
            Axis ay = config.Axes.Find(v => v.Id == 1);
            MotorsControl.MovePoint2D(60, ax.StartPoint, ay.StartPoint, ax, ay);
            dlvwProgress.SetStartNum(0);
        }

        private void tsInich_CheckedChanged(object sender, EventArgs e)
        {
            var Ts = sender as JCS.ToggleSwitch;
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, Ts.Checked ? 1 : 0);
            switch (Ts.OnText)
            {
                case "6寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, Ts.Checked ? 1 : 0);
                    break;
                case "8寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, Ts.Checked ? 1 : 0);
                    break;
            }

            if (Ts.Checked)
            {
                dlvwProgress.Next(); dlvwProgress.Next();
            }
            else
            {
                dlvwProgress.Previous(); dlvwProgress.Previous();
            }
        }
        #endregion

        #region 第二步 方向角整

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
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //ilFocus.SaveImg("bmp", @"D:\矫正图像\" + fileName + ".bmp");
                //if (imageRotate.IsInitialized()) HOperatorSet.WriteImage(imageRotate, "bmp", 0, @"D:\矫正图像\" + fileName + "-Rotate.bmp");
                imageRotate.Dispose();
                Axis ar = config.Axes.Find(v => v.Remarks == "载盘旋转轴");
                //MotorsControl.startCompare();
                int posNow = MotorsControl.GetREncPos(ar.Id);
                MotorsControl.MoveTrap(ar.Id, ar.TrapPrm.Get(), 1, posNow + Convert.ToInt32(Degree.D * 1000), true);
                MotorsControl.setCompareMode(ar.Id, ar.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1, 50);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(50); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.stopCompare();
            });
        }
        /// <summary>
        /// 对焦FUNC
        /// </summary>
        /// <param name="e"></param>
        private async Task StartFocusingAsync()
        {
            bestZScore = 0;
            bestZPulse = 0;
            Axis az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
            //int zHeight = MotorsControl.GetZEncPos(az.Id);

            await Task.Run(() => {
                MotorsControl.stopCompare();
                Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
                Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
                PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
                MotorsControl.MovePoint2D(100, pointInfo.X, pointInfo.Y, ax, ay, true);
#if DEBUG
                MotorsControl.GoHome(az.Id, az.GoHomePar.Get(), out GSN.THomeStatus homeStatus);
#endif
                MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(300); // 等差模式 
                MotorsControl.startCompare();
                int maxH = 4000;
                int mixH = -10000;
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, maxH, true);
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, mixH, true);

                MotorsControl.setCompareData_Pso(150); // 等差模式 
                int range = 3000;
                int upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                int downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
                MotorsControl.setCompareData_Pso(100); // 等差模式 
                range = 1000;
                upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 3, upH, true);
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 3, downH, true);
                MotorsControl.setCompareData_Pso(50); // 等差模式
                range = 500;
                upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
                downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 1, upH, true);
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 1, downH, true);
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
        /// <summary>
        /// 步骤三自动执行
        /// </summary>
        /// <returns></returns>
        private async Task AutoRun3Async()
        {
            this.BeginInvoke(new Action(() =>
            {
                darkButton3.Enabled = false;
                darkButton4.Enabled = false;
                darkButton5.Enabled = false;
                dlvwProgress.SetStartNum(0);
            }));
            this.BeginInvoke(new Action(() => { dlvwProgress.Next(); }));

            await SearchAsync(pointInfos.Find(i => i.Remark == "右边沿搜索中心点"), int.Parse(tbRangeRight.Text));
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() => { dlvwProgress.Next(); }));

            await SearchAsync(pointInfos.Find(i => i.Remark == "上边沿搜索中心点"), int.Parse(tbRangeTop.Text));
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() => { dlvwProgress.Next(); }));

            await SearchAsync(pointInfos.Find(i => i.Remark == "下边沿搜索中心点"), int.Parse(tbRangeBottom.Text));
            Task.WaitAll(tasklst.ToArray());
            this.BeginInvoke(new Action(() =>
            {
                darkButton3.Enabled = true;
                darkButton4.Enabled = true;
                darkButton5.Enabled = true;
                dlvwProgress.Next();
            }));
        }
        /// <summary>
        /// 搜寻边沿
        /// </summary>
        /// <param name="pointInfo">搜寻范围中心信息</param>
        /// <param name="range">搜寻的正方形范围边长</param>
        /// <param name="flyInterval">相机飞拍的间隔</param>
        private async Task SearchAsync(PointInfo pointInfo, int range, int flyInterval = 500)
        {
            MotorsControl.stopCompare();
            await Task.Run(() => {
                flyInterval = 1500;
                int moveinterval = 3000; //这是移动的间隔，用来算行数
                int flyLines = range / moveinterval;
                int startY = pointInfo.Y - range / 2, startX = pointInfo.X - range / 2;
                int endY = pointInfo.Y + range / 2, endX = pointInfo.X + range / 2;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                Axis ay = config.Axes.Find(v => v.Id == 1);
                MotorsControl.MovePoint2D(100, startX, startY, ax, ay, true);
                //MotorsControl.startCompare();
                //MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 10, endY);
                //MotorsControl.stopCompare();
                if (pointInfo.Remark.Contains("右边沿"))
                {
                    ax = config.Axes.Find(v => v.Id == 1);
                    ay = config.Axes.Find(v => v.Id == 2);
                }
                MotorsControl.setCompareMode(ay.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
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
                    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 30, location1, true);
                    MotorsControl.stopCompare();
                    if (i < flyLines - 1)
                    {
                        if (pointInfo.Remark.Contains("右边沿"))
                        {
                            startY = startY + moveinterval;
                            MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, startY, true);
                        }
                        else
                        {
                            startX = startX + moveinterval;
                            MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, startX, true);
                        }
                    }
                }
            });
            #endregion
        }
        #endregion

        #region 第四步 扫描芯片

        private PointInfo step4TopLeftPoint, step4BottomRightPoint;
        private void CreateChipModel()
        {
            //Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            //Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            //PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
            //MotorsControl.MovePoint2D(100, pointInfo.X, pointInfo.Y, ax, ay, true);
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
                int height = endPoint.Y - startPoint.Y;
                programConfig.DieWidth = width;
                programConfig.DieHeight = height;
                int flyLines = height / flyInterval; // 飞拍的行数
                int startY = startPoint.Y;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                Axis ay = config.Axes.Find(v => v.Id == 1);
                MotorsControl.MovePoint2D(100, startPoint.X, startPoint.Y, ax, ay, true);
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                if (flyLines == 0 && width / flyInterval == 0) //如果X和Y除以间距都是0的话那就在中心拍一张
                {
                    MotorsControl.MovePoint2D(100, startPoint.X + width /2, startPoint.Y + height /2, ax, ay, true);
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                }
                else // 这里飞拍还要判断下XY方向
                {
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
                    case "框选芯片的左上角和右下角":
                        ilChipModel.ShowImg(e);
                        HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, 500, 0);
                        HOperatorSet.SetColor(hswcChipModel.HalconWindow, "red");
                        HOperatorSet.DispObj(hObjectCross, hswcChipModel.HalconWindow);
                        hObjectCross.Dispose();
                        break;
                    case "扫描单个芯片":
                        ilChipModel.ShowImg(e); // 简单模拟是拼图的结果
                        //tasklst.Add(fac.StartNew(obs =>
                        //{
                        //    // 将object转成数组
                        //    ImageArgs imgArg = (ImageArgs)obs;
                        //    //Thread.Sleep(0); // 飞拍模拟拼图
                        //    this.BeginInvoke(new Action<ImageArgs>((val) =>
                        //    {
                        //        ilChipModel.ShowImg(val);
                        //        HOperatorSet.GenCrossContourXld(out HObject hObjectCross, val.Height / 2, val.Width / 2, 500, 0);
                        //        HOperatorSet.SetColor(hswcChipModel.HalconWindow, "red");
                        //        HOperatorSet.DispObj(hObjectCross, hswcChipModel.HalconWindow);
                        //        hObjectCross.Dispose();
                        //    }), e);
                        //    return false;
                        //}, e));
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
                case "确定左上角":
                    step4TopLeftPoint = MotorsControl.GetXYZEncPos(config.Axes.Find(v => v.Remarks == "载盘X轴").Id, config.Axes.Find(v => v.Remarks == "载盘Y轴").Id, config.Axes.Find(v => v.Remarks == "载盘Z轴").Id);
                    tbStep4TopLeftX.Text = step4TopLeftPoint.X.ToString();
                    tbStep4TopLeftY.Text = step4TopLeftPoint.Y.ToString();
                    break;
                case "确定右下角":
                    step4BottomRightPoint = MotorsControl.GetXYZEncPos(config.Axes.Find(v => v.Remarks == "载盘X轴").Id, config.Axes.Find(v => v.Remarks == "载盘Y轴").Id, config.Axes.Find(v => v.Remarks == "载盘Z轴").Id);
                    tbStep4BottomRightX.Text = step4BottomRightPoint.X.ToString();
                    tbStep4BottomRightY.Text = step4BottomRightPoint.Y.ToString();
                    break;
                case "扫描单个芯片":
                    if (step4TopLeftPoint == null || step4BottomRightPoint == null) { DarkMessageBox.ShowWarning("请先框选相应的两个角"); return; }
                    dlvwProgress.SetStartNum(1);
                    _ = ScanOneChipAsync();
                    break;
            }
        }
        #endregion

        #region 第五步 制作图谱

        private void HswcStep5Model_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                ilStep5Model.X = Convert.ToInt32(e.X);
                ilStep5Model.Y = Convert.ToInt32(e.Y);
            }
            catch (Exception er) { }
        }
        /// <summary>
        /// 飞拍
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="dieWidth"></param>
        /// <param name="dieHeight"></param>
        /// <param name="cutRoadWidth"></param>
        /// <param name="fov"></param>
        /// <returns></returns>
        private async Task Step5SearchAsync(PointInfo startPoint, PointInfo endPoint, int dieWidth, int dieHeight, int cutRoadWidth , int fov = 5000)
        {
            //MotorsControl.stopCompare();
            //await Task.Run(() =>
            //{
            //    int width = endPoint.X - startPoint.X;
            //    int height = endPoint.Y - startPoint.Y;
            //    programConfig.DieWidth = width;
            //    programConfig.DieHeight = height;
            //    int flyLines = height / flyInterval; // 飞拍的行数
            //    int startY = startPoint.Y;
            //    Axis ax = config.Axes.Find(v => v.Id == 2);
            //    Axis ay = config.Axes.Find(v => v.Id == 1);
            //    MotorsControl.MovePoint2D(100, startPoint.X, startPoint.Y, ax, ay, true);
            //    MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            //    MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
            //    #region 这里是个小范围的飞拍
            //    for (int i = 0; i < flyLines; i++)
            //    {
            //        int location1 = 1;
            //        if (i % 2 == 0)
            //        {
            //            location1 = endPoint.X;
            //        }
            //        else
            //        {
            //            location1 = startPoint.X;
            //        }
            //        //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
            //        MotorsControl.startCompare();
            //        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 30, location1, true);
            //        MotorsControl.stopCompare();
            //        if (i < flyLines - 1)
            //        {
            //            startY = startY + flyInterval;
            //            MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 30, startY, true);
            //        }
            //    }
            //    #endregion

            //});
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
            programConfig.CutRoadWidth = distance.D;
            dlvwProgress.Next();
            Axis az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
            MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(800); // 等差模式 
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
        }

        private void 画一个模板区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilStep5Model.DrawRectange1();
            dlvwProgress.SetStartNum(2);
        }
        private void Step5_DarkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "扫描整个晶圆":
                    dlvwProgress.SetStartNum(3);
                    break;
            }
        }
        private void MVCameraHelper_CameraImageCallBack_Step5(object sender, ImageArgs e)
        {
            try
            {
#if DEBUG
                Debug.WriteLine(dlvwProgress.NowItem().Text);
#endif
                switch (dlvwProgress.NowItem().Text)
                {
                    case "测量切割道的宽度":
                        ilStep5Model.ShowImg(e);
                        break;
                    case "移动芯片的切割道到图像中心":
                        ilStep5Model.ShowImg(e);
                        double row1 = e.Height / 2 - programConfig.CutRoadWidth / 2;
                        double col1 = e.Width / 2 - 100;
                        double row2 = e.Height / 2 + programConfig.CutRoadWidth / 2;
                        double col2 = e.Width / 2 + 100;
                        HOperatorSet.GenRectangle1(out HObject rect1, row1, col1, row2, col2);
                        row1 = e.Height / 2 - 100;
                        col1 = e.Width / 2 - programConfig.CutRoadWidth / 2;
                        row2 = e.Height / 2 + 100;
                        col2 = e.Width / 2 + programConfig.CutRoadWidth / 2;
                        HOperatorSet.GenRectangle1(out HObject rect2, row1, col1, row2, col2);
                        HOperatorSet.ConcatObj(rect1, rect2, out HObject rect);
                        HOperatorSet.SetColor(hswcStep5Model.HalconWindow, "white");
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
                    case "扫描整个晶圆":
                        // 这里先保存上一步的模板信息
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            Thread.Sleep(20); //模拟匹配切割道十字线
                            this.BeginInvoke(new Action<ImageArgs>((val) =>
                            {
                                ilStep5Model.ShowImg(val);
                            }), e);
                            return false;
                        }, e));
                        break;
                }
            }
            catch (Exception er) { }
        }
        #endregion

        /// <summary>
        /// 上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreviousStep_Click(object sender, EventArgs e)
        {
            darkStepViewer1.PreviousStep();
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
#if DEBUG
                Console.WriteLine("调试模式已开启");
#else
                if (!tsInich6.Checked && !tsInich8.Checked)
                {
                    DarkMessageBox.ShowWarning("请先开启真空阀再进行下一步");
                    return;
                }
#endif
            }
            darkStepViewer1.Complete();
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

        private void darkButton6_Click(object sender, EventArgs e)
        {
            hDevProgramHelper.FindEdge(ilRight.hImage, out HObject obj, out HTuple pointXPulse, out HTuple pointYPulse, 1024, 1024);
            ShowObj(obj, hswcRight);
        }

        private void darkButton7_Click(object sender, EventArgs e)
        {
            hDevProgramHelper.FindEdge(ilTop.hImage, out HObject obj, out HTuple pointXPulse, out HTuple pointYPulse, 1024, 1024);
            ShowObj(obj, hswcTop);
        }

        private void darkButton8_Click(object sender, EventArgs e)
        {
            hDevProgramHelper.FindEdge(ilBottom.hImage, out HObject obj, out HTuple pointXPulse, out HTuple pointYPulse, 1024, 1024);
            ShowObj(obj, hswcBottom);
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
