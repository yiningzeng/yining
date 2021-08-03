using YiNing.UI.Controls;

namespace WaferAoi
{
    partial class DialogGoHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogGoHome));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.darkInputSignal2 = new YiNing.UI.Controls.DarkInputSignal();
            this.darkInputSignal1 = new YiNing.UI.Controls.DarkInputSignal();
            this.lbTargetPos = new YiNing.UI.Controls.DarkLabel();
            this.lbCapturePos = new YiNing.UI.Controls.DarkLabel();
            this.lbError = new YiNing.UI.Controls.DarkLabel();
            this.lbStage = new YiNing.UI.Controls.DarkLabel();
            this.lbStatus = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel25 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel24 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel23 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel22 = new YiNing.UI.Controls.DarkLabel();
            this.darkLabel21 = new YiNing.UI.Controls.DarkLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.darkInputSignal2);
            this.pnlMain.Controls.Add(this.darkInputSignal1);
            this.pnlMain.Controls.Add(this.lbTargetPos);
            this.pnlMain.Controls.Add(this.lbCapturePos);
            this.pnlMain.Controls.Add(this.lbError);
            this.pnlMain.Controls.Add(this.lbStage);
            this.pnlMain.Controls.Add(this.lbStatus);
            this.pnlMain.Controls.Add(this.darkLabel25);
            this.pnlMain.Controls.Add(this.darkLabel24);
            this.pnlMain.Controls.Add(this.darkLabel23);
            this.pnlMain.Controls.Add(this.darkLabel22);
            this.pnlMain.Controls.Add(this.darkLabel21);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(302, 193);
            this.pnlMain.TabIndex = 2;
            // 
            // darkInputSignal2
            // 
            this.darkInputSignal2.AutoSize = true;
            this.darkInputSignal2.Location = new System.Drawing.Point(92, 163);
            this.darkInputSignal2.Name = "darkInputSignal2";
            this.darkInputSignal2.Size = new System.Drawing.Size(65, 19);
            this.darkInputSignal2.TabIndex = 21;
            this.darkInputSignal2.Text = "负限位";
            // 
            // darkInputSignal1
            // 
            this.darkInputSignal1.AutoSize = true;
            this.darkInputSignal1.Location = new System.Drawing.Point(21, 163);
            this.darkInputSignal1.Name = "darkInputSignal1";
            this.darkInputSignal1.Size = new System.Drawing.Size(65, 19);
            this.darkInputSignal1.TabIndex = 20;
            this.darkInputSignal1.Text = "正限位";
            // 
            // lbTargetPos
            // 
            this.lbTargetPos.AutoSize = true;
            this.lbTargetPos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTargetPos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbTargetPos.Location = new System.Drawing.Point(105, 128);
            this.lbTargetPos.Name = "lbTargetPos";
            this.lbTargetPos.Size = new System.Drawing.Size(67, 15);
            this.lbTargetPos.TabIndex = 19;
            this.lbTargetPos.Tag = "回零-目标位置";
            this.lbTargetPos.Text = "                    ";
            // 
            // lbCapturePos
            // 
            this.lbCapturePos.AutoSize = true;
            this.lbCapturePos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCapturePos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbCapturePos.Location = new System.Drawing.Point(105, 100);
            this.lbCapturePos.Name = "lbCapturePos";
            this.lbCapturePos.Size = new System.Drawing.Size(67, 15);
            this.lbCapturePos.TabIndex = 18;
            this.lbCapturePos.Tag = "回零-捕获位置";
            this.lbCapturePos.Text = "                    ";
            // 
            // lbError
            // 
            this.lbError.AutoSize = true;
            this.lbError.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbError.ForeColor = System.Drawing.Color.Red;
            this.lbError.Location = new System.Drawing.Point(106, 78);
            this.lbError.Name = "lbError";
            this.lbError.Size = new System.Drawing.Size(113, 11);
            this.lbError.TabIndex = 17;
            this.lbError.Tag = "回零-错误提示";
            this.lbError.Text = "                  ";
            // 
            // lbStage
            // 
            this.lbStage.AutoSize = true;
            this.lbStage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbStage.Location = new System.Drawing.Point(105, 48);
            this.lbStage.Name = "lbStage";
            this.lbStage.Size = new System.Drawing.Size(67, 15);
            this.lbStage.TabIndex = 16;
            this.lbStage.Tag = "回零-当前阶段";
            this.lbStage.Text = "                    ";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbStatus.Location = new System.Drawing.Point(105, 24);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(72, 15);
            this.lbStatus.TabIndex = 15;
            this.lbStatus.Tag = "回零-运动状态";
            this.lbStatus.Text = "回零未启动";
            // 
            // darkLabel25
            // 
            this.darkLabel25.AutoSize = true;
            this.darkLabel25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel25.Location = new System.Drawing.Point(18, 128);
            this.darkLabel25.Name = "darkLabel25";
            this.darkLabel25.Size = new System.Drawing.Size(72, 15);
            this.darkLabel25.TabIndex = 14;
            this.darkLabel25.Text = "目标位置：";
            // 
            // darkLabel24
            // 
            this.darkLabel24.AutoSize = true;
            this.darkLabel24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel24.Location = new System.Drawing.Point(18, 100);
            this.darkLabel24.Name = "darkLabel24";
            this.darkLabel24.Size = new System.Drawing.Size(72, 15);
            this.darkLabel24.TabIndex = 13;
            this.darkLabel24.Text = "捕获位置：";
            // 
            // darkLabel23
            // 
            this.darkLabel23.AutoSize = true;
            this.darkLabel23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel23.Location = new System.Drawing.Point(18, 75);
            this.darkLabel23.Name = "darkLabel23";
            this.darkLabel23.Size = new System.Drawing.Size(72, 15);
            this.darkLabel23.TabIndex = 12;
            this.darkLabel23.Text = "错误提示：";
            // 
            // darkLabel22
            // 
            this.darkLabel22.AutoSize = true;
            this.darkLabel22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel22.Location = new System.Drawing.Point(18, 48);
            this.darkLabel22.Name = "darkLabel22";
            this.darkLabel22.Size = new System.Drawing.Size(72, 15);
            this.darkLabel22.TabIndex = 11;
            this.darkLabel22.Text = "当前阶段：";
            // 
            // darkLabel21
            // 
            this.darkLabel21.AutoSize = true;
            this.darkLabel21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel21.Location = new System.Drawing.Point(18, 24);
            this.darkLabel21.Name = "darkLabel21";
            this.darkLabel21.Size = new System.Drawing.Size(72, 15);
            this.darkLabel21.TabIndex = 10;
            this.darkLabel21.Text = "运动状态：";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DialogGoHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 235);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(318, 274);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(318, 274);
            this.Name = "DialogGoHome";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "回零状态";
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DarkLabel lbTargetPos;
        private DarkLabel lbCapturePos;
        private DarkLabel lbError;
        private DarkLabel lbStage;
        private DarkLabel lbStatus;
        private DarkLabel darkLabel25;
        private DarkLabel darkLabel24;
        private DarkLabel darkLabel23;
        private DarkLabel darkLabel22;
        private DarkLabel darkLabel21;
        private DarkInputSignal darkInputSignal2;
        private DarkInputSignal darkInputSignal1;
        private System.Windows.Forms.Timer timer1;
    }
}