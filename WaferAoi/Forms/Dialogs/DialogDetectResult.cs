using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.WafermapDisplay.WafermapControl;

namespace WaferAoi
{
    public partial class DialogDetectResult : Form
    {
        InterLayerDraw ilMain;
        public DialogDetectResult()
        {
            InitializeComponent();
            this.Load += DialogDetectResult_Load;
        }

        private void DialogDetectResult_Load(object sender, EventArgs e)
        {
            ilMain = new InterLayerDraw(hswcMain);
            darkSectionPanel1.Width = Convert.ToInt32(this.Width * 0.4);
            waferMap.SelectMultiple = false;
            waferMap.OnDieClick += WaferMap_OnDieClick;
            waferMap.Colors = new Color[] {Color.Green, Color.Red };
            Die[,] data = new Die[41, 40];
            for (int x = 0; x < 41; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    if (y > 18 && y < 21 && x >= 2 && x < 41 - 2)
                    {
                        data[x, y] = new Die() { ColorIndex = 1, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                    }
                    else data[x, y] = new Die() { ColorIndex = 0, XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
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

            //waferMap.SelectRegionDiagonalDie = new Die[] { new Die() { XIndex = 10, YIndex = 10 }, new Die() { XIndex = 21, YIndex = 23 } };
            //wmap.NoDataString = "没有数据";
            //this.Controls.Add(wmap);
        }

        private void WaferMap_OnDieClick(object sender, Die e)
        {
            ilMain.ShowImg("D:/2222.jpg");
            //e.
        }
    }
}
