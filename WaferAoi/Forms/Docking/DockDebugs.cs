using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using YiNing.Tools;
using System;
using WaferAoi.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using HalconDotNet;
using System.Threading.Tasks;
using YiNing.WafermapDisplay.WafermapControl;

namespace WaferAoi
{
    public partial class DockDebugs : DarkDocument
    {
        #region Constructor Region
        InterLayerDraw interLayerDraw;
        MVCameraHelper mVCameraHelper;
        Config config;
        public DockDebugs()
        {
            InitializeComponent();
            HookEvents();
            Ini();
        }

        public DockDebugs(string text, Image icon, ref MVCameraHelper mc)
            : this()
        {
            DockText = text;
            Icon = icon;
            mVCameraHelper = mc;
        }

        #endregion

        #region Method Region
        private void HookEvents()// 写活了
        {
            //运动控制
            ControlHelper.SetButtonMouseDown(new Control[] { dsepMove }, Button_MouseDown);
            ControlHelper.SetButtonMouseUp(new Control[] { dsepMove }, Button_MouseUp);
            //注册所有darkbutton
            ControlHelper.SetDarkButtonClick(new Control[] { this }, DarkButton_Click);
        }
        private void Ini()
        {
            hSmartWindowControl1.HMouseMove += HSmartWindowControl1_HMouseMove;
            //mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
            interLayerDraw = new InterLayerDraw(hSmartWindowControl1);
                        config = JsonHelper.DeserializeByFile<Config>("yining.config");
            if (config == null) { DarkMessageBox.ShowError("未找到运动控制的相关配置", "错误提醒"); }
            else
            {
                foreach (var axis in config.Axes)
                {
                    var item = new DarkListItem(axis.Remarks);
                    item.Icon = Icons.x轴;
                    item.Tag = axis;
                    dlvwAxes.Items.Add(item);
                }
                dlvwAxes.SelectItem(0);
            }
            List<DarkStepViewerItem> list = new List<DarkStepViewerItem>();
            //list.Add(new DarkStepViewerItem("1", "步骤1", 1, "这里是该步骤的描述信息", null));

            //list.Add(new DarkStepViewerItem("2", "步骤2", 2, "这里是该步骤的描述信息", null));
            //list.Add(new DarkStepViewerItem("3", "步骤步骤23", 3, "这里是该步骤的描述信息", null));
            //list.Add(new DarkStepViewerItem("2", "步骤4", 4, "这里是该步骤的描述信息", null));

            this.darkStepViewer1.CurrentStep = 2;
            this.darkStepViewer1.ListDataSource = list;
        }

        private void HSmartWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            lbX.Text = e.X.ToString();
            lbY.Text = e.Y.ToString();
        }


