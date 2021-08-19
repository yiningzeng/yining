using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;

namespace WaferAoi
{
    partial class DockWaferList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.darkWaferList1 = new YiNing.UI.Controls.DarkWaferList();
            this.darkButton1 = new YiNing.UI.Controls.DarkButton();
            this.darkButton2 = new YiNing.UI.Controls.DarkButton();
            this.darkButton3 = new YiNing.UI.Controls.DarkButton();
            this.SuspendLayout();
            // 
            // darkWaferList1
            // 
            this.darkWaferList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkWaferList1.ItemHeight = 30;
            this.darkWaferList1.Location = new System.Drawing.Point(0, 25);
            this.darkWaferList1.MultiSelect = true;
            this.darkWaferList1.Name = "darkWaferList1";
            this.darkWaferList1.ShowIcons = true;
            this.darkWaferList1.Size = new System.Drawing.Size(240, 469);
            this.darkWaferList1.TabIndex = 0;
            this.darkWaferList1.Text = "darkWaferList1";
            // 
            // darkButton1
            // 
            this.darkButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.darkButton1.Location = new System.Drawing.Point(3, 414);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 23);
            this.darkButton1.TabIndex = 1;
            this.darkButton1.Text = "重载";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.darkButton2.Location = new System.Drawing.Point(84, 414);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(73, 23);
            this.darkButton2.TabIndex = 2;
            this.darkButton2.Text = "开始工作";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton3
            // 
            this.darkButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.darkButton3.Location = new System.Drawing.Point(81, 443);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(76, 23);
            this.darkButton3.TabIndex = 3;
            this.darkButton3.Text = "停止工作";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // DockWaferList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkButton3);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.darkWaferList1);
            this.DefaultDockArea = YiNing.UI.Docking.DarkDockArea.Left;
            this.DockText = "晶圆列表";
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::WaferAoi.Icons.application_16x;
            this.Name = "DockWaferList";
            this.SerializationKey = "DockWaferList";
            this.Size = new System.Drawing.Size(240, 494);
            this.Load += new System.EventHandler(this.DockWaferList_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private DarkButton darkButton1;
        public DarkWaferList darkWaferList1;
        private DarkButton darkButton2;
        private DarkButton darkButton3;
    }
}
