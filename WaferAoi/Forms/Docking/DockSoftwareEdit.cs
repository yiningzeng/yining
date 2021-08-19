using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using WaferAoi.Tools;
using System.Collections.Generic;

namespace WaferAoi
{
    public partial class DockSoftwareEdit : DarkDocument
    {
        #region Field Region
        private MainForm main;
        #endregion

        #region Constructor Region

        public DockSoftwareEdit()
        {
            InitializeComponent();
           
            List<DarkStepViewer.StepEntity> list = new List<DarkStepViewer.StepEntity>();
            list.Add(new DarkStepViewer.StepEntity("1", "放置晶圆", 1, "请放置晶圆", null));

            list.Add(new DarkStepViewer.StepEntity("2", "晶圆定位", 2, "精准的定位到晶圆的位置", null));
            list.Add(new DarkStepViewer.StepEntity("3", "制作程式", 3, "这里是该步骤的描述信息",null));
            list.Add(new DarkStepViewer.StepEntity("4", "程式测试", 4, "这里是该步骤的描述信息", null));
            list.Add(new DarkStepViewer.StepEntity("5", "保存程式", 5, "这里是该步骤的描述信息", null));

            this.darkStepViewer1.CurrentStep = 2;
            this.darkStepViewer1.ListDataSource = list;
        }

        public DockSoftwareEdit(MainForm main, string text, Image icon) : this()
        {
            this.main = main;
            DockText = text;
            Icon = icon;
        }
        #endregion

        #region Event Handler Region
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
            darkStepViewer1.Complete();
            int w = darkStepViewer1.Width;
           //int aaa=  darkStepViewer1.getlineWidth();
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            darkStepViewer1.PreviousStep();
        }
    }
}
