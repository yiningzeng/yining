using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogCreateModel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogCreateModel));
            this.hswcMain = new HalconDotNet.HSmartWindowControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.画一个矩形区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStrip1 = new YiNing.UI.Controls.DarkToolStrip();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.debugButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.darkSectionPanel2 = new YiNing.UI.Controls.DarkSectionPanel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.dlvwProgress = new YiNing.UI.Controls.DarkProgressReminder();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            this.darkToolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.darkSectionPanel2.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hswcMain
            // 
            this.hswcMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hswcMain.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hswcMain.ContextMenuStrip = this.contextMenuStrip1;
            this.hswcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hswcMain.HDoubleClickToFitContent = true;
            this.hswcMain.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hswcMain.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hswcMain.HKeepAspectRatio = true;
            this.hswcMain.HMoveContent = true;
            this.hswcMain.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hswcMain.Location = new System.Drawing.Point(0, 25);
            this.hswcMain.Margin = new System.Windows.Forms.Padding(0);
            this.hswcMain.Name = "hswcMain";
            this.hswcMain.Size = new System.Drawing.Size(1090, 778);
            this.hswcMain.TabIndex = 4;
            this.hswcMain.WindowSize = new System.Drawing.Size(1090, 778);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.画一个矩形区域ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 26);
            // 
            // 画一个矩形区域ToolStripMenuItem
            // 
            this.画一个矩形区域ToolStripMenuItem.Name = "画一个矩形区域ToolStripMenuItem";
            this.画一个矩形区域ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.画一个矩形区域ToolStripMenuItem.Text = "画一个矩形区域";
            this.画一个矩形区域ToolStripMenuItem.Click += new System.EventHandler(this.画一个矩形区域ToolStripMenuItem_Click);
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton7,
            this.debugButton,
            this.toolStripButton1});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(1090, 25);
            this.darkToolStrip1.TabIndex = 3;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton7.Image = global::WaferAoi.Properties.Resources.Save;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton7.Text = "保存模板";
            this.toolStripButton7.Click += new System.EventHandler(this.SaveModel_Click);
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
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton1.Image = global::WaferAoi.Icons.Close_16xLG;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(124, 22);
            this.toolStripButton1.Text = "双击关闭测试窗口";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.darkSectionPanel2);
            this.panel2.Controls.Add(this.darkSectionPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(1093, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 803);
            this.panel2.TabIndex = 4;
            // 
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.propertyGrid1);
            this.darkSectionPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkSectionPanel2.Location = new System.Drawing.Point(0, 295);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "参数";
            this.darkSectionPanel2.Size = new System.Drawing.Size(200, 508);
            this.darkSectionPanel2.TabIndex = 3;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(1, 25);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(198, 482);
            this.propertyGrid1.TabIndex = 0;
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.dlvwProgress);
            this.darkSectionPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkSectionPanel1.Location = new System.Drawing.Point(0, 0);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "流程参考";
            this.darkSectionPanel1.Size = new System.Drawing.Size(200, 295);
            this.darkSectionPanel1.TabIndex = 2;
            // 
            // dlvwProgress
            // 
            this.dlvwProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dlvwProgress.Location = new System.Drawing.Point(1, 25);
            this.dlvwProgress.Name = "dlvwProgress";
            this.dlvwProgress.Size = new System.Drawing.Size(198, 269);
            this.dlvwProgress.TabIndex = 1;
            this.dlvwProgress.Text = "darkProgressReminder1";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1090, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 803);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1090, 803);
            this.panel3.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.hswcMain);
            this.panel1.Controls.Add(this.darkToolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1090, 803);
            this.panel1.TabIndex = 0;
            // 
            // DialogCreateModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1293, 845);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel2);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogCreateModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片模板";
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.splitter1, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.contextMenuStrip1.ResumeLayout(false);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.darkSectionPanel2.ResumeLayout(false);
            this.darkSectionPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel3;
        private HalconDotNet.HSmartWindowControl hswcMain;
        private DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton debugButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 画一个矩形区域ToolStripMenuItem;
        private DarkSectionPanel darkSectionPanel1;
        private DarkProgressReminder dlvwProgress;
        private DarkSectionPanel darkSectionPanel2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}