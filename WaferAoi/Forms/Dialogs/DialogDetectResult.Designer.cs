
namespace WaferAoi
{
    partial class DialogDetectResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogDetectResult));
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkSectionPanel3 = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkLabel6 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel5 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel4 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel3 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.waferMap = new YiNing.UI.Controls.Wafermap();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.darkSectionPanel2 = new YiNing.UI.Controls.DarkSectionPanel();
            this.hswcMain = new HalconDotNet.HSmartWindowControl();
            this.darkSectionPanel1.SuspendLayout();
            this.darkSectionPanel3.SuspendLayout();
            this.darkSectionPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.darkSectionPanel3);
            this.darkSectionPanel1.Controls.Add(this.waferMap);
            this.darkSectionPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.darkSectionPanel1.Location = new System.Drawing.Point(0, 0);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "晶圆";
            this.darkSectionPanel1.Size = new System.Drawing.Size(705, 798);
            this.darkSectionPanel1.TabIndex = 0;
            // 
            // darkSectionPanel3
            // 
            this.darkSectionPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkSectionPanel3.Controls.Add(this.darkLabel6);
            this.darkSectionPanel3.Controls.Add(this.darkLabel5);
            this.darkSectionPanel3.Controls.Add(this.darkLabel4);
            this.darkSectionPanel3.Controls.Add(this.darkLabel3);
            this.darkSectionPanel3.Controls.Add(this.darkLabel2);
            this.darkSectionPanel3.Controls.Add(this.darkLabel1);
            this.darkSectionPanel3.DragEnable = true;
            this.darkSectionPanel3.ForeColor = System.Drawing.Color.Chocolate;
            this.darkSectionPanel3.Location = new System.Drawing.Point(552, 41);
            this.darkSectionPanel3.Name = "darkSectionPanel3";
            this.darkSectionPanel3.SectionHeader = "统计";
            this.darkSectionPanel3.Size = new System.Drawing.Size(136, 106);
            this.darkSectionPanel3.TabIndex = 1;
            // 
            // darkLabel6
            // 
            this.darkLabel6.AutoSize = true;
            this.darkLabel6.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel6.Location = new System.Drawing.Point(75, 86);
            this.darkLabel6.Name = "darkLabel6";
            this.darkLabel6.Size = new System.Drawing.Size(11, 12);
            this.darkLabel6.TabIndex = 5;
            this.darkLabel6.Text = "-";
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel5.Location = new System.Drawing.Point(75, 63);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(11, 12);
            this.darkLabel5.TabIndex = 4;
            this.darkLabel5.Text = "-";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel4.Location = new System.Drawing.Point(75, 38);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(29, 12);
            this.darkLabel4.TabIndex = 3;
            this.darkLabel4.Text = "3844";
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel3.Location = new System.Drawing.Point(13, 86);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(65, 12);
            this.darkLabel3.TabIndex = 2;
            this.darkLabel3.Text = "缺陷个数：";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel2.Location = new System.Drawing.Point(13, 63);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(65, 12);
            this.darkLabel2.TabIndex = 1;
            this.darkLabel2.Text = "良品个数：";
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel1.Location = new System.Drawing.Point(13, 38);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(65, 12);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "芯片总数：";
            // 
            // waferMap
            // 
            this.waferMap.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(139))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(203))))),
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty};
            this.waferMap.Dataset = null;
            this.waferMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waferMap.Location = new System.Drawing.Point(1, 25);
            this.waferMap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.waferMap.Name = "waferMap";
            this.waferMap.NoDataString = "NO DATA";
            this.waferMap.Notchlocation = 0;
            this.waferMap.Rotation = 0;
            this.waferMap.SelectOneDie = null;
            this.waferMap.SelectRegionDiagonalDie = null;
            this.waferMap.SelectX = 0;
            this.waferMap.SelectY = 0;
            this.waferMap.Size = new System.Drawing.Size(703, 772);
            this.waferMap.TabIndex = 0;
            this.waferMap.TooSmallString = "TOO SMALL";
            this.waferMap.TranslationX = 0;
            this.waferMap.TranslationY = 0;
            this.waferMap.VisibleDatasetSavePath = "";
            this.waferMap.Zoom = 1F;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(705, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 798);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.hswcMain);
            this.darkSectionPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkSectionPanel2.Location = new System.Drawing.Point(710, 0);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "检测结果";
            this.darkSectionPanel2.Size = new System.Drawing.Size(548, 798);
            this.darkSectionPanel2.TabIndex = 2;
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
            this.hswcMain.Location = new System.Drawing.Point(1, 25);
            this.hswcMain.Margin = new System.Windows.Forms.Padding(0);
            this.hswcMain.Name = "hswcMain";
            this.hswcMain.Size = new System.Drawing.Size(546, 772);
            this.hswcMain.TabIndex = 0;
            this.hswcMain.WindowSize = new System.Drawing.Size(546, 772);
            // 
            // DialogDetectResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(1258, 798);
            this.Controls.Add(this.darkSectionPanel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.darkSectionPanel1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogDetectResult";
            this.Text = "检测结果";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel3.ResumeLayout(false);
            this.darkSectionPanel3.PerformLayout();
            this.darkSectionPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private YiNing.UI.Controls.DarkSectionPanel darkSectionPanel1;
        private System.Windows.Forms.Splitter splitter1;
        private YiNing.UI.Controls.DarkSectionPanel darkSectionPanel2;
        private HalconDotNet.HSmartWindowControl hswcMain;
        private YiNing.UI.Controls.Wafermap waferMap;
        private YiNing.UI.Controls.DarkSectionPanel darkSectionPanel3;
        private YiNing.UI.Controls.DarkLabel darkLabel1;
        private YiNing.UI.Controls.DarkLabel darkLabel3;
        private YiNing.UI.Controls.DarkLabel darkLabel2;
        private YiNing.UI.Controls.DarkLabel darkLabel4;
        private YiNing.UI.Controls.DarkLabel darkLabel6;
        private YiNing.UI.Controls.DarkLabel darkLabel5;
    }
}