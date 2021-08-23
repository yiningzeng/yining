using YiNing.UI.Controls;
using YiNing.UI.Docking;

namespace WaferAoi
{
    partial class DockControl
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
            this.darkGroupBox1 = new YiNing.UI.Controls.DarkGroupBox();
            this.btnRobot = new System.Windows.Forms.Button();
            this.darkGroupBox2 = new YiNing.UI.Controls.DarkGroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.darkGroupBox1.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.btnRobot);
            this.darkGroupBox1.Location = new System.Drawing.Point(3, 28);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(381, 213);
            this.darkGroupBox1.TabIndex = 0;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "自动取放晶圆";
            // 
            // btnRobot
            // 
            this.btnRobot.Location = new System.Drawing.Point(300, 22);
            this.btnRobot.Name = "btnRobot";
            this.btnRobot.Size = new System.Drawing.Size(75, 185);
            this.btnRobot.TabIndex = 0;
            this.btnRobot.Text = "开始检测";
            this.btnRobot.UseVisualStyleBackColor = true;
            this.btnRobot.Click += new System.EventHandler(this.btnRobot_Click);
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Controls.Add(this.button2);
            this.darkGroupBox2.Location = new System.Drawing.Point(390, 28);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(381, 213);
            this.darkGroupBox2.TabIndex = 1;
            this.darkGroupBox2.TabStop = false;
            this.darkGroupBox2.Text = "手动取放晶圆";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(290, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 185);
            this.button2.TabIndex = 1;
            this.button2.Text = "开始检测";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // DockControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkGroupBox2);
            this.Controls.Add(this.darkGroupBox1);
            this.DefaultDockArea = YiNing.UI.Docking.DarkDockArea.Bottom;
            this.DockText = "控制台";
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::WaferAoi.Icons.Console;
            this.Name = "DockControl";
            this.SerializationKey = "DockConsole";
            this.Size = new System.Drawing.Size(781, 254);
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox darkGroupBox1;
        private DarkGroupBox darkGroupBox2;
        private System.Windows.Forms.Button btnRobot;
        private System.Windows.Forms.Button button2;
    }
}
