using YiNing.UI.Forms;
using System.Windows.Forms;

namespace WaferAoi
{
    public partial class DialogMessage : DarkDialog
    {
        #region Constructor Region

        public DialogMessage()
        {
            InitializeComponent();

            //lblVersion.Text = $"版本号: {Application.ProductVersion.ToString()}";
            btnOk.Text = "Close";
        }

        #endregion
    }
}
