using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;

namespace WaferAoi
{
    public partial class DockSoftwareEdit : DarkDocument
    {
        private MVCameraHelper mVCameraHelper;
        Config config;
        private ProgramConfig programConfig;

        public DockSoftwareEdit()
        {
            InitializeComponent();
            
            this.darkStepViewer1.OnStepChanged += DarkStepViewer1_OnStepChanged;
            IniData();
        }

        void IniData()
        {
            config = FsmHelper.GetConfig();
            #region 总流程步骤条
            List<DarkStepViewerItem> list = new List<DarkStepViewerItem>();
            list.Add(new DarkStepViewerItem("1", "放置晶圆", 1, "请放置晶圆", null));
            list.Add(new DarkStepViewerItem("2", "角度矫正", 2, "计算晶圆的中心和矫正晶圆的角度", null));
            list.Add(new DarkStepViewerItem("3", "芯片定位", 3, "精准的定位到芯片的位置", null));
            list.Add(new DarkStepViewerItem("4", "制作程式", 4, "制作芯片的模板信息来检测", null));
            list.Add(new DarkStepViewerItem("5", "程式测试", 5, "测试制作的模板信息的检测结果", null));
            list.Add(new DarkStepViewerItem("6", "保存程式", 6, "保存该程式下次直接调用检测", null));
            darkStepViewer1.ListDataSource = list;
            darkStepViewer1.CurrentStep = 1;
            #endregion

            #region 第一步流程引导
            dlvwProgress1.Items.Clear();
            dlvwProgress1.Items.Add(new DarkListItem("放置晶圆"));
            dlvwProgress1.Items.Add(new DarkListItem("开启真空阀"));
            dlvwProgress1.SetStartNum(0);
            #endregion

            #region 第二步流程引导
            dlvwProgress2.Items.Clear();
            dlvwProgress2.Items.Add(new DarkListItem("开始"));
            //dlvwProgress2.Items.Add(new DarkListItem("定位晶圆切面/凹槽"));
            dlvwProgress2.Items.Add(new DarkListItem("寻找右边沿"));
            dlvwProgress2.Items.Add(new DarkListItem("寻找上边沿"));
            dlvwProgress2.Items.Add(new DarkListItem("寻找下边沿"));
            dlvwProgress2.Items.Add(new DarkListItem("开始计算"));
            dlvwProgress2.Items.Add(new DarkListItem("方向矫正"));
            dlvwProgress2.Items.Add(new DarkListItem("完成"));
            dlvwProgress2.SetStartNum(0);
            dlvwProgress2.Stop();
            
            #endregion
        }

        public DockSoftwareEdit(string text, Image icon, ref MVCameraHelper mc, ProgramConfig pc = null) : this()
        {
            DockText = text;
            Icon = icon;
            programConfig = pc;
            mVCameraHelper = mc;
            mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
        }

        private void MVCameraHelper_CameraImageCallBack(object sender, ImageArgs e)
        {
            HSmartWindowControl tempControl = null;
            switch (dlvwProgress2.NowItem().Text)
            {
                case "定位晶圆切面/凹槽":
                    tempControl = hswcFlatOrNotch;
                    break;
                case "寻找右边沿":
                    tempControl = hswcRight;
                    break;
                case "寻找上边沿":
                    tempControl = hswcTop;
                    break;
                case "寻找下边沿":
                    tempControl = hswcBottom;
                    break;
            }

            if (tempControl != null)
            {
                //HOperatorSet.GetImageSize(e.ImageHobject, out HTuple Iwidth, out HTuple Iheight);
                HOperatorSet.SetPart(tempControl.HalconWindow, 0, 0, e.Height - 1, e.Width - 1);
                HOperatorSet.ClearWindow(tempControl.HalconWindow);
                HOperatorSet.DispObj(e.ImageHobject, tempControl.HalconWindow);
                tempControl.SetFullImagePart();
                HOperatorSet.GenCrossContourXld(out HObject hObjectCross, e.Height / 2, e.Width / 2, e.Width, 0);
                HOperatorSet.DispObj(hObjectCross, tempControl.HalconWindow);
                //new HDevelopExport();
                e.ImageHobject.Dispose();
            }
        }


        public void SetConfig(ProgramConfig pc)
        {
            programConfig = pc;
        }

        protected virtual void DarkStepViewer1_OnStepChanged(object sender, EventArgs e)
        {
            darkTabControl1.SelectedIndex = darkStepViewer1.CurrentStep - 1;
        }


        #region 第一步
        private void btnMoveToPlace_Click(object sender, EventArgs e)
        {
            Axis ax = config.Axes.Find(v => v.Id == 2);
            Axis ay = config.Axes.Find(v => v.Id == 1);
            MotorsControl.MovePoint2D(60, ax.StartPoint, ay.StartPoint, ax, ay);
            dlvwProgress1.SetStartNum(0);
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
                dlvwProgress1.Next(); dlvwProgress1.Next();
            }
            else
            {
                dlvwProgress1.Previous(); dlvwProgress1.Previous();
            }
        }
        #endregion

        #region 第二步
        /// <summary>
        /// 第二部自动开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartSearch_Click(object sender, EventArgs e)
        {
            dlvwProgress2.SetStartNum(1);
            MotorsControl.setCompareMode(2, 1, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(200); // 等差模式
            MotorsControl.startCompare();
            //MotorsControl.stopCompare();
        }
        private void darkButton1_Click(object sender, EventArgs e)
        {
            dlvwProgress2.Next();
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            string tag = (sender as ContextMenuStrip).SourceControl.Tag.ToString();
            HSmartWindowControl tempControl = null;
            switch (tag)
            {
                case "定位晶圆切面或凹槽":
                    tempControl = hswcFlatOrNotch;
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
            }
            contextMenuStrip1.Tag = tempControl;
        }
    }
}
