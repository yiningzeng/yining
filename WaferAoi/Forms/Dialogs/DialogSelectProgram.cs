using YiNing.UI.Forms;
using System.Windows.Forms;
using WaferAoi.Tools;
using System.IO;

namespace WaferAoi
{
    public partial class DialogSelectProgram : DarkDialog
    {
        Config config;
        #region Constructor Region

        public DialogSelectProgram()
        {
            InitializeComponent();
            this.btnOk.Text = "确定";
            this.btnCancel.Text = "取消";
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            config = FsmHelper.GetConfig();
            this.Load += DialogSelectProgram_Load;
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            if (cmbProgram.Text == "" || tbWafweId.Text == "")
            {
                DarkMessageBox.ShowError("请填写参数");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Tag = cmbProgram.Text+"@"+tbWafweId.Text;
            //this.Hide();
        }

        private void DialogSelectProgram_Load(object sender, System.EventArgs e)
        {
            if (!Directory.Exists(config.ProgramSavePath)) return;
            foreach (var f in Directory.GetDirectories(config.ProgramSavePath))
            {
                cmbProgram.Items.Add(Path.GetFileName(f));
            }
        }

        #endregion
    }
}
