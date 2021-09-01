using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogSavePoint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogSavePoint));
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.tbName = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.tbX = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.tbY = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel3 = new YiNing.UI.Controls.DarkLabel();
            this.tbZ = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel4 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel5 = new YiNing.UI.Controls.DarkLabel();
            this.cmbWaferSize = new YiNing.UI.Controls.DarkComboBox();
            this.darkSectionPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.darkLabel5);
            this.darkSectionPanel1.Controls.Add(this.cmbWaferSize);
            this.darkSectionPanel1.Controls.Add(this.tbZ);
            this.darkSectionPanel1.Controls.Add(this.darkLabel4);
            this.darkSectionPanel1.Controls.Add(this.tbY);
            this.darkSectionPanel1.Controls.Add(this.darkLabel3);
            this.darkSectionPanel1.Controls.Add(this.tbX);
            this.darkSectionPanel1.Controls.Add(this.darkLabel2);
            this.darkSectionPanel1.Controls.Add(this.tbName);
            this.darkSectionPanel1.Controls.Add(this.darkLabel1);
            this.darkSectionPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkSectionPanel1.Location = new System.Drawing.Point(0, 0);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = null;
            this.darkSectionPanel1.Size = new System.Drawing.Size(281, 208);
            this.darkSectionPanel1.TabIndex = 2;
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbName.Location = new System.Drawing.Point(87, 39);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(183, 23);
            this.tbName.TabIndex = 1;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(9, 41);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(72, 15);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "点位名称：";
            // 
            // tbX
            // 
            this.tbX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbX.Location = new System.Drawing.Point(87, 98);
            this.tbX.Name = "tbX";
            this.tbX.ReadOnly = true;
            this.tbX.Size = new System.Drawing.Size(183, 23);
            this.tbX.TabIndex = 3;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(54, 100);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(27, 15);
            this.darkLabel2.TabIndex = 2;
            this.darkLabel2.Text = "X：";
            // 
            // tbY
            // 
            this.tbY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbY.Location = new System.Drawing.Point(87, 127);
            this.tbY.Name = "tbY";
            this.tbY.ReadOnly = true;
            this.tbY.Size = new System.Drawing.Size(183, 23);
            this.tbY.TabIndex = 5;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(54, 129);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(27, 15);
            this.darkLabel3.TabIndex = 4;
            this.darkLabel3.Text = "Y：";
            // 
            // tbZ
            // 
            this.tbZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbZ.Location = new System.Drawing.Point(87, 156);
            this.tbZ.Name = "tbZ";
            this.tbZ.ReadOnly = true;
            this.tbZ.Size = new System.Drawing.Size(183, 23);
            this.tbZ.TabIndex = 7;
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(54, 158);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(27, 15);
            this.darkLabel4.TabIndex = 6;
            this.darkLabel4.Text = "Z：";
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(9, 71);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(72, 15);
            this.darkLabel5.TabIndex = 18;
            this.darkLabel5.Text = "关联尺寸：";
            // 
            // cmbWaferSize
            // 
            this.cmbWaferSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbWaferSize.FormattingEnabled = true;
            this.cmbWaferSize.Items.AddRange(new object[] {
            "6寸",
            "8寸"});
            this.cmbWaferSize.Location = new System.Drawing.Point(87, 68);
            this.cmbWaferSize.Name = "cmbWaferSize";
            this.cmbWaferSize.ReadOnly = true;
            this.cmbWaferSize.Size = new System.Drawing.Size(183, 24);
            this.cmbWaferSize.TabIndex = 17;
            // 
            // DialogSavePoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 250);
            this.Controls.Add(this.darkSectionPanel1);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogSavePoint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "保存点位信息";
            this.Controls.SetChildIndex(this.darkSectionPanel1, 0);
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkSectionPanel darkSectionPanel1;
        private DarkLabel darkLabel1;
        private DarkTextBox tbName;
        private DarkTextBox tbZ;
        private DarkLabel darkLabel4;
        private DarkTextBox tbY;
        private DarkLabel darkLabel3;
        private DarkTextBox tbX;
        private DarkLabel darkLabel2;
        private DarkLabel darkLabel5;
        private DarkComboBox cmbWaferSize;
    }
}