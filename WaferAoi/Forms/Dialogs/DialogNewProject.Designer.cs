using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogNewProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogNewProject));
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkLabel5 = new YiNing.UI.Controls.DarkLabel();
            this.cmbFlatOrNotche = new YiNing.UI.Controls.DarkComboBox();
            this.lbFlatNotcheDirection = new YiNing.UI.Controls.DarkLabel();
            this.cmbFlatNotcheDirection = new YiNing.UI.Controls.DarkComboBox();
            this.darkLabel3 = new YiNing.UI.Controls.DarkLabel();
            this.cmbWaferType = new YiNing.UI.Controls.DarkComboBox();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.cmbWaferSize = new YiNing.UI.Controls.DarkComboBox();
            this.tbJobName = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel4 = new YiNing.UI.Controls.DarkLabel();
            this.cmbRingPiece = new YiNing.UI.Controls.DarkComboBox();
            this.darkSectionPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.darkLabel4);
            this.darkSectionPanel1.Controls.Add(this.cmbRingPiece);
            this.darkSectionPanel1.Controls.Add(this.darkLabel5);
            this.darkSectionPanel1.Controls.Add(this.cmbFlatOrNotche);
            this.darkSectionPanel1.Controls.Add(this.lbFlatNotcheDirection);
            this.darkSectionPanel1.Controls.Add(this.cmbFlatNotcheDirection);
            this.darkSectionPanel1.Controls.Add(this.darkLabel3);
            this.darkSectionPanel1.Controls.Add(this.cmbWaferType);
            this.darkSectionPanel1.Controls.Add(this.darkLabel2);
            this.darkSectionPanel1.Controls.Add(this.cmbWaferSize);
            this.darkSectionPanel1.Controls.Add(this.tbJobName);
            this.darkSectionPanel1.Controls.Add(this.darkLabel1);
            this.darkSectionPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkSectionPanel1.Location = new System.Drawing.Point(0, 0);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = null;
            this.darkSectionPanel1.Size = new System.Drawing.Size(291, 274);
            this.darkSectionPanel1.TabIndex = 2;
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(15, 131);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(77, 15);
            this.darkLabel5.TabIndex = 22;
            this.darkLabel5.Text = "凹槽/切面：";
            // 
            // cmbFlatOrNotche
            // 
            this.cmbFlatOrNotche.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbFlatOrNotche.FormattingEnabled = true;
            this.cmbFlatOrNotche.Items.AddRange(new object[] {
            "凹槽",
            "切面"});
            this.cmbFlatOrNotche.Location = new System.Drawing.Point(94, 128);
            this.cmbFlatOrNotche.Name = "cmbFlatOrNotche";
            this.cmbFlatOrNotche.ReadOnly = true;
            this.cmbFlatOrNotche.Size = new System.Drawing.Size(183, 24);
            this.cmbFlatOrNotche.TabIndex = 21;
            this.cmbFlatOrNotche.SelectedIndexChanged += new System.EventHandler(this.cmbFlatOrNotche_SelectedIndexChanged);
            // 
            // lbFlatNotcheDirection
            // 
            this.lbFlatNotcheDirection.AutoSize = true;
            this.lbFlatNotcheDirection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbFlatNotcheDirection.Location = new System.Drawing.Point(20, 162);
            this.lbFlatNotcheDirection.Name = "lbFlatNotcheDirection";
            this.lbFlatNotcheDirection.Size = new System.Drawing.Size(72, 15);
            this.lbFlatNotcheDirection.TabIndex = 20;
            this.lbFlatNotcheDirection.Text = "凹槽位置：";
            // 
            // cmbFlatNotcheDirection
            // 
            this.cmbFlatNotcheDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbFlatNotcheDirection.FormattingEnabled = true;
            this.cmbFlatNotcheDirection.Items.AddRange(new object[] {
            "左"});
            this.cmbFlatNotcheDirection.Location = new System.Drawing.Point(94, 158);
            this.cmbFlatNotcheDirection.MaxDropDownItems = 1;
            this.cmbFlatNotcheDirection.Name = "cmbFlatNotcheDirection";
            this.cmbFlatNotcheDirection.ReadOnly = true;
            this.cmbFlatNotcheDirection.Size = new System.Drawing.Size(183, 24);
            this.cmbFlatNotcheDirection.TabIndex = 19;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(20, 102);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(72, 15);
            this.darkLabel3.TabIndex = 18;
            this.darkLabel3.Text = "晶圆类型：";
            // 
            // cmbWaferType
            // 
            this.cmbWaferType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbWaferType.FormattingEnabled = true;
            this.cmbWaferType.Items.AddRange(new object[] {
            "图案晶圆",
            "无图案晶圆"});
            this.cmbWaferType.Location = new System.Drawing.Point(94, 98);
            this.cmbWaferType.Name = "cmbWaferType";
            this.cmbWaferType.ReadOnly = true;
            this.cmbWaferType.Size = new System.Drawing.Size(183, 24);
            this.cmbWaferType.TabIndex = 17;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(20, 72);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(72, 15);
            this.darkLabel2.TabIndex = 16;
            this.darkLabel2.Text = "晶圆尺寸：";
            // 
            // cmbWaferSize
            // 
            this.cmbWaferSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbWaferSize.FormattingEnabled = true;
            this.cmbWaferSize.Items.AddRange(new object[] {
            "6寸",
            "8寸"});
            this.cmbWaferSize.Location = new System.Drawing.Point(94, 68);
            this.cmbWaferSize.Name = "cmbWaferSize";
            this.cmbWaferSize.ReadOnly = true;
            this.cmbWaferSize.Size = new System.Drawing.Size(183, 24);
            this.cmbWaferSize.TabIndex = 15;
            // 
            // tbJobName
            // 
            this.tbJobName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbJobName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbJobName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbJobName.Location = new System.Drawing.Point(94, 39);
            this.tbJobName.Name = "tbJobName";
            this.tbJobName.Size = new System.Drawing.Size(183, 23);
            this.tbJobName.TabIndex = 1;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(20, 43);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(72, 15);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "程式名称：";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(7, 191);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(85, 15);
            this.darkLabel4.TabIndex = 24;
            this.darkLabel4.Text = "是否带环片：";
            // 
            // cmbRingPiece
            // 
            this.cmbRingPiece.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbRingPiece.FormattingEnabled = true;
            this.cmbRingPiece.Items.AddRange(new object[] {
            "不带环片",
            "带环片"});
            this.cmbRingPiece.Location = new System.Drawing.Point(94, 188);
            this.cmbRingPiece.MaxDropDownItems = 1;
            this.cmbRingPiece.Name = "cmbRingPiece";
            this.cmbRingPiece.ReadOnly = true;
            this.cmbRingPiece.Size = new System.Drawing.Size(183, 24);
            this.cmbRingPiece.TabIndex = 23;
            // 
            // DialogNewProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 274);
            this.Controls.Add(this.darkSectionPanel1);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogNewProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新建程式";
            this.Controls.SetChildIndex(this.darkSectionPanel1, 0);
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkSectionPanel darkSectionPanel1;
        private DarkLabel darkLabel1;
        private DarkTextBox tbJobName;
        private DarkLabel darkLabel2;
        private DarkComboBox cmbWaferSize;
        private DarkLabel darkLabel3;
        private DarkComboBox cmbWaferType;
        private DarkLabel lbFlatNotcheDirection;
        private DarkComboBox cmbFlatNotcheDirection;
        private DarkLabel darkLabel5;
        private DarkComboBox cmbFlatOrNotche;
        private DarkLabel darkLabel4;
        private DarkComboBox cmbRingPiece;
    }
}