        private Axis GetSelectAxis()
        {
            try
            {
                if (dlvwAxes.SelectedIndices.Count > 0)
                {
                    Axis axis = dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag as Axis;
                    return axis;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception er) { return null; }
        }
        #endregion

        #region Event Handler Region
        private void MVCameraHelper_CameraImageCallBack(object sender, ImageArgs e)
        {
            //HOperatorSet.GetImageSize(e.ImageHobject, out HTuple Iwidth, out HTuple Iheight);
            HOperatorSet.SetPart(hSmartWindowControl1.HalconWindow, 0, 0, e.Height - 1, e.Width - 1);
            HOperatorSet.ClearWindow(hSmartWindowControl1.HalconWindow);
            HOperatorSet.DispObj(e.ImageHobject, hSmartWindowControl1.HalconWindow);
            hSmartWindowControl1.SetFullImagePart();

            HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, e.Width, 0);
            HOperatorSet.DispObj(hObjectCross, hSmartWindowControl1.HalconWindow);

            //new HDevelopExport();


            e.ImageHobject.Dispose();
        }
        /// <summary>
        /// 运动控制停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1; break;//"晶圆载具Y轴状态";
                case "向右": axisId = 2; direction = -1; break;//晶圆载具Y轴状态";
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
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = 1; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = -1; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = -1; break;//"晶圆载具Y轴状态";
                case "向右": axisId = 2; direction = 1; break;//晶圆载具Y轴状态";
                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
                MotorsControl.MoveJog(axis.Id, axis.JogPrm.Get(), axis.JogPrm.Vel, direction);
        }
        private void DarkButton_Click(object sender, EventArgs e)
        {
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            Axis axis;
            switch (type)
            {
                case "设置飞拍":
                    axis = GetSelectAxis();
                    MotorsControl.setCompareMode(axis.Id, axis.Id);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(int.Parse(dtebInterval.Text)); // 等差模式
                    MotorsControl.startCompare();
                    break;
                case "跑到保存点1":
                    axis = GetSelectAxis();
                    MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), axis.TrapPrm.Vel, axis.StartPoint);
                    MotorsControl.setCompareMode(axis.Id, axis.Id);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(int.Parse(dtebInterval.Text)); // 等差模式
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case "跑到保存点2":
                    axis = GetSelectAxis();
                    MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), axis.TrapPrm.Vel, axis.EndPoint);
                    MotorsControl.setCompareMode(axis.Id, axis.Id);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(int.Parse(dtebInterval.Text)); // 等差模式
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case "上一步骤":
                    darkStepViewer1.PreviousStep();
                    break;
                case "下一步骤":
                    darkStepViewer1.Complete();
                    break;
                case "读取mapping图":
                    //var aa =WaferMappingHelper.GetLatestFiles(@"D:\WaferDataIn\mapping");
                    break;

                case "点1":
                    MovePoint(tbVel.Text, tb1x.Text, tb1y.Text);
                    break;
                case "点2":
                    MovePoint(tbVel.Text, tb2x.Text, tb2y.Text);
                    break;
                case "点3":
                    MovePoint(tbVel.Text, tb3x.Text, tb3y.Text);
                    break;
                case "点4":
                    MovePoint(tbVel.Text, tb4x.Text, tb4y.Text);
                    break;
                case "点5":
                    MovePoint(tbVel.Text, tb5x.Text, tb5y.Text);
                    break;
                case "点6":
                    MovePoint(tbVel.Text, tb6x.Text, tb6y.Text);
                    break;
                case "拍照比对":
                    MotorsControl.setCompareMode(1, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                    MotorsControl.setCompareData_Pso(1000); // 等差模式
                    MotorsControl.startCompare();
                    MotorsControl.stopCompare();
                    break;
                case "相机曝光":
                    mVCameraHelper.CameraSetExposureTime(double.Parse(dtbExposeTime.Text));
                    break;

                case "计算点1 点2 点3的圆心":
                    Point point = Utils.FindCenter(new Point(Convert.ToInt32(tb1x.Text), Convert.ToInt32(tb1y.Text)),
                        new Point(Convert.ToInt32(tb2x.Text), Convert.ToInt32(tb2y.Text)),
                        new Point(Convert.ToInt32(tb3x.Text), Convert.ToInt32(tb3y.Text))
                        );
                    tbWaferCenterX.Text = point.X.ToString();
                    tbWaferCenterY.Text = point.Y.ToString();
                    break;
                case "计算等边三角形":
                    try
                    {
                        int rightX = int.Parse(tbTopPointX.Text);
                        int rightY = int.Parse(tbTopPointY.Text);
                        int radius = int.Parse(tbRd.Text) / 2;
                        Point[] points = getSanjiaoxingOtherPoint(new Point(rightX, rightY), radius);
                        tb1x.Text = points[0].X.ToString();
                        tb1y.Text = points[0].Y.ToString();

                        tb2x.Text = points[1].X.ToString();
                        tb2y.Text = points[1].Y.ToString();

                        tb3x.Text = points[2].X.ToString();
                        tb3y.Text = points[2].Y.ToString();
                        //if (config.Inch8 == null)
                        //{
                        //    Inch inch8 = new Inch() { Size = 8, RightPoint = new Point(rightX, rightY), Diameter = int.Parse(tbRd.Text) };
                        //    config.Inch8 = inch8;
                        //    JsonHelper.Serialize(config, "yining.config");
                        //}
                    }
                    catch (Exception er) { }

                    break;
                case "相机关":
                    mVCameraHelper.CloseCameras();
                    break;
                case "相机开":
                    mVCameraHelper.OpenCameras();
                    break;
                case "生成图谱":
                    Die[,] data = new Die[64, 65];

                    for (int x = 0; x < 64; x++)
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
                    JsonHelper.Serialize(data, "D:/map.json");
                    break;
            }
        }

        private Point[] getSanjiaoxingOtherPoint(Point rightPoint, int radius)
        {
            Point topPoint = new Point(rightPoint.X - radius, rightPoint.Y + radius);
            Point bottomPoint = new Point(rightPoint.X - radius, rightPoint.Y - radius);
            return new Point[] { rightPoint, topPoint, bottomPoint };
        }

        void MovePoint(string velStr, string pxStr, string pyStr)
        {
            try
            {
                double vel = double.Parse(velStr);
                int px = int.Parse(pxStr);
                int py = int.Parse(pyStr);
                Config cc = JsonHelper.DeserializeByFile<Config>("yining.config");
                Axis ax = cc.Axes.Find(v => v.Id == 2);
                Axis ay = cc.Axes.Find(v => v.Id == 1);
                Parallel.Invoke(() => MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), vel, px), () => MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), vel, py));
                //MessageBox.Show("daole");
            }
            catch (Exception er) { MessageBox.Show("输入有误"); }
        }
        public override void Close()
        {
            MotorsControl.stopCompare();
            mVCameraHelper.CloseCameras();
            base.Close();
        }
        #endregion

        int x = 0, y = 0;
        private void button12_Click(object sender, EventArgs e)
        {
            x += 1000;
            Axis axis = GetSelectAxis();
            MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), axis.TrapPrm.Vel, x);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            x = 0;y = 0;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            y -= 1000;
            Axis axis = GetSelectAxis();
            MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), axis.TrapPrm.Vel, y);
        }
    }
}
