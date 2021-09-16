using HalconDotNet;
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
using YiNing.Tools;
using YiNing.WafermapDisplay.WafermapControl;

namespace WaferAoi
{
    public partial class DialogDetectResult : Form
    {
        Die[,] data;
        List<Die> visDie;
        string mapFilePath = "", visFilePath ="";
        InterLayerDraw ilMain;
        public DialogDetectResult()
        {
            InitializeComponent();
            this.Load += DialogDetectResult_Load;
        }
        public DialogDetectResult(string mapfilePath, string visfilePath):this()
        {
            this.mapFilePath = mapfilePath;
            this.visFilePath = visfilePath;
        }

        private void DialogDetectResult_Load(object sender, EventArgs e)
        {
            ilMain = new InterLayerDraw(hswcMain);
            darkSectionPanel1.Width = Convert.ToInt32(this.Width * 0.4);
            waferMap.SelectMultiple = false;
            waferMap.OnDieClick += WaferMap_OnDieClick;
            waferMap.Colors =  new Color[] { Color.DimGray, Color.Blue, Color.Red };

            if (mapFilePath == "") mapFilePath = @"D:\QTWaferProgram\test-final\mapping.zyn";
            data = JsonHelper.DeserializeByFile<Die[,]>(mapFilePath);

            waferMap.Dataset = data;
            waferMap.Notchlocation = 90;
            //wmap.MinimumSize = new Size(500, 500);
            waferMap.Dock = DockStyle.Fill;
            if (visFilePath != "")
            {
                visDie = JsonHelper.DeserializeByFile<List<Die>>(visFilePath);
                foreach (var die in visDie)
                {
                    waferMap.Dataset[die.XIndex, die.YIndex] = die;
                }
                waferMap.ReFresh();
            }

        }

        private void WaferMap_OnDieClick(object sender, Die e)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject img, e.GetImagePath());
                HOperatorSet.ReadRegion(out HObject region, e.GetHobjPath()+"");
                ilMain.ShowImg(img, region);
            }
            catch(Exception er)
            {
                HOperatorSet.ReadImage(out HObject img, @"D:\QTWaferExport\2021-09-15\123\img\3-41.jpg");
                HOperatorSet.ReadRegion(out HObject region, @"D:\QTWaferExport\2021-09-15\123\img\3-41.hobj");
                ilMain.ShowImg(img, region);
            }

            //e.
        }
    }
}
