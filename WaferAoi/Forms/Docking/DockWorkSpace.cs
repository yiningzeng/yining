using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using WaferAoi.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using YiNing.WafermapDisplay.WafermapControl;
using HalconDotNet;
using YiNing.Tools;
using System.IO;

namespace WaferAoi
{
    public partial class DockWorkSpace : DarkDocument
    {
        private MVCameraHelper mVCameraHelper;
        HDevProgramHelper hDevProgramHelper;
        #region Field Region
        private int photoInterval = -1; // 第五步这个飞拍的时候的间距，这个回实时改变
        private int yInterval = -1; //y轴移动的间距，这个回实时改变
        private Axis ax, ay, az, ar;// = FsmHelper.GetConfig().Axes.Find(v => v.Remarks == "载盘X轴");
        private bool goBack = false;
        private bool stop = false;
        private ProgramConfig programConfig;
        private Config config;
        private MainForm main;

        List<PointInfo> pointInfos;
        #endregion

        private List<int> focusPos = new List<int>();

        #region Constructor Region

        public DockWorkSpace()
        {
            InitializeComponent();
            UpdateGetAxes();
            waferMap.OnDieClick += WaferMap_OnDieClick;
            dlvwProgress.Items.Add(new DarkListItem("空闲") { Icon = Icons.进度指向, TextColor = Color.Chocolate });
            for (int waferNum = 0; waferNum < 1; waferNum++)
            {
                // Create sample dataset
                Die[,] data = new Die[41, 40];
                Random binGenerator = new Random(waferNum);
                for (int x = 0; x < 41; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        data[x, y] = new Die() { ColorIndex = binGenerator.Next(8), XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    }
                }
                int a = data.Length;
                waferMap.Dataset = data;
                waferMap.Notchlocation = 90;
                //wmap.MinimumSize = new Size(500, 500);
                waferMap.Dock = DockStyle.Fill;
                //waferMap.SelectX = 21;
                //waferMap.SelectY = 14;
                //waferMap.SelectOneDie = data[21, 14];

                //waferMap.SelectRegionDiagonalDie = new Die[] { new Die() { XIndex = 10, YIndex = 10 }, new Die() { XIndex = 21, YIndex = 23 } };
                //wmap.NoDataString = "没有数据";
                //this.Controls.Add(wmap);
            }
        }
        private void UpdateGetAxes()
        {
            config = FsmHelper.GetConfig();
            ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
            ar = config.Axes.Find(v => v.Remarks == "载盘旋转轴");
        }

        private void WaferMap_OnDieClick(object sender, Die e)
        {
        }

        public void SetProgress(string programName)
        {
            mVCameraHelper.RemoveAllEvent("CameraImageCallBack");
            config = FsmHelper.GetConfig();
            programConfig = JsonHelper.DeserializeByFile<ProgramConfig>(Path.Combine(config.ProgramSavePath, programName + "/config.zyn"));
            pointInfos = programConfig.WaferSize == WaferSize.INCH6 ? config.Inch6SavePoints : config.Inch8SavePoints;
            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("加载图谱"));
            dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
            dlvwProgress.Items.Add(new DarkListItem("定位晶圆"));
            dlvwProgress.Items.Add(new DarkListItem("方向矫正"));
            dlvwProgress.Items.Add(new DarkListItem("检测中"));
            dlvwProgress.Items.Add(new DarkListItem("检测完毕"));
            dlvwProgress.Items.Add(new DarkListItem("数据导出"));
            dlvwProgress.Items.Add(new DarkListItem("取料"));
            dlvwProgress.Items.Add(new DarkListItem("空闲"));
            dlvwProgress.SetStartNum(0);

            if (!File.Exists(programConfig.GetMappingFileName())) {
                DarkMessageBox.ShowError("图谱丢失，请重新制作");
                return;
            }
            Die[,] data = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
            waferMap.Dataset = data;
            dlvwProgress.Next();

