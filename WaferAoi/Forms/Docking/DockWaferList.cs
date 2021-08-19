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
        #region Constructor Region

        public DockWaferList()
        {
            InitializeComponent();
        }

        #endregion
        private void darkButton1_Click(object sender, System.EventArgs e)
        {
            darkWaferList1.RefreshWafer();
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            darkWaferList1.StartWorking();
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            darkWaferList1.StopWorking();
        }

        private void DockWaferList_Load(object sender, EventArgs e)
        {
            darkWaferList1.RefreshWafer();
        }
    }
}
