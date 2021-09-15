using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogSelectProgram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogSelectProgram));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.cmbProgram = new YiNing.UI.Controls.DarkComboBox();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.tbWafweId = new YiNing.UI.Controls.DarkTextBox();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tbWafweId);
            this.pnlMain.Controls.Add(this.darkLabel2);
            this.pnlMain.Controls.Add(this.cmbProgram);
            this.pnlMain.Controls.Add(this.darkLabel1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(273, 120);
            this.pnlMain.TabIndex = 2;
            // 
            // cmbProgram
            // 
            this.cmbProgram.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbProgram.FormattingEnabled = true;
            this.cmbProgram.Location = new System.Drawing.Point(15, 33);
            this.cmbProgram.Name = "cmbProgram";
            this.cmbProgram.ReadOnly = true;
            this.cmbProgram.Size = new System.Drawing.Size(239, 24);
            this.cmbProgram.TabIndex = 16;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(12, 15);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(124, 15);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "选择需要运行的程式";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(12, 60);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(145, 15);
            this.darkLabel2.TabIndex = 17;
            this.darkLabel2.Text = "晶圆编号(手动设备填写)";
            // 
            // tbWafweId
            // 
            this.tbWafweId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbWafweId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbWafweId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbWafweId.Location = new System.Drawing.Point(15, 78);
            this.tbWafweId.Name = "tbWafweId";
            this.tbWafweId.Size = new System.Drawing.Size(239, 23);
            this.tbWafweId.TabIndex = 18;
            // 
            // DialogSelectProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 162);
            this.Controls.Add(this.pnlMain);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogSelectProgram";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "程式选择";
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DarkLabel darkLabel1;
        private DarkComboBox cmbProgram;
        private DarkLabel darkLabel2;
        private DarkTextBox tbWafweId;
    }
}