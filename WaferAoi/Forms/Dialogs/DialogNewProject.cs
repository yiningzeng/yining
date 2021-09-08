using System;
using System.IO;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Forms;
using static WaferAoi.Tools.EMUMS;

namespace WaferAoi
{
    public partial class DialogNewProject : DarkDialog
    {
        public DialogNewProject()
        {
            InitializeComponent();
            Ini();
        }
        public void Ini()
        {
            this.btnOk.Text = "确定";
            this.btnCancel.Text = "取消";
            cmbWaferSize.SelectedIndex = 0;
            cmbWaferType.SelectedIndex = 0;
            cmbFlatOrNotche.SelectedIndex = 0;
            cmbFlatNotcheDirection.SelectedIndex = 0;
            cmbRingPiece.SelectedIndex = 0;
#if DEBUG
            tbJobName.Text = "test";
#endif
        }

        private void cmbFlatOrNotche_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            lbFlatNotcheDirection.Text = cmbFlatOrNotche.SelectedIndex == 0 ? "凹槽位置：" : "切面位置：";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK && tbJobName.Text == "")
            {
                DarkMessageBox.ShowWarning("参数有误");
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.OK)
            {
                ProgramConfig pc = new ProgramConfig() { Name = tbJobName.Text,
                    HaveRingPiece = (HaveRingPiece)cmbRingPiece.SelectedIndex,
                    WaferSize = (WaferSize)cmbWaferSize.SelectedIndex,
                    WaferType = (WaferType)cmbWaferType.SelectedIndex, 
                    TraitType =(TraitType)cmbWaferType.SelectedIndex, 
                    TraitLocation = (TraitLocation)Enum.ToObject(typeof(TraitLocation), cmbFlatNotcheDirection.SelectedIndex),
                    ModelSavePath = Path.Combine(@"D:\QTWaferProgram", tbJobName.Text)
                };
                this.Tag = pc;

                if (!Directory.Exists(pc.ModelSavePath)) Directory.CreateDirectory(pc.ModelSavePath);
                JsonHelper.Serialize(pc, Path.Combine(pc.ModelSavePath, "config.zyn"));
            }
        }
    }
}
