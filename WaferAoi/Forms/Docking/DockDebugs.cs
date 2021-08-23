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

namespace WaferAoi
{
    public partial class DockDebugs : DarkDocument
    {
        #region Constructor Region
        MVCameraHelper mVCameraHelper = new MVCameraHelper(2);
        Config config;
        public DockDebugs()
        {
            InitializeComponent();
            HookEvents();
            Ini();
        }

        public DockDebugs(string text, Image icon)
            : this()
        {
            DockText = text;
            Icon = icon;
        }

        #endregion

        #region Method Region
        private void HookEvents()
        {
            //运动控制
            ControlHelper.SetButtonMouseDown(new Control[] { dsepMove }, Button_MouseDown);
            ControlHelper.SetButtonMouseUp(new Control[] { dsepMove }, Button_MouseUp);
            //注册所有darkbutton
            ControlHelper.SetDarkButtonClick(new Control[] { this }, DarkButton_Click);
        }
        private void Ini()
        {
            mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
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
            List<DarkStepViewer.StepEntity> list = new List<DarkStepViewer.StepEntity>();
            list.Add(new DarkStepViewer.StepEntity("1", "步骤1", 1, "这里是该步骤的描述信息", null));

            list.Add(new DarkStepViewer.StepEntity("2", "步骤2", 2, "这里是该步骤的描述信息", null));
            list.Add(new DarkStepViewer.StepEntity("3", "步骤步骤23", 3, "这里是该步骤的描述信息", null));
            list.Add(new DarkStepViewer.StepEntity("2", "步骤4", 4, "这里是该步骤的描述信息", null));

            this.darkStepViewer1.CurrentStep = 2;
            this.darkStepViewer1.ListDataSource = list;
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
                    break;
                case "跑到保存点2":
                    axis = GetSelectAxis();
                    MotorsControl.MoveTrap(axis.Id, axis.TrapPrm.Get(), axis.TrapPrm.Vel, axis.EndPoint);
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
            }
        }
        public override void Close()
        {
            MotorsControl.stopCompare();
            mVCameraHelper.CloseCameras();
            base.Close();
        }
        #endregion
    }
}
