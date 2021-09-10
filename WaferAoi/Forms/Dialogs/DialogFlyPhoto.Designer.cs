﻿using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogFlyPhoto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogFlyPhoto));
            this.hswcMain = new HalconDotNet.HSmartWindowControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.画一个矩形区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStrip1 = new YiNing.UI.Controls.DarkToolStrip();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dsepMove = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkLabel9 = new YiNing.UI.Controls.DarkLabel();
            this.dbupVel = new YiNing.UI.Controls.DarkNumericUpDown();
            this.darkGroupBox7 = new YiNing.UI.Controls.DarkGroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tbDistance = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tbVel = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.tbStart = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel3 = new YiNing.UI.Controls.DarkLabel();
            this.btnSetStart = new YiNing.UI.Controls.DarkButton();
            this.contextMenuStrip1.SuspendLayout();
            this.darkToolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.dsepMove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbupVel)).BeginInit();
            this.darkGroupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // hswcMain
            // 
            this.hswcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hswcMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hswcMain.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hswcMain.ContextMenuStrip = this.contextMenuStrip1;
            this.hswcMain.HDoubleClickToFitContent = true;
            this.hswcMain.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hswcMain.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hswcMain.HKeepAspectRatio = true;
            this.hswcMain.HMoveContent = true;
            this.hswcMain.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hswcMain.Location = new System.Drawing.Point(0, 25);
            this.hswcMain.Margin = new System.Windows.Forms.Padding(0);
            this.hswcMain.Name = "hswcMain";
            this.hswcMain.Size = new System.Drawing.Size(1001, 658);
            this.hswcMain.TabIndex = 4;
            this.hswcMain.WindowSize = new System.Drawing.Size(1001, 658);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.画一个矩形区域ToolStripMenuItem,
            this.保存模板ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
            // 
            // 画一个矩形区域ToolStripMenuItem
            // 
            this.画一个矩形区域ToolStripMenuItem.Name = "画一个矩形区域ToolStripMenuItem";
            this.画一个矩形区域ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.画一个矩形区域ToolStripMenuItem.Text = "画一个模板";
            this.画一个矩形区域ToolStripMenuItem.Click += new System.EventHandler(this.画一个矩形区域ToolStripMenuItem_Click_1);
            // 
            // 保存模板ToolStripMenuItem
            // 
            this.保存模板ToolStripMenuItem.Name = "保存模板ToolStripMenuItem";
            this.保存模板ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.保存模板ToolStripMenuItem.Text = "保存模板";
            this.保存模板ToolStripMenuItem.Click += new System.EventHandler(this.保存模板ToolStripMenuItem_Click);
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton7,
            this.toolStripButton1});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(1146, 25);
            this.darkToolStrip1.TabIndex = 3;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton7.Image = global::WaferAoi.Properties.Resources.debug;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton7.Tag = "加载本地模板";
            this.toolStripButton7.Text = "加载本地模板";
            this.toolStripButton7.ToolTipText = "加载本地模板";
            this.toolStripButton7.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1146, 686);
            this.panel3.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dsepMove);
            this.panel1.Controls.Add(this.hswcMain);
            this.panel1.Controls.Add(this.darkToolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1146, 686);
            this.panel1.TabIndex = 0;
            // 
            // dsepMove
            // 
            this.dsepMove.Controls.Add(this.btnSetStart);
            this.dsepMove.Controls.Add(this.tbStart);
            this.dsepMove.Controls.Add(this.darkLabel3);
            this.dsepMove.Controls.Add(this.tbVel);
            this.dsepMove.Controls.Add(this.darkLabel1);
            this.dsepMove.Controls.Add(this.tbDistance);
            this.dsepMove.Controls.Add(this.darkLabel2);
            this.dsepMove.Controls.Add(this.darkLabel9);
            this.dsepMove.Controls.Add(this.dbupVel);
            this.dsepMove.Controls.Add(this.darkGroupBox7);
            this.dsepMove.Dock = System.Windows.Forms.DockStyle.Right;
            this.dsepMove.Location = new System.Drawing.Point(1003, 25);
            this.dsepMove.Name = "dsepMove";
            this.dsepMove.SectionHeader = "移动";
            this.dsepMove.Size = new System.Drawing.Size(143, 661);
            this.dsepMove.TabIndex = 39;
            // 
            // darkLabel9
            // 
            this.darkLabel9.AutoSize = true;
            this.darkLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel9.Location = new System.Drawing.Point(12, 33);
            this.darkLabel9.Name = "darkLabel9";
            this.darkLabel9.Size = new System.Drawing.Size(33, 15);
            this.darkLabel9.TabIndex = 35;
            this.darkLabel9.Text = "速度";
            // 
            // dbupVel
            // 
            this.dbupVel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbupVel.DecimalPlaces = 2;
            this.dbupVel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.dbupVel.Location = new System.Drawing.Point(50, 29);
            this.dbupVel.Name = "dbupVel";
            this.dbupVel.Size = new System.Drawing.Size(89, 23);
            this.dbupVel.TabIndex = 34;
            this.dbupVel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // darkGroupBox7
            // 
            this.darkGroupBox7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox7.Controls.Add(this.button6);
            this.darkGroupBox7.Controls.Add(this.button2);
            this.darkGroupBox7.Controls.Add(this.button5);
            this.darkGroupBox7.Controls.Add(this.button7);
            this.darkGroupBox7.Controls.Add(this.button9);
            this.darkGroupBox7.Controls.Add(this.button8);
            this.darkGroupBox7.Controls.Add(this.button4);
            this.darkGroupBox7.Controls.Add(this.button3);
            this.darkGroupBox7.Controls.Add(this.button1);
            this.darkGroupBox7.Location = new System.Drawing.Point(11, 59);
            this.darkGroupBox7.Name = "darkGroupBox7";
            this.darkGroupBox7.Size = new System.Drawing.Size(120, 132);
            this.darkGroupBox7.TabIndex = 32;
            this.darkGroupBox7.TabStop = false;
            this.darkGroupBox7.Text = "移动";
            // 
            // button6
            // 
            this.button6.BackgroundImage = global::WaferAoi.Icons._106方向_左上;
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button6.Location = new System.Drawing.Point(10, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(30, 32);
            this.button6.TabIndex = 28;
            this.button6.Tag = "左上";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::WaferAoi.Icons._102方向_向下;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Location = new System.Drawing.Point(46, 95);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 32);
            this.button2.TabIndex = 24;
            this.button2.Tag = "向下";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button5
            // 
            this.button5.BackgroundImage = global::WaferAoi.Icons._100方向_向上;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.Location = new System.Drawing.Point(46, 19);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(30, 32);
            this.button5.TabIndex = 27;
            this.button5.Tag = "向上";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button7
            // 
            this.button7.BackgroundImage = global::WaferAoi.Icons._104方向_右上;
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.Location = new System.Drawing.Point(82, 19);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(30, 32);
            this.button7.TabIndex = 29;
            this.button7.Tag = "右上";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button9
            // 
            this.button9.BackgroundImage = global::WaferAoi.Icons._107方向_左下;
            this.button9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button9.Location = new System.Drawing.Point(10, 95);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(30, 32);
            this.button9.TabIndex = 31;
            this.button9.Tag = "左下";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Visible = false;
            this.button9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button9.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button8
            // 
            this.button8.BackgroundImage = global::WaferAoi.Icons._105方向_右下;
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button8.Location = new System.Drawing.Point(82, 95);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(30, 32);
            this.button8.TabIndex = 30;
            this.button8.Tag = "右下";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Visible = false;
            this.button8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::WaferAoi.Icons._101方向_向右;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.Location = new System.Drawing.Point(82, 57);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(30, 32);
            this.button4.TabIndex = 26;
            this.button4.Tag = "向右";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button3
            // 
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(46, 57);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 32);
            this.button3.TabIndex = 25;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::WaferAoi.Icons._103方向_向左;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(10, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 32);
            this.button1.TabIndex = 23;
            this.button1.Tag = "向左";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton1.Image = global::WaferAoi.Icons.Play;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(124, 22);
            this.toolStripButton1.Tag = "开始单行飞拍测试";
            this.toolStripButton1.Text = "开始单行飞拍测试";
            this.toolStripButton1.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // tbDistance
            // 
            this.tbDistance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbDistance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDistance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbDistance.Location = new System.Drawing.Point(8, 300);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(120, 23);
            this.tbDistance.TabIndex = 39;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(12, 282);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(79, 15);
            this.darkLabel2.TabIndex = 38;
            this.darkLabel2.Text = "X轴飞拍距离";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton2.Image = global::WaferAoi.Icons.smile;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton2.Tag = "加载本地配置";
            this.toolStripButton2.Text = "加载本地配置";
            this.toolStripButton2.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // tbVel
            // 
            this.tbVel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVel.Location = new System.Drawing.Point(11, 350);
            this.tbVel.Name = "tbVel";
            this.tbVel.Size = new System.Drawing.Size(120, 23);
            this.tbVel.TabIndex = 41;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(15, 332);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(79, 15);
            this.darkLabel1.TabIndex = 40;
            this.darkLabel1.Text = "X轴飞拍速度";
            // 
            // tbStart
            // 
            this.tbStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbStart.Location = new System.Drawing.Point(8, 256);
            this.tbStart.Name = "tbStart";
            this.tbStart.Size = new System.Drawing.Size(120, 23);
            this.tbStart.TabIndex = 43;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(12, 238);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(105, 15);
            this.darkLabel3.TabIndex = 42;
            this.darkLabel3.Text = "X轴飞拍起始位置";
            // 
            // btnSetStart
            // 
            this.btnSetStart.Location = new System.Drawing.Point(11, 198);
            this.btnSetStart.Name = "btnSetStart";
            this.btnSetStart.Padding = new System.Windows.Forms.Padding(5);
            this.btnSetStart.Size = new System.Drawing.Size(120, 23);
            this.btnSetStart.TabIndex = 44;
            this.btnSetStart.Tag = "设置当前位置开始";
            this.btnSetStart.Text = "设置当前位置开始";
            this.btnSetStart.Click += new System.EventHandler(this.btnSetStart_Click);
            // 
            // DialogFlyPhoto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 728);
            this.Controls.Add(this.panel3);
            this.DialogButtons = YiNing.UI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogFlyPhoto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片模型制作";
            this.Controls.SetChildIndex(this.panel3, 0);
            this.contextMenuStrip1.ResumeLayout(false);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.dsepMove.ResumeLayout(false);
            this.dsepMove.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbupVel)).EndInit();
            this.darkGroupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel3;
        private HalconDotNet.HSmartWindowControl hswcMain;
        private DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.Panel panel1;
        private DarkSectionPanel dsepMove;
        private DarkLabel darkLabel9;
        private DarkNumericUpDown dbupVel;
        private DarkGroupBox darkGroupBox7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 画一个矩形区域ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存模板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private DarkTextBox tbDistance;
        private DarkLabel darkLabel2;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private DarkTextBox tbVel;
        private DarkLabel darkLabel1;
        private DarkTextBox tbStart;
        private DarkLabel darkLabel3;
        private DarkButton btnSetStart;
    }
}