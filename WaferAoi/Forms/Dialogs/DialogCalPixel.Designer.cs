using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogCalPixel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogCalPixel));
            this.hswcMain = new HalconDotNet.HSmartWindowControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.画一个矩形区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStrip1 = new YiNing.UI.Controls.DarkToolStrip();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dsepMove = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkGroupBox1 = new YiNing.UI.Controls.DarkGroupBox();
            this.darkLabel3 = new YiNing.UI.Controls.DarkLabel();
            this.tbAvg = new YiNing.UI.Controls.DarkTextBox();
            this.tbFinalX = new YiNing.UI.Controls.DarkTextBox();
            this.tbFinalY = new YiNing.UI.Controls.DarkTextBox();
            this.darkLabel1 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel2 = new YiNing.UI.Controls.DarkLabel();
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.darkListView1 = new YiNing.UI.Controls.DarkListView();
            this.darkButton3 = new YiNing.UI.Controls.DarkButton();
            this.darkButton1 = new YiNing.UI.Controls.DarkButton();
            this.darkButton2 = new YiNing.UI.Controls.DarkButton();
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
            this.contextMenuStrip1.SuspendLayout();
            this.darkToolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.dsepMove.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
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
            this.hswcMain.Size = new System.Drawing.Size(932, 658);
            this.hswcMain.TabIndex = 4;
            this.hswcMain.WindowSize = new System.Drawing.Size(932, 658);
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
            this.toolStripButton7,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
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
            this.toolStripButton7.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton7.Tag = "计算一组";
            this.toolStripButton7.Text = "计算一组";
            this.toolStripButton7.ToolTipText = "计算一组";
            this.toolStripButton7.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton1.Image = global::WaferAoi.Icons.properties_16xLG;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(136, 22);
            this.toolStripButton1.Tag = "去掉最低最高求平均";
            this.toolStripButton1.Text = "去掉最低最高求平均";
            this.toolStripButton1.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton2.Image = global::WaferAoi.Icons.Cup;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton2.Tag = "清空";
            this.toolStripButton2.Text = "清空";
            this.toolStripButton2.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolStripButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButton3.Image = global::WaferAoi.Properties.Resources.Save;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton3.Tag = "保存最终结果";
            this.toolStripButton3.Text = "保存最终结果";
            this.toolStripButton3.ToolTipText = "保存最终结果";
            this.toolStripButton3.Click += new System.EventHandler(this.DarkButtonClick);
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
            this.dsepMove.Controls.Add(this.darkGroupBox1);
            this.dsepMove.Controls.Add(this.darkSectionPanel1);
            this.dsepMove.Controls.Add(this.darkLabel9);
            this.dsepMove.Controls.Add(this.dbupVel);
            this.dsepMove.Controls.Add(this.darkGroupBox7);
            this.dsepMove.Dock = System.Windows.Forms.DockStyle.Right;
            this.dsepMove.Location = new System.Drawing.Point(935, 25);
            this.dsepMove.Name = "dsepMove";
            this.dsepMove.SectionHeader = "移动";
            this.dsepMove.Size = new System.Drawing.Size(211, 661);
            this.dsepMove.TabIndex = 39;
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkLabel3);
            this.darkGroupBox1.Controls.Add(this.tbAvg);
            this.darkGroupBox1.Controls.Add(this.tbFinalX);
            this.darkGroupBox1.Controls.Add(this.tbFinalY);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Location = new System.Drawing.Point(4, 197);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(162, 111);
            this.darkGroupBox1.TabIndex = 44;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "最终像元";
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(1, 82);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(29, 15);
            this.darkLabel3.TabIndex = 45;
            this.darkLabel3.Text = "avg:";
            // 
            // tbAvg
            // 
            this.tbAvg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbAvg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbAvg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbAvg.Location = new System.Drawing.Point(36, 80);
            this.tbAvg.Name = "tbAvg";
            this.tbAvg.Size = new System.Drawing.Size(118, 23);
            this.tbAvg.TabIndex = 44;
            // 
            // tbFinalX
            // 
            this.tbFinalX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbFinalX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFinalX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbFinalX.Location = new System.Drawing.Point(36, 22);
            this.tbFinalX.Name = "tbFinalX";
            this.tbFinalX.Size = new System.Drawing.Size(118, 23);
            this.tbFinalX.TabIndex = 42;
            // 
            // tbFinalY
            // 
            this.tbFinalY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbFinalY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFinalY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbFinalY.Location = new System.Drawing.Point(36, 51);
            this.tbFinalY.Name = "tbFinalY";
            this.tbFinalY.Size = new System.Drawing.Size(118, 23);
            this.tbFinalY.TabIndex = 43;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(13, 24);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(17, 15);
            this.darkLabel1.TabIndex = 40;
            this.darkLabel1.Text = "X:";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(13, 53);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(17, 15);
            this.darkLabel2.TabIndex = 41;
            this.darkLabel2.Text = "Y:";
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.darkSectionPanel1.Controls.Add(this.darkListView1);
            this.darkSectionPanel1.Controls.Add(this.darkButton3);
            this.darkSectionPanel1.Controls.Add(this.darkButton1);
            this.darkSectionPanel1.Controls.Add(this.darkButton2);
            this.darkSectionPanel1.Location = new System.Drawing.Point(0, 306);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "像元列表";
            this.darkSectionPanel1.Size = new System.Drawing.Size(211, 355);
            this.darkSectionPanel1.TabIndex = 37;
            // 
            // darkListView1
            // 
            this.darkListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkListView1.Location = new System.Drawing.Point(1, 25);
            this.darkListView1.Name = "darkListView1";
            this.darkListView1.Size = new System.Drawing.Size(209, 329);
            this.darkListView1.TabIndex = 0;
            this.darkListView1.Text = "darkListView1";
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(9, 148);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(133, 23);
            this.darkButton3.TabIndex = 39;
            this.darkButton3.Tag = "清空";
            this.darkButton3.Text = "清空";
            this.darkButton3.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(9, 90);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(133, 23);
            this.darkButton1.TabIndex = 36;
            this.darkButton1.Tag = "计算一组";
            this.darkButton1.Text = "计算一组";
            this.darkButton1.Click += new System.EventHandler(this.DarkButtonClick);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(9, 119);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(133, 23);
            this.darkButton2.TabIndex = 38;
            this.darkButton2.Tag = "去掉最低最高求平均";
            this.darkButton2.Text = "去掉最低最高求平均";
            this.darkButton2.Click += new System.EventHandler(this.DarkButtonClick);
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
            this.dbupVel.Size = new System.Drawing.Size(157, 23);
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
            // DialogCalPixel
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
            this.Name = "DialogCalPixel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片模型制作";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.SetChildIndex(this.panel3, 0);
            this.contextMenuStrip1.ResumeLayout(false);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.dsepMove.ResumeLayout(false);
            this.dsepMove.PerformLayout();
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
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
        private DarkButton darkButton1;
        private DarkSectionPanel darkSectionPanel1;
        private DarkButton darkButton2;
        private DarkListView darkListView1;
        private DarkButton darkButton3;
        private DarkLabel darkLabel1;
        private DarkLabel darkLabel2;
        private DarkTextBox tbFinalY;
        private DarkTextBox tbFinalX;
        private DarkGroupBox darkGroupBox1;
        private System.Windows.Forms.ToolStripMenuItem 保存模板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private DarkLabel darkLabel3;
        private DarkTextBox tbAvg;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}