            MotorsControl.MovePoint2D(80, ax.StartPoint, ay.StartPoint, ax, ay);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 0);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 0);
            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 0);
        }

        public DockWorkSpace(MainForm main, MVCameraHelper mc, string text, Image icon) : this()
        {
            this.mVCameraHelper = mc;
            this.main = main;
            DockText = text;
            Icon = icon;
            this.Load += DockWorkSpace_Load;
        }

        private void DockWorkSpace_Load(object sender, EventArgs e)
        {
            this.timerCheck.Start();
        }
        #endregion

        #region Event Handler Region
        private void timerCheck_Tick(object sender, EventArgs e)
        {
            int exi = MotorsControl.IoSignalEXI(4);
            this.BeginInvoke(new Action<int>((exiSts) =>
            {
                isEmergencyStop.Checked = (exi & (1 << 0)) == 0;
                isStart.Checked = (exi & (1 << 1)) != 0;
                isReset.Checked = (exi & (1 << 2)) != 0;
                isStop.Checked = (exi & (1 << 3)) != 0;
                isDoor.Checked = (exi & (1 << 4)) != 0;
                isPositivePressure.Checked = (exi & (1 << 7)) != 0;
                isNegativePressure1.Checked = (exi & (1 << 8)) != 0;
                isNegativePressure2.Checked = (exi & (1 << 9)) != 0;
                isNegativePressure3.Checked = (exi & (1 << 10)) != 0;

                //if (!isEmergencyStop.Checked) main.fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_SCRAM);
                //if (!isReset.Checked) main.fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_INIT_RESET);
            }), exi);
        }
        public override void Close()
        {
            var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;

            base.Close();
        }


        #endregion

        private void darkButton1_Click(object sender, EventArgs e)
        {
            dlvwProgress.Next();
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            dlvwProgress.SetStartNum(1);
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            dlvwProgress.Error();
            //waferMap.DieAlpha = 0;
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            dlvwProgress.Stop();
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            Config config = FsmHelper.GetConfig();
            Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "1":
                    MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, 100000, false);
                        break;

                case "2":
                    MotorsControl.MoveJog(ax.Id, ax.JogPrm.Get(), 20);
                    Thread.Sleep(1000);
                    MotorsControl.StopAxis(ax.Id, 0);
                    break;
            }
         }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 对焦FUNC， 一倍镜默认600微米步长调整， 其他的除以倍率
        /// </summary>
        /// <param name="e"></param>
        private async Task StartFocusingAsync()
        {
            await Task.Run(() =>
            {
            //    MotorsControl.stopCompare();
            //    PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, programConfig.BestZPulse, true);
            //    int interval = 600;
            //    MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            //    MotorsControl.setCompareData_Pso(interval); // 等差模式 
            //    int maxH = 4000;
            //    int mixH = -10000;
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, maxH, true);

            //    focusPos.AddRange(MotorsControl.GetFlyPos(maxH, mixH, interval, -1));
            //    MotorsControl.startCompare();
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, mixH, true);
            //    MotorsControl.stopCompare();

            //    //int range = 3000;
            //    //int upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
            //    //int downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;

            //    interval = 100;
            //    MotorsControl.setCompareData_Pso(interval); // 等差模式 
            //    int range = 1000;
            //    int upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
            //    int downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;

            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, upH, true);
            //    focusPos.AddRange(MotorsControl.GetFlyPos(upH, downH, interval, -1));
            //    MotorsControl.startCompare();
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 3, downH, true);
            //    MotorsControl.stopCompare();


            //    interval = 10;
            //    MotorsControl.setCompareData_Pso(interval); // 等差模式
            //    range = 900;
            //    upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
            //    downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, upH, true);
            //    focusPos.AddRange(MotorsControl.GetFlyPos(upH, downH, interval, -1));
            //    MotorsControl.startCompare();
            //    MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 1, downH, true);
            //    MotorsControl.stopCompare();
            //    //MotorsControl.setCompareData_Pso(30); // 等差模式
            //    //range = 150;
            //    //upH = bestZPulse + range > maxH ? maxH : bestZPulse + range;
            //    //downH = bestZPulse - range < mixH ? mixH : bestZPulse - range;
            //    //MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 0.5, upH, true);
            //    //MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 0.5, downH, true);
            //});
            //Task.WaitAll(tasklst.ToArray());
            //this.BeginInvoke(new Action(() =>
            //{
            //    dlvwProgress.Next();
            //    btnFocus.Enabled = true;
            //}));
        }

        private async Task Start()
        {

            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 1);
            if (programConfig.HaveRingPiece == HaveRingPiece.Yes)
            {
                if (programConfig.WaferSize == WaferSize.INCH6) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 1);
                if (programConfig.WaferSize == WaferSize.INCH8) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 1);
            }
            Parallel.Invoke(() => MotorsControl.MovePoint2D(100, programConfig.WaferCenter.X, programConfig.WaferCenter.Y, ax, ay, true),
                ()=>MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, programConfig.BestZPulse, true));

            #region 对焦


            #endregion



        }

        #region 扫描晶圆
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
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, topLeftPoint.X, true);
                    else
                        MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, bottomRightPoint.X, true);
                    yInterval -= Convert.ToInt32(programConfig.CutRoadWidth / 2 + programConfig.DieHeight);
                    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 20, yInterval, true);
                    goBack = !goBack;
                }
                MotorsControl.stopCompare();
            });
        }
        #endregion
    }
}
