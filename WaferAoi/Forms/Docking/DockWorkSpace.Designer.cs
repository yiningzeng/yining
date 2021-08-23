namespace WaferAoi
{
    partial class DockWorkSpace
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
            this.components = new System.ComponentModel.Container();
            this.darkSectionPanel6 = new YiNing.UI.Controls.DarkSectionPanel();
            this.isNegativePressure3 = new YiNing.UI.Controls.DarkPointsIn();
            this.isNegativePressure2 = new YiNing.UI.Controls.DarkPointsIn();
            this.isEmergencyStop = new YiNing.UI.Controls.DarkPointsIn();
            this.isNegativePressure1 = new YiNing.UI.Controls.DarkPointsIn();
            this.isStart = new YiNing.UI.Controls.DarkPointsIn();
            this.isPositivePressure = new YiNing.UI.Controls.DarkPointsIn();
            this.isReset = new YiNing.UI.Controls.DarkPointsIn();
            this.isDoor = new YiNing.UI.Controls.DarkPointsIn();
            this.isStop = new YiNing.UI.Controls.DarkPointsIn();
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.darkSectionPanel1 = new YiNing.UI.Controls.DarkSectionPanel();
            this.dlvwProgress = new YiNing.UI.Controls.DarkProgressReminder();
            this.darkButton1 = new YiNing.UI.Controls.DarkButton();
            this.darkButton2 = new YiNing.UI.Controls.DarkButton();
            this.darkButton3 = new YiNing.UI.Controls.DarkButton();
            this.darkButton4 = new YiNing.UI.Controls.DarkButton();
            this.darkButton5 = new YiNing.UI.Controls.DarkButton();
            this.waferMap = new YiNing.WafermapDisplay.Wafermap();
            this.darkSectionPanel6.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkSectionPanel6
            // 
            this.darkSectionPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkSectionPanel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkSectionPanel6.Controls.Add(this.isNegativePressure3);
            this.darkSectionPanel6.Controls.Add(this.isNegativePressure2);
            this.darkSectionPanel6.Controls.Add(this.isEmergencyStop);
            this.darkSectionPanel6.Controls.Add(this.isNegativePressure1);
            this.darkSectionPanel6.Controls.Add(this.isStart);
            this.darkSectionPanel6.Controls.Add(this.isPositivePressure);
            this.darkSectionPanel6.Controls.Add(this.isReset);
            this.darkSectionPanel6.Controls.Add(this.isDoor);
            this.darkSectionPanel6.Controls.Add(this.isStop);
            this.darkSectionPanel6.DragEnable = true;
            this.darkSectionPanel6.Location = new System.Drawing.Point(693, 10);
            this.darkSectionPanel6.Name = "darkSectionPanel6";
            this.darkSectionPanel6.SectionHeader = "信号监控";
            this.darkSectionPanel6.Size = new System.Drawing.Size(152, 156);
            this.darkSectionPanel6.TabIndex = 19;
            // 
            // isNegativePressure3
            // 
            this.isNegativePressure3.AutoSize = true;
            this.isNegativePressure3.CheckedColor = System.Drawing.Color.Lime;
            this.isNegativePressure3.Enabled = true;
            this.isNegativePressure3.Location = new System.Drawing.Point(62, 104);
            this.isNegativePressure3.Name = "isNegativePressure3";
            this.isNegativePressure3.Size = new System.Drawing.Size(84, 19);
            this.isNegativePressure3.TabIndex = 8;
            this.isNegativePressure3.Text = "负压检测3";
            this.isNegativePressure3.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isNegativePressure2
            // 
            this.isNegativePressure2.AutoSize = true;
            this.isNegativePressure2.CheckedColor = System.Drawing.Color.Lime;
            this.isNegativePressure2.Enabled = true;
            this.isNegativePressure2.Location = new System.Drawing.Point(62, 79);
            this.isNegativePressure2.Name = "isNegativePressure2";
            this.isNegativePressure2.Size = new System.Drawing.Size(84, 19);
            this.isNegativePressure2.TabIndex = 7;
            this.isNegativePressure2.Text = "负压检测2";
            this.isNegativePressure2.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isEmergencyStop
            // 
            this.isEmergencyStop.AutoSize = true;
            this.isEmergencyStop.CheckedColor = System.Drawing.Color.Lime;
            this.isEmergencyStop.Enabled = true;
            this.isEmergencyStop.Location = new System.Drawing.Point(4, 29);
            this.isEmergencyStop.Name = "isEmergencyStop";
            this.isEmergencyStop.Size = new System.Drawing.Size(52, 19);
            this.isEmergencyStop.TabIndex = 0;
            this.isEmergencyStop.Text = "急停";
            this.isEmergencyStop.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isNegativePressure1
            // 
            this.isNegativePressure1.AutoSize = true;
            this.isNegativePressure1.CheckedColor = System.Drawing.Color.Lime;
            this.isNegativePressure1.Enabled = true;
            this.isNegativePressure1.Location = new System.Drawing.Point(62, 54);
            this.isNegativePressure1.Name = "isNegativePressure1";
            this.isNegativePressure1.Size = new System.Drawing.Size(84, 19);
            this.isNegativePressure1.TabIndex = 6;
            this.isNegativePressure1.Text = "负压检测1";
            this.isNegativePressure1.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isStart
            // 
            this.isStart.AutoSize = true;
            this.isStart.CheckedColor = System.Drawing.Color.Lime;
            this.isStart.Enabled = true;
            this.isStart.Location = new System.Drawing.Point(4, 54);
            this.isStart.Name = "isStart";
            this.isStart.Size = new System.Drawing.Size(52, 19);
            this.isStart.TabIndex = 1;
            this.isStart.Text = "启动";
            this.isStart.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isPositivePressure
            // 
            this.isPositivePressure.AutoSize = true;
            this.isPositivePressure.CheckedColor = System.Drawing.Color.Lime;
            this.isPositivePressure.Enabled = true;
            this.isPositivePressure.Location = new System.Drawing.Point(62, 29);
            this.isPositivePressure.Name = "isPositivePressure";
            this.isPositivePressure.Size = new System.Drawing.Size(78, 19);
            this.isPositivePressure.TabIndex = 5;
            this.isPositivePressure.Text = "正压检测";
            this.isPositivePressure.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isReset
            // 
            this.isReset.AutoSize = true;
            this.isReset.CheckedColor = System.Drawing.Color.Lime;
            this.isReset.Enabled = true;
            this.isReset.Location = new System.Drawing.Point(4, 79);
            this.isReset.Name = "isReset";
            this.isReset.Size = new System.Drawing.Size(52, 19);
            this.isReset.TabIndex = 2;
            this.isReset.Text = "复位";
            this.isReset.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isDoor
            // 
            this.isDoor.AutoSize = true;
            this.isDoor.CheckedColor = System.Drawing.Color.Lime;
            this.isDoor.Enabled = true;
            this.isDoor.Location = new System.Drawing.Point(4, 129);
            this.isDoor.Name = "isDoor";
            this.isDoor.Size = new System.Drawing.Size(52, 19);
            this.isDoor.TabIndex = 4;
            this.isDoor.Text = "门禁";
            this.isDoor.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // isStop
            // 
            this.isStop.AutoSize = true;
            this.isStop.CheckedColor = System.Drawing.Color.Lime;
            this.isStop.Enabled = true;
            this.isStop.Location = new System.Drawing.Point(4, 104);
            this.isStop.Name = "isStop";
            this.isStop.Size = new System.Drawing.Size(52, 19);
            this.isStop.TabIndex = 3;
            this.isStop.Text = "停止";
            this.isStop.UnCheckedColor = System.Drawing.Color.Red;
            // 
            // timerCheck
            // 
            this.timerCheck.Interval = 150;
            this.timerCheck.Tick += new System.EventHandler(this.timerCheck_Tick);
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.dlvwProgress);
            this.darkSectionPanel1.DragEnable = true;
            this.darkSectionPanel1.Location = new System.Drawing.Point(10, 10);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "进度提醒";
            this.darkSectionPanel1.Size = new System.Drawing.Size(170, 259);
            this.darkSectionPanel1.TabIndex = 20;
            // 
            // dlvwProgress
            // 
            this.dlvwProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dlvwProgress.Location = new System.Drawing.Point(1, 25);
            this.dlvwProgress.Name = "dlvwProgress";
            this.dlvwProgress.Size = new System.Drawing.Size(168, 233);
            this.dlvwProgress.TabIndex = 0;
            this.dlvwProgress.Text = "darkProgressReminder1";
            // 
            // darkButton1
            // 
            this.darkButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.darkButton1.Location = new System.Drawing.Point(186, 35);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 23);
            this.darkButton1.TabIndex = 21;
            this.darkButton1.Text = "Next";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(186, 6);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(75, 23);
            this.darkButton2.TabIndex = 22;
            this.darkButton2.Text = "go to 1";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(268, 5);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(75, 23);
            this.darkButton3.TabIndex = 23;
            this.darkButton3.Text = "出错啦";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // darkButton4
            // 
            this.darkButton4.Location = new System.Drawing.Point(268, 34);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(75, 23);
            this.darkButton4.TabIndex = 24;
            this.darkButton4.Text = "停止了";
            this.darkButton4.Click += new System.EventHandler(this.darkButton4_Click);
            // 
            // darkButton5
            // 
            this.darkButton5.Location = new System.Drawing.Point(593, 5);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton5.Size = new System.Drawing.Size(75, 23);
            this.darkButton5.TabIndex = 25;
            this.darkButton5.Text = "darkButton5";
            this.darkButton5.Click += new System.EventHandler(this.darkButton5_Click);
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
            this.waferMap.Interactive = false;
            this.waferMap.Location = new System.Drawing.Point(0, 0);
            this.waferMap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.waferMap.Name = "waferMap";
            this.waferMap.NoDataString = "NO DATA";
            this.waferMap.Notchlocation = 0;
            this.waferMap.Rotation = 0;
            this.waferMap.SelectDies = null;
            this.waferMap.SelectOneDie = null;
            this.waferMap.SelectX = 0;
            this.waferMap.SelectY = 0;
            this.waferMap.Size = new System.Drawing.Size(858, 533);
            this.waferMap.TabIndex = 0;
            this.waferMap.TooSmallString = "TOO SMALL";
            this.waferMap.TranslationX = 0;
            this.waferMap.TranslationY = 0;
            this.waferMap.Zoom = 1F;
            // 
            // DockWorkSpace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkButton5);
            this.Controls.Add(this.darkButton4);
            this.Controls.Add(this.darkButton3);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.darkSectionPanel1);
            this.Controls.Add(this.darkSectionPanel6);
            this.Controls.Add(this.waferMap);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DockWorkSpace";
            this.Size = new System.Drawing.Size(858, 533);
            this.darkSectionPanel6.ResumeLayout(false);
            this.darkSectionPanel6.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private YiNing.UI.Controls.DarkSectionPanel darkSectionPanel6;
        private YiNing.UI.Controls.DarkPointsIn isNegativePressure3;
        private YiNing.UI.Controls.DarkPointsIn isNegativePressure2;
        private YiNing.UI.Controls.DarkPointsIn isNegativePressure1;
        private YiNing.UI.Controls.DarkPointsIn isPositivePressure;
        private YiNing.UI.Controls.DarkPointsIn isDoor;
        private YiNing.UI.Controls.DarkPointsIn isStop;
        private YiNing.UI.Controls.DarkPointsIn isReset;
        private YiNing.UI.Controls.DarkPointsIn isStart;
        private YiNing.UI.Controls.DarkPointsIn isEmergencyStop;
        private System.Windows.Forms.Timer timerCheck;
        private YiNing.UI.Controls.DarkSectionPanel darkSectionPanel1;
        public YiNing.WafermapDisplay.Wafermap waferMap;
        private YiNing.UI.Controls.DarkProgressReminder dlvwProgress;
        private YiNing.UI.Controls.DarkButton darkButton1;
        private YiNing.UI.Controls.DarkButton darkButton2;
        private YiNing.UI.Controls.DarkButton darkButton3;
        private YiNing.UI.Controls.DarkButton darkButton4;
        private YiNing.UI.Controls.DarkButton darkButton5;
    }
}
