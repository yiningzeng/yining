using YiNing.UI.Forms;
using System.Windows.Forms;

namespace WaferAoi
{
    public partial class DialogAbout : DarkDialog
    {
        #region Constructor Region

        public DialogAbout()
        {
            InitializeComponent();

            lblVersion.Text = $"版本号: {Application.ProductVersion.ToString()}";
            btnOk.Text = "Close";
        }

        #endregion
    }
}
