using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogTestModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogTestModel));
            this.hswcMain = new HalconDotNet.HSmartWindowControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.darkToolStrip1 = new YiNing.UI.Controls.DarkToolStrip();
            this.debugButton = new System.Windows.Forms.ToolStripButton();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.darkToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hswcMain
            // 
            this.hswcMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hswcMain.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hswcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hswcMain.HDoubleClickToFitContent = true;
            this.hswcMain.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hswcMain.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hswcMain.HKeepAspectRatio = true;
            this.hswcMain.HMoveContent = true;
            this.hswcMain.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hswcMain.Location = new System.Drawing.Point(0, 0);
            this.hswcMain.Margin = new System.Windows.Forms.Padding(0);
            this.hswcMain.Name = "hswcMain";
            this.hswcMain.Size = new System.Drawing.Size(961, 700);
            this.hswcMain.TabIndex = 4;
            this.hswcMain.WindowSize = new System.Drawing.Size(961, 700);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(961, 700);
            this.panel3.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.darkToolStrip1);
            this.panel1.Controls.Add(this.hswcMain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(961, 700);
            this.panel1.TabIndex = 0;
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugButton});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(961, 25);
            this.darkToolStrip1.TabIndex = 5;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // debugButton
            // 
            this.debugButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.debugButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.debugButton.Image = global::WaferAoi.Properties.Resources.debug;
            this.debugButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(124, 22);
            this.debugButton.Text = "测试一张本地图片";
            this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
            // 
            // DialogTestModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 742);
            this.Controls.Add(this.panel3);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.Close;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogTestModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模板测试";
            this.Controls.SetChildIndex(this.panel3, 0);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel3;
        private HalconDotNet.HSmartWindowControl hswcMain;
        private System.Windows.Forms.Panel panel1;
        private DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton debugButton;
    }
}