using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using WaferAoi.Tools;
using System.Collections.Generic;

namespace WaferAoi
{
    public partial class DockWorkSpace : DarkDocument
    {
        #region Field Region
        private MainForm main;
        #endregion

        #region Constructor Region

        public DockWorkSpace()
        {
            InitializeComponent();
            for (int waferNum = 0; waferNum < 1; waferNum++)
            {
                // Create sample dataset
                int[,] data = new int[41, 40];
                Random binGenerator = new Random(waferNum);
                for (int x = 0; x < 41; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        data[x, y] = binGenerator.Next(8);
                    }
                }
                int a = data.Length;
                waferMap.Dataset = data;
                waferMap.Notchlocation = 0;
                //wmap.MinimumSize = new Size(500, 500);
                waferMap.Interactive = true;
                waferMap.Dock = DockStyle.Fill;
                waferMap.SelectX = 21;
                waferMap.SelectY = 14;
                waferMap.SelectBincode = data[21, 14];
                //wmap.NoDataString = "没有数据";
                //this.Controls.Add(wmap);
            }
        }

        public DockWorkSpace(MainForm main, string text, Image icon) : this()
        {
            this.main = main;
            DockText = text;
            Icon = icon;
            this.timerCheck.Start();
        }
        #endregion

        #region Event Handler Region
        private void timerCheck_Tick(object sender, EventArgs e)
        {
            int exi = MotorsControl.IoSignalEXI(4);
            this.BeginInvoke(new Action<int>((exiSts) =>
            {
           
                isEmergencyStop.Checked = (exi & (1 << 0)) == 0;
                isStart.Checked = (exi & (1 << 1)) != 0;
                isReset.Checked = (exi & (1 << 2)) != 0;
                isStop.Checked = (exi & (1 << 3)) != 0;
                isDoor.Checked = (exi & (1 << 4)) != 0;
                isPositivePressure.Checked = (exi & (1 << 7)) != 0;
                isNegativePressure1.Checked = (exi & (1 << 8)) != 0;
                isNegativePressure2.Checked = (exi & (1 << 9)) != 0;
                isNegativePressure3.Checked = (exi & (1 << 10)) != 0;
            }), exi);
        }
        public override void Close()
        {
            var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;

            base.Close();
        }


        #endregion
    }
}
