using System.Drawing;
using YiNing.UI.Controls;
using YiNing.UI.Docking;

namespace WaferAoi
{
    public partial class DockControl : DarkToolWindow
    {
        private MainForm mainForm;

        #region Constructor Region

        public DockControl(MainForm mForm)
        {
            InitializeComponent();
            mainForm = mForm;
        }
        #endregion

        private void btnRobot_Click(object sender, System.EventArgs e)
        {
            mainForm._dockWaferList.StartWorking();
            //mainForm._dockWorkSpace.SetProgress();
        }
    }
}
