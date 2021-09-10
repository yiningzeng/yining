using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using WaferAoi.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using YiNing.WafermapDisplay.WafermapControl;
using HalconDotNet;

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
            waferMap.OnDieClick += WaferMap_OnDieClick;
            dlvwProgress.Items.Add(new DarkListItem("空闲") { Icon = Icons.进度指向, TextColor = Color.Chocolate });
            for (int waferNum = 0; waferNum < 1; waferNum++)
            {
                // Create sample dataset
                Die[,] data = new Die[41, 40];
                Random binGenerator = new Random(waferNum);
                for (int x = 0; x < 41; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        data[x, y] = new Die() { ColorIndex = binGenerator.Next(8), XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    }
                }
                int a = data.Length;
                waferMap.Dataset = data;
                waferMap.Notchlocation = 90;
                //wmap.MinimumSize = new Size(500, 500);
                waferMap.Dock = DockStyle.Fill;
                //waferMap.SelectX = 21;
                //waferMap.SelectY = 14;
                //waferMap.SelectOneDie = data[21, 14];

                waferMap.SelectRegionDiagonalDie = new Die[] { new Die() { XIndex = 10, YIndex = 10 }, new Die() { XIndex = 21, YIndex = 23 } };
                //wmap.NoDataString = "没有数据";
                //this.Controls.Add(wmap);
            }
        }

        private void WaferMap_OnDieClick(object sender, Die e)
        {
        }

        public void SetProgress()
        {
            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("加载图谱"));
            dlvwProgress.Items.Add(new DarkListItem("放料"));
            dlvwProgress.Items.Add(new DarkListItem("定位晶圆切面/凹槽"));
            dlvwProgress.Items.Add(new DarkListItem("方向矫正"));
            dlvwProgress.Items.Add(new DarkListItem("检测中"));
            dlvwProgress.Items.Add(new DarkListItem("检测完毕"));
            dlvwProgress.Items.Add(new DarkListItem("数据导出"));
            dlvwProgress.Items.Add(new DarkListItem("取料"));
            dlvwProgress.Items.Add(new DarkListItem("空闲"));
            dlvwProgress.SetStartNum(0);
            waferMap.DieAlpha = 0;
            Task.Run(() =>
            {
                int i = 0;
                while (i < 9)
                {
                    i++;
                    Thread.Sleep(1500);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (i <= 2) waferMap.DieAlpha = 150;
                        dlvwProgress.Next();
                        if (dlvwProgress.IsDone()) i = 10;
                    }));
                }
            });
        }

        public DockWorkSpace(MainForm main, string text, Image icon) : this()
        {
            this.main = main;
            DockText = text;
            Icon = icon;
            this.Load += DockWorkSpace_Load;
        }

        private void DockWorkSpace_Load(object sender, EventArgs e)
        {
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

                //if (!isEmergencyStop.Checked) main.fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_SCRAM);
                //if (!isReset.Checked) main.fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_INIT_RESET);
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

        private void darkButton1_Click(object sender, EventArgs e)
        {
            dlvwProgress.Next();
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            dlvwProgress.SetStartNum(1);
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            dlvwProgress.Error();
            //waferMap.DieAlpha = 0;
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            dlvwProgress.Stop();
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            Config config = FsmHelper.GetConfig();
            Axis ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            Axis ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "1":
                    MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 50, 100000, false);
                        break;

                case "2":
                    MotorsControl.MoveJog(ax.Id, ax.JogPrm.Get(), 20);
                    Thread.Sleep(1000);
                    MotorsControl.StopAxis(ax.Id, 0);
                    break;
            
            }
         }
    }
}
