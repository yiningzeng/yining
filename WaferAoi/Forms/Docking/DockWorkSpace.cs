using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace WaferAoi
{
    public partial class DockWorkSpace : DarkDocument
    {
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

        public DockWorkSpace(string text, Image icon):this()
        {
            DockText = text;
            Icon = icon;
        }

        #endregion

        #region Event Handler Region

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
