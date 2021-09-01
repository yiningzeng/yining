using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using YiNing.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;

namespace WaferAoi
{
    public partial class DockWaferList : DarkToolWindow
    {
        private MainForm mainForm;
        #region Constructor Region

        public DockWaferList()
        {
            InitializeComponent();
        }
        public DockWaferList(MainForm main) : this()
        {
            mainForm = main;
        }
        #endregion


        public void StartWorking()
        {
            if (darkWaferList1.SelectedIndices.Count == 0)
            {
                DarkMessageBox.ShowWarning("还未选择需要检测的晶圆");
                return;
            }
            //var aa = darkWaferList1.Items[darkWaferList1.SelectedIndices[0]].Tag;
            darkWaferList1.StartWorking();
            mainForm._dockWorkSpace.SetProgress();
        }

        public void StopWorking()
        {
            darkWaferList1.StopWorking();
        }

        public void RefreshWafer()
        {
            darkWaferList1.RefreshWafer();
        }

        private void darkButton1_Click(object sender, System.EventArgs e)
        {
            RefreshWafer();
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            StartWorking();
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            StopWorking();
        }

        private void DockWaferList_Load(object sender, EventArgs e)
        {
            darkWaferList1.RefreshWafer();
        }
    }
}
