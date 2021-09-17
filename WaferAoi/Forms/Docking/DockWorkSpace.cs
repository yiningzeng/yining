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
using HalconDotNet;
using YiNing.Tools;
using System.IO;
using static WaferAoi.Tools.Utils;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace WaferAoi
{
    public partial class DockWorkSpace : DarkDocument
    {
        #region 飞拍模型矫正
        int photoNum = 0;
        bool isNewLine = true; // 表示是否是新行
        int flyCorrectNum = 0;
        int flyInterval = -1;
        int nowLineId = 0;
        int dY = 0; // 每一行走下来y的平局误差
        HTuple flyModelId;
        double halfRow, halfCol;
        #endregion

        #region 对焦参数
        //自动对焦图像回调个数
        private int focusImgCallNum = -1; //从-1开始那是因为focusPos下标识0开始
        private List<int> focusPos = new List<int>();
        private int bestZPulse = 0; // 最好的z的高度
        private double bestZScore = 0; //最好的Z高度分值
        private bool isCoarse = false; // 粗略扫描
        #endregion

        #region 角度
        private ConcurrentQueue<double> RotateRes = new ConcurrentQueue<double>();
        #endregion

        #region 检测
        private ConcurrentQueue<ImageArgs> DetectImageArgs = new ConcurrentQueue<ImageArgs>();
        private HTuple VariationModelID;
        #endregion

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

        InterLayerDraw ilMain, ilRight, ilTop, ilBottom;
        List<Task> tasklst = new List<Task>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(8));


        List<Task> tasklstDetect = new List<Task>();
        TaskFactory facDetect = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(8));


        List<Die> visibleDies = new List<Die>();




        #region Constructor Region

        public DockWorkSpace()
        {
            InitializeComponent();
            hDevProgramHelper = new HDevProgramHelper("圆心查找ver3.1.hdev");
            ilMain = new InterLayerDraw(hswcMain);

            ilRight = new InterLayerDraw();
            ilTop = new InterLayerDraw();
            ilBottom = new InterLayerDraw();

            hswcMain.MouseWheel += hswcMain.HSmartWindowControl_MouseWheel;

            //hswcMain.HMouseMove += HswcChipModel_HMouseMove;
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
                waferMap.Notchlocation = 0;
                //wmap.MinimumSize = new Size(500, 500);
                waferMap.Dock = DockStyle.Fill;
                //waferMap.SelectX = 21;
                //waferMap.SelectY = 14;YPluse
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

        public void SetProgress(string programName, string waferId)
        {
            mVCameraHelper.RemoveAllEvent("CameraImageCallBack");
            mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
            config = FsmHelper.GetConfig();
            programConfig = JsonHelper.DeserializeByFile<ProgramConfig>(Path.Combine(config.ProgramSavePath, programName + "/config.zyn"));
            programConfig.Id = waferId;
            programConfig.ExportPath = Path.Combine(config.Export, DateTime.Now.ToString("yyyy-MM-dd") + "/" + programConfig.Id);
            pointInfos = programConfig.WaferSize == WaferSize.INCH6 ? config.Inch6SavePoints : config.Inch8SavePoints;
            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("加载图谱"));
            dlvwProgress.Items.Add(new DarkListItem("放置晶圆"));
            dlvwProgress.Items.Add(new DarkListItem("自动对焦"));
            dlvwProgress.Items.Add(new DarkListItem("角度校正"));
            dlvwProgress.Items.Add(new DarkListItem("寻找右边沿"));
            dlvwProgress.Items.Add(new DarkListItem("寻找上边沿"));
            dlvwProgress.Items.Add(new DarkListItem("寻找下边沿"));
            dlvwProgress.Items.Add(new DarkListItem("定位晶圆"));
            dlvwProgress.Items.Add(new DarkListItem("检测中"));
            dlvwProgress.Items.Add(new DarkListItem("检测完毕"));
            dlvwProgress.Items.Add(new DarkListItem("数据导出"));
            dlvwProgress.Items.Add(new DarkListItem("空闲"));
            dlvwProgress.SetStartNum(0);

            if (!File.Exists(programConfig.GetMappingFileName())) {
                DarkMessageBox.ShowError("图谱丢失，请重新制作");
                return;
            }
            Directory.CreateDirectory(programConfig.ExportPath);
            Die[,] data = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
            waferMap.VisibleDatasetSavePath = programConfig.GetVisibleMappingFileName();
            waferMap.Dataset = data;
            waferMap.DieAlpha = 150;
            waferMap.Colors = new Color[] {Color.DimGray, Color.Blue, Color.Red };
            //waferMap.Interactive = true;
            //waferMap.SelectOneDie = data[30, 30];
            if (waferMap.VisibleDataset.Count > 0) waferMap.SelectOneDie = waferMap.VisibleDataset[0];
            dlvwProgress.Next();
            
            MotorsControl.MovePoint2D(150, ax.StartPoint, ay.StartPoint, ax, ay);
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
                    default:
                        ilMain.ShowImg(e);
                        break;
                    case "自动对焦":
                        #region 自动对焦
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
                            }
                            this.BeginInvoke(new Action<HObject>(v =>
                            {
                                ilMain.ShowImg(v);
                            }), imgArg.ImageHobject);
                            return true;
                        }, param));
                        #endregion
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
                                ilMain.ShowImg(v);
                            }), imgArg);
                            return true;
                        }, e));
                        //ilFocus.ShowImg(e);
                        break;

                    case "寻找右边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\右.bmp");
                            bool res = hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90);
                            PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                            JsonHelper.Serialize(pf, "D:/右.json");
                            Debug.WriteLine("右边沿，占比: " + ratio);
                            if (res)
                            {
                                ilRight.hImage = imgArg.ImageHobject;
                                ilRight.XPulse = imgArg.XPulse;
                                ilRight.YPulse = imgArg.YPulse;
                                ilRight.AllCols = imgArg.Width;
                                ilRight.AllRows = imgArg.Height;

                                //if (!FindEdgeAndShow("edgepoint_right", ilRight, hswcMain)) { DarkMessageBox.ShowError("未找到边沿");};
                                //alalreadyEdge = true;
                                //MotorsControl.stopCompare();
                                //MotorsControl.StopAxis(ax.Id, 1);
                                //MotorsControl.StopAxis(ay.Id, 1);
                                //this.BeginInvoke(new Action(() =>
                                //{
                                //    dlvwProgress.Stop();
                                //}));
                                //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);

                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilRight.ShowImg(e);
                                //}), pf);
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return true;
                        }, e));
                        break;
                    case "寻找上边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                            if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\上.bmp");
                            JsonHelper.Serialize(pf, "D:/上.json");
                            bool res = hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90);
                            Debug.WriteLine("找到上边沿，占比: " + ratio);
                            //if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\上边沿\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                            if (res)
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
                           
                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilTop.ShowImg(e);
                                //}), pf); 
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return true;
                        }, e));
                        break;
                    case "寻找下边沿":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            PointInfo pf = new PointInfo() { X = imgArg.XPulse, Y = imgArg.YPulse };
                            if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\下.bmp");
                            JsonHelper.Serialize(pf, "D:/下.json");
                            bool res = hDevProgramHelper.CheckEdgeRatio(imgArg.ImageHobject, out HTuple ratio, 20, 10, 90);
                            Debug.WriteLine("找到下边沿，占比: " + ratio);
                            //if (imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "bmp", 0, @"D:\下边沿\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                            if (res)
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
                        
                                //this.BeginInvoke(new Action<PointInfo>((p) =>
                                //{
                                //    ilBottom.ShowImg(e);
                                //}), pf);
                            }
                            else
                            {
                                imgArg.Dispose();
                            }
                            return true;
                        }, e));
                        break;
                    case "检测中":
                        tasklst.Add(fac.StartNew(obs =>
                        {
                            // 将object转成数组
                            ImageArgs imgArg = (ImageArgs)obs;
                            GC.Collect();
                            HOperatorSet.Rgb1ToGray(imgArg.ImageHobject, out HObject grayImage);
                            //flyCorrectNum = 5;
                            flyCorrectNum = programConfig.ChipModelPar.CorrectInterval <= 1 ? programConfig.ChipModelPar.CorrectInterval : flyCorrectNum;
                            if (flyCorrectNum == programConfig.ChipModelPar.CorrectInterval)
                            {
                                HOperatorSet.FindShapeModel(grayImage, flyModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent,
                                                             programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                                if (Row.Length > 0 && Col.Length > 0)
                                {
                                    flyCorrectNum = 0;
                                    isNewLine = false;
                                    //如果模板结果中心在图像左边就+，右边就减
                                    double offsetX = Col.D - imgArg.Width / 2;
                                    double offsetXPulse = Math.Abs(ProgramConfig.GetXPulseByPixel(Convert.ToInt32(offsetX), config.ActualPixelLenght));

                                    double offsetY = Row.D - imgArg.Height / 2;
                                    double offsetYPulse = Math.Abs(ProgramConfig.GetXPulseByPixel(Convert.ToInt32(offsetY), config.ActualPixelLenght));
                                    int tempFlyInterval = -1;
                                    //if (nowLineId % 2 == 0) offsetXPulse =  - offsetXPulse;
                                    double dx = 1000;
                                    if (offsetX > 0)
                                    {
                                        if (nowLineId % 2 == 0) tempFlyInterval = Convert.ToInt32(flyInterval + offsetXPulse + dx);
                                        else
                                        tempFlyInterval = Convert.ToInt32(flyInterval - offsetXPulse - dx);
                                    }
                                    else
                                    {
                                        if (nowLineId % 2 == 0) tempFlyInterval = Convert.ToInt32(flyInterval - offsetXPulse - dx);
                                        else
                                            tempFlyInterval = Convert.ToInt32(flyInterval + offsetXPulse + dx);
                                    }

                                    if (offsetY > 0)
                                    {
                                        dY = Convert.ToInt32(offsetYPulse);
                                    }
                                    else
                                    {
                                        dY = -Convert.ToInt32(offsetYPulse);
                                    }
                                    MotorsControl.setCompareData_Pso(tempFlyInterval);
                                }
                            }
                            flyCorrectNum++;
                            grayImage.Dispose();
                            //HOperatorSet.WriteImage(imgArg.ImageHobject, "jpg", 0, Path.Combine(programConfig.ExportPath, "img/" + imgArg.XPulse + imgArg.YPulse + ".jpg"));
                            imgArg.XId = 1;
                            imgArg.YId = 2;
                            DetectImageArgs.Enqueue(imgArg);
                            this.BeginInvoke(new Action<ImageArgs>(v =>
                            {
                                ilMain.ShowImg(v, false, false);
                            }), imgArg);
                            return true;
                        }, e));
                        photoNum++;
                        #region 开始飞拍矫正
                                    #endregion
                        //HOperatorSet.WriteImage(e.ImageHobject, "jpg", 0, Path.Combine(programConfig.ExportPath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg"));
                        //ilMain.ShowImg(e);
                        break;
                }
            }
            catch (Exception er)
            {

            }
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
            dlvwProgress.SetStartNum(2);
            Directory.CreateDirectory(programConfig.ExportPath);
            Directory.CreateDirectory(programConfig.ExportPath + "/img");
            _ = Start();
        }
        private void btnContinue_Click(object sender, EventArgs e)
        {
            int a = 1;
            if (a==0)
            {
                config = FsmHelper.GetConfig();
                programConfig = JsonHelper.DeserializeByFile<ProgramConfig>(programConfig.GetThisFileName());
                pointInfos = programConfig.WaferSize == WaferSize.INCH6 ? config.Inch6SavePoints : config.Inch8SavePoints;
                dlvwProgress.SetStartNum(4);
                _ = FindWaferCenter();
            }
            else
            {
                _ = ScanWafer();
            }
        }
        private void darkButton5_Click_1(object sender, EventArgs e)
        {
            int aa =programConfig.ChipModelPar.CorrectInterval;
            _ = sss();
            //bool aa = timerDetect.Enabled;

            ////timerDetect.Start();
            //timerDetect.Enabled = true;
            ////var cts = new CancellationTokenSource();
            //Task.WaitAll(tasklstDetect.ToArray());
            MotorsControl.StopAxis(ax.Id, 0);
            stop = true;
            waferMap.Dataset[8,40].ColorIndex = 1;
            waferMap.ReFresh();
            //visibleDies = JsonHelper.DeserializeByFile<List<Die>>(programConfig.GetVisibleMappingFileName());
            //if (visibleDies.Count > 0)
            //{
            //    waferMap.SelectX = visibleDies[10].XIndex;
            //    waferMap.SelectY = visibleDies[10].YIndex;
            //}
        }
        private async Task sss()
        {
            await Task.WhenAll(tasklstDetect);
            Task.WaitAll(tasklstDetect.ToArray());
            await Task.Factory.ContinueWhenAll(tasklstDetect.ToArray(), (m) => { Debug.WriteLine("asdsdsd"); });
            int a = 0;
        }


        private async Task Start()
        {

            MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 1);
            if (programConfig.HaveRingPiece == HaveRingPiece.Yes)
            {
                if (programConfig.WaferSize == WaferSize.INCH6) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, 1);
                if (programConfig.WaferSize == WaferSize.INCH8) MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, 1);
            }
            Thread.Sleep(500);
            PointInfo pointInfo = pointInfos.Find(i => i.Remark == "调焦点位");
            Parallel.Invoke(() => MotorsControl.MovePoint2D(150, pointInfo.X, pointInfo.Y, ax, ay, true),
                ()=>MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, programConfig.BestZPulse, true));

            #region 对焦

            await StartFocusingAsync();
            MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, focusPos[focusImgCallNum], true);
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
            await Rotate();
            await FindWaferCenter();

            if (dlvwProgress.Status() == DarkProgressReminder._Status.ERROR) return;
            await ScanWafer();
            Task.WaitAll(tasklstDetect.ToArray());
            tasklstDetect.Clear();
            //this.BeginInvoke(new Action(() =>
            //{
            //    dlvwProgress.Next();
            //    dlvwProgress.Next();
            //    dlvwProgress.Next();
            //    dlvwProgress.Next();
            //}));
            #endregion
        }


        /// <summary>
        /// 对焦FUNC， 一倍镜默认500微米步长调整， 其他的除以倍率
        /// </summary>
        /// <param name="e"></param>
        private async Task StartFocusingAsync()
        {
            isCoarse = false;
            focusImgCallNum = -1;
            focusPos.Clear();
            await Task.Run(() => {
                MotorsControl.stopCompare();


                int interval = 500 / programConfig.GetLenseMag();
                double vel = 5 / programConfig.GetLenseMag();
                int distance = 5000 / programConfig.GetLenseMag();

                isCoarse = false;
                int maxH = 4000;
                int mixH = -10000;

                int now = programConfig.BestZPulse;
                int up = now + distance > maxH ? maxH : now + distance;
                int down = now - distance < mixH ? mixH : now - distance;

                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), 10, up, true);
                now = up;

                MotorsControl.setCompareMode(az.Id, az.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(interval); // 等差模式 

                focusPos.Add(now);
                while (now >= down)
                {
                    now -= interval;
                    focusPos.Add(now);
                }
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(az.Id, az.TrapPrm.Get(), vel, down, true);
                MotorsControl.stopCompare();
            });
            Task.WaitAll(tasklst.ToArray());
            tasklst.Clear();
            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
            }));
        }

        int detectNum = 0;

        private void Detect(object obs)
        {
            // 将object转成数组
            object[] objArr = (object[])obs;
            ImageArgs imgArg = (ImageArgs)objArr[0];
            int id = (int)objArr[1];

            Stopwatch sw = new Stopwatch();
            sw.Restart();
            HOperatorSet.Rgb1ToGray(imgArg.ImageHobject, out HObject grayImage);
            Debug.WriteLine("灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
            sw.Restart();
            HOperatorSet.FindShapeModel(grayImage, flyModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent, programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
            Debug.WriteLine("FindShapeModel-耗时(ms):" + sw.ElapsedMilliseconds);
            if (Score1.Length > 0)
            {
                sw.Restart();
                ImageTrans(grayImage, out HObject fu, Row, Col, Angle1, programConfig.ChipModelPar.ShapeModelCenterRow, programConfig.ChipModelPar.ShapeModelCenterCol, 0, out HTuple HomMat2D);
                grayImage.Dispose();
                Debug.WriteLine("ImageTrans-耗时(ms):" + sw.ElapsedMilliseconds);
                HOperatorSet.CropRectangle1(fu, out HObject detectImg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
                fu.Dispose();

                //HOperatorSet.WriteImage(trainImg, "jpg", 0, "D:/fu.jpg");
                try
                {//compare_variation_model
                    sw.Restart();
                    HOperatorSet.CompareVariationModel(detectImg, out HObject region, VariationModelID);

                    Debug.WriteLine("CompareVariationModel-耗时(ms):" + sw.ElapsedMilliseconds);
                    sw.Restart();
                    HOperatorSet.AffineTransImage(imgArg.ImageHobject, out HObject finalimg, HomMat2D, "constant", "false");
                    Debug.WriteLine("AffineTransImage-耗时(ms):" + sw.ElapsedMilliseconds);
                    sw.Restart();
                    HOperatorSet.CropRectangle1(finalimg, out HObject finalCropimg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
                    Debug.WriteLine("CropRectangle1-耗时(ms):" + sw.ElapsedMilliseconds);
                    //                                connection(Region, ConnectedRegions)
                    //select_shape(ConnectedRegions, SelectedRegions, 'area', 'and', area_min, area_max)

                    HOperatorSet.Connection(region, out HObject ConnectedRegions);

                    HOperatorSet.SelectShape(ConnectedRegions, out HObject selectRegions, "area", "and", programConfig.ChipDetectPar.AreaMin, programConfig.ChipDetectPar.AreaMax);

                    if (id < visibleDies.Count) {
                        try
                        {
                            string name = visibleDies[id].XIndex + "-" + visibleDies[id].YIndex;
                            visibleDies[id].ColorIndex = 2;
                            visibleDies[id].Res = Path.Combine(programConfig.ExportPath, "img/" + name);
                            HOperatorSet.WriteImage(finalCropimg, "jpg", 0, Path.Combine(programConfig.ExportPath, "img/" + name + ".jpg"));
                            HOperatorSet.WriteRegion(selectRegions, Path.Combine(programConfig.ExportPath, "img/" + name + ".hobj"));
                        }
                        catch (Exception er) { }
                    }
                    detectImg.Dispose();
                    region.Dispose();
                    finalimg.Dispose();
                    finalCropimg.Dispose();
                    ConnectedRegions.Dispose();
                    selectRegions.Dispose();
                }
                catch (HalconException er) { }
            }
            imgArg.Dispose();
            try
            {
                this.BeginInvoke(new Action<int>((i) =>
                {
                    try
                    {
                        waferMap.Dataset[visibleDies[i].YIndex, visibleDies[i].XIndex].ColorIndex = 1;
                        waferMap.ReFresh();
                    }
                    catch (Exception er) { }

                }), id);
            }
            catch { }
        }
        private void timerDetect_Tick(object sender, EventArgs e)
        {
            if (DetectImageArgs.TryDequeue(out ImageArgs imageArgs))
            {
                detectNum++;
                object[] objectArray = new object[2];//这里的2就是改成你要传递几个参数
                objectArray[0] = imageArgs;
                objectArray[1] = detectNum;
                object param = (object)objectArray;
                tasklstDetect.Add(facDetect.StartNew(Detect, param));
                if (detectNum >= photoNum && detectNum > 3700)
                {
                    //timerDetect.Enabled = false;
                    //timerDetect.Stop();
                    JsonHelper.Serialize(visibleDies, programConfig.GetVisibleMappingFileName());
                    MotorsControl.MovePoint2D(150, ax.StartPoint, ay.StartPoint, ax, ay);
                    this.BeginInvoke(new Action(() =>
                    {
                        dlvwProgress.SetStartNum(11);
                        new DialogDetectResult(programConfig.GetMappingFileName(), programConfig.GetVisibleMappingFileName()).ShowDialog();
                    }));
                }
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

        /// <summary>
        /// 角度矫正
        /// </summary>
        /// <returns></returns>
        private async Task Rotate()
        {
            await Task.Run(() => {

                HOperatorSet.GenEmptyObj(out HObject imageRotate);
                hDevProgramHelper.ChipDeg(ilMain.hImage, out imageRotate, out HTuple OrientationAngle, out HTuple Degree);
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

                PointInfo startRotatePoint = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
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
            tasklst.Clear();
            List<double> rotateVal = new List<double>();
            while (RotateRes.TryDequeue(out double val))
            {
                rotateVal.Add(val);
            }
            double sum = 0;
            rotateVal.Sort((x, y) => y.CompareTo(x));
            for (int i = 3; i < rotateVal.Count - 3; i++)
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
        /// 定位晶圆
        /// </summary>
        /// <returns></returns>
        private async Task FindWaferCenter()
        {
            PointInfo p = pointInfos.Find(i => i.Remark == "右边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            tasklst.Clear();
            if (!FindEdgeAndShow("edgepoint_right", ilRight, hswcMain)) { DarkMessageBox.ShowError("未找到右边沿"); return; };

            //await SearchAsync(pointInfos.Find(i => i.Remark == "上边沿搜索中心点"), int.Parse(tbRangeTop.Text));
            p = pointInfos.Find(i => i.Remark == "上边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            tasklst.Clear();
            if (!FindEdgeAndShow("edgepoint", ilTop, hswcMain)) { DarkMessageBox.ShowError("未找到上边沿"); return; };

            p = pointInfos.Find(i => i.Remark == "下边沿搜索中心点");
            await TestMove(p.X, p.Y);
            tasklst.Add(fac.StartNew(() =>
            {
                Thread.Sleep(500);
                return false;
            }));
            Task.WaitAll(tasklst.ToArray());
            tasklst.Clear();
            if (!FindEdgeAndShow("edgepoint_down", ilBottom, hswcMain)) { DarkMessageBox.ShowError("未找到下边沿"); return; };

            #region 计算圆心
            hDevProgramHelper.FindCenter(ilTop.Point_X, ilTop.Point_Y, ilBottom.Point_X, ilBottom.Point_Y, ilRight.Point_X, ilRight.Point_Y, out HTuple centerX, out HTuple centerY, out HTuple radius);
            programConfig.WaferCenter = new Point(Convert.ToInt32(centerX.D), Convert.ToInt32(centerY.D));
            programConfig.WaferRadius = radius.D;
            #endregion
            JsonHelper.Serialize(programConfig, programConfig.GetExportFileName());
            #region 移动到圆心
            MotorsControl.MovePoint2D(100, programConfig.WaferCenter.X, programConfig.WaferCenter.Y, ax, ay, true);
            #endregion
            MotorsControl.startCompare();
            MotorsControl.stopCompare();

            this.BeginInvoke(new Action(() =>
            {
                dlvwProgress.Next();
            }));
        }

        private async Task TestMove(double xPos, double yPos)
        {
            MotorsControl.stopCompare();
            await Task.Run(() => {
                MotorsControl.MovePoint2D(100, Convert.ToInt32(xPos), Convert.ToInt32(yPos), ax, ay, true);

                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(800); // 等差模式 
                MotorsControl.startCompare();
                MotorsControl.stopCompare();
            });
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
                //HOperatorSet.WriteImage(il.hImage, "jpg", 0, "D:/aaaaaaaaaaaaaaaaaaaa.jpg");
                hDevProgramHelper.FindEdge(funcName, il.hImage, out obj, out HTuple pointX, out HTuple pointY, il.XPulse, il.YPulse, il.AllCols * config.ActualPixelLenght / programConfig.GetLenseMag(), il.AllRows * config.ActualPixelLenght / programConfig.GetLenseMag(), programConfig.GetLenseMag(), config.ActualPixelLenght, 0.4);
                //HOperatorSet.DispObj(obj, hswc.HalconWindow);
                if (obj.IsInitialized())
                {

                    il.Point_X = pointX;
                    il.Point_Y = pointY;

                    if (pointX.Length > 0 && pointY.Length >0)
                    {
                        this.BeginInvoke(new Action<HObject, HObject>((a, b) => {
                            dlvwProgress.Next();
                            ilMain.ShowImg(a, b, false);
                            b.Dispose();
                        }), il.hImage, obj);
                        JsonHelper.Serialize(new PointInfo() { X = Convert.ToInt32(il.Point_X.D), Y = Convert.ToInt32(il.Point_Y.D) }, "D:/" + funcName + ".json");

                        return true;
                    }
                    else
                    {
                        this.BeginInvoke(new Action<HObject, HObject>((a, b) => {
                            dlvwProgress.Error();
                            ilMain.ShowImg(a, b, false);
                            b.Dispose();
                        }), il.hImage, obj);
                        return false;
                    }

                }
                else
                {
                    this.BeginInvoke(new Action(() =>
                    {
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


        public class FlyPoint
        {
            public int StartX { get; set; }
            public int EndX { get; set; }
            public int Y { get; set; }

            public FlyPoint(double sX, double eX, double y)
            {
                StartX = Convert.ToInt32(sX);
                EndX = Convert.ToInt32(eX);
                Y = Convert.ToInt32(y);
            }
            public FlyPoint(int sX, int eX, int y)
            {
                StartX = sX;
                EndX = eX;
                Y = y;
            }
        }
        /// <summary>
        /// 扫描晶圆
        /// </summary>
        /// <returns></returns>
        private async Task ScanWafer()
        {
            timerDetect.Start();
            isNewLine = true;
            flyCorrectNum = 0;
            photoNum = 0;
            detectNum = 0;
            flyInterval = Convert.ToInt32(programConfig.DieWidth + programConfig.CutRoadWidth);

            Die[,] data = JsonHelper.DeserializeByFile<Die[,]>(programConfig.GetMappingFileName());
            waferMap.VisibleDatasetSavePath = programConfig.GetVisibleMappingFileName();
            waferMap.Dataset = data;
            waferMap.DieAlpha = 150;
            waferMap.Colors = new Color[] { Color.DimGray, Color.Blue };

            HOperatorSet.ReadShapeModel(programConfig.GetFlyModelFileName(), out flyModelId);
            HOperatorSet.ReadRegion(out HObject region, programConfig.GetChipModelRegionFileName());
            HOperatorSet.SmallestRectangle1(region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
            HOperatorSet.ReadVariationModel(programConfig.GetChipVariationModelFileName(), out VariationModelID);
            HOperatorSet.PrepareVariationModel(VariationModelID, programConfig.ChipDetectPar.AbsThreshold, programConfig.ChipDetectPar.VarThreshold);



            halfRow = (row2.D - row1.D) / 2;
            halfCol = (col2.D - col1.D) / 2;
            visibleDies.Clear();
            visibleDies = JsonHelper.DeserializeByFile<List<Die>>(programConfig.GetVisibleMappingFileName());
            //if (visibleDies.Count > 0) waferMap.SelectOneDie = visibleDies[0];
            stop = false;
            await Task.Run(() =>
            {
                int upNum = 31;
                int downNum = 1;
                int leftNum = 31;
                int rightNum = 30;
                double oneStepX = programConfig.DieWidth + programConfig.CutRoadWidth;
                double oneStepY = programConfig.DieHeight + programConfig.CutRoadWidth;
                List<FlyPoint> flyLines = new List<FlyPoint>();

                while (upNum >= 0)
                {
                    double dy = upNum * oneStepY;
                    double lenghtStartR = Math.Sqrt(Math.Abs(programConfig.WaferRadius * programConfig.WaferRadius - dy * dy));
                    double startX = programConfig.WaferCenter.X - lenghtStartR;
                    double finalStartX = programConfig.WaferCenter.X;
                    while (finalStartX >= startX)
                    {
                        finalStartX -= oneStepX;
                    }
                    double endX = programConfig.WaferCenter.X + lenghtStartR;
                    double finalEndX = programConfig.WaferCenter.X;
                    while (finalEndX <= endX)
                    {
                        finalEndX += oneStepX;
                    }
                    double y = programConfig.WaferCenter.Y + dy;
                    flyLines.Add(new FlyPoint(finalStartX, finalEndX, y));
                    upNum--;
                }
                while (downNum <= 31)
                {
                    double dy = downNum * oneStepY;
                    double lenghtStartR = Math.Sqrt(Math.Abs(programConfig.WaferRadius * programConfig.WaferRadius - dy * dy));
                    double startX = programConfig.WaferCenter.X - lenghtStartR;
                    double finalStartX = programConfig.WaferCenter.X;
                    while (finalStartX >= startX)
                    {
                        finalStartX -= oneStepX;
                    }

                    double endX = programConfig.WaferCenter.X + lenghtStartR;
                    double finalEndX = programConfig.WaferCenter.X;
                    while (finalEndX <= endX)
                    {
                        finalEndX += oneStepX;
                    }
                    double y = programConfig.WaferCenter.Y - dy;
                    flyLines.Add(new FlyPoint(finalStartX, finalEndX, y));
                    downNum++;
                }
                JsonHelper.Serialize(flyLines, Path.Combine(programConfig.ExportPath, "flyLines.lhh"));

                if (flyLines.Count > 0) MotorsControl.MovePoint2D(100, flyLines[0].StartX, flyLines[0].Y, ax, ay, true);

                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式 


                double velX = 30;
                int dY = 300;

                for (int i = 0; i < flyLines.Count; i++)
                {
                    velX = 20;
                    MotorsControl.startCompare();
                    nowLineId = i;
                    if (i > 31) {
                        //dY = -dY;
                    }
                    if (i % 2 == 0)
                    {
                        Parallel.Invoke(() => MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), velX, flyLines[i].EndX, true)
                                        /*() => MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), velY, flyLines[i].Y + dY, true)*/);
                    }
                    else
                    {
                
                        Parallel.Invoke(() => MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), velX, flyLines[i].StartX, true)
                            /*() => MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), velY, flyLines[i].Y - dY, true)*/);
                    }
                    MotorsControl.stopCompare();
                    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 30, Convert.ToInt32(flyLines[i].Y + dY ), true);
                    isNewLine = true;
                    flyCorrectNum = programConfig.ChipModelPar.CorrectInterval;
                    if (stop) break;
                }
            });
           
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
