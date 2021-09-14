using YiNing.UI.Forms;
using System.Windows.Forms;

namespace WaferAoi
{
    public partial class DialogWait : DarkDialog
    {
        #region Constructor Region

        public DialogWait()
        {
            InitializeComponent();
            btnOk.Text = "Close";
        }

        public DialogWait(string msg) : this()
        {
            lbMsg.Text = msg;
        }

        #endregion
    }
}
