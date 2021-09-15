using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiNing.WafermapDisplay.WafermapControl;

namespace WaferAoi.Tools
{    /// <summary>
     /// 物镜
     /// </summary>
    public enum ObjectiveLense
    {
        [Description("5A A5 06 83 10 03 01 00 01 9E")]
        X1,
        [Description("5A A5 06 83 10 03 01 00 02 9F")]
        X2,
        [Description("5A A5 06 83 10 03 01 00 03 A0")]
        X5,
        [Description("5A A5 06 83 10 03 01 00 04 A1")]
        X10,
        [Description("5A A5 06 83 10 03 01 00 05 A2")]
        X20,
    }
    public enum WaferSize : int
    {
        INCH6 = 0, // 6寸
        INCH8 = 1 // 8寸
    }
    public enum WaferType : int
    {
        PATTERNED = 0, //图案晶圆
        BARE = 1 // 无图案晶圆
    }
    public enum TraitType : int
    {
        Notch = 0,
        Flat = 1 
    }
    public enum TraitLocation : int
    {
        LEFT = 0,
        RIGHT = 1,
        TOP = 2,
        BOTTOM = 3,
    }
    public enum HaveRingPiece : int
    {
        No = 0, // 不带环片
        Yes = 1, // 带环片
    }
    public class Inch
    {
        /// <summary>
        /// 尺寸
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 圆心定位中心用到的 固定右边的点，单位脉冲
        /// </summary>
        public Point RightPoint { get; set; }
        /// <summary>
        /// 晶圆定位中心需要用到的直径
        /// </summary>
        public int Diameter { get; set; }
    }


    public class ChipModel
    {
        /// <summary>
        /// 矫正的间隔FOV
        /// </summary>
        [Description("矫正间隔")]
        public int CorrectInterval { get; set; }
        /// <summary>
        /// 创建模型的时候中心坐标
        /// </summary>
        [ReadOnly(true)]
        public double ShapeModelCenterRow { get; set; }
        [ReadOnly(true)]
        public double ShapeModelCenterCol { get; set; }
        public double AngleStart { get; set; }
        public double AngleExtent { get; set; }

        [Description("Minimum score of the instances of the model to be found.")]
        public double MinScore { get; set; }

        [Description("Number of instances of the model to be found (or 0 for all matches).")]
        public double NumMatches { get; set; }
        //[Description("Maximum overlap of the instances of the model to be found.")]
        //public double MaxOverlap { get; set; }
        [Description("Number of pyramid levels used in the matching (and lowest pyramid level to use if |NumLevels| = 2).")]
        public double NumLevels { get; set; }

        #region 检测区域
        [ReadOnly(true)]
        public double DetectRow1 { get; set; }
        [ReadOnly(true)]
        public double DetectCol1 { get; set; }
        [ReadOnly(true)]
        public double DetectRow2 { get; set; }
        [ReadOnly(true)]
        public double DetectCol2 { get; set; }
        #endregion
    }

    public class ChipDetect
    {
        [Description("AbsThreshold determines the minimum amount of gray levels by which the image of the current object must differ from the image of the ideal object. ")]
        public double AbsThreshold { get; set; }
        [Description("VarThreshold determines a factor relative to the variation image for the minimum difference of the current image and the ideal image. ")]
        public double VarThreshold { get; set; }

        public double AreaMin { get; set; }
        public double AreaMax { get; set; }
    }

    public class ProgramConfig
    {
        //public sealed class MyCollectionEditor : CollectionEditor // need a reference to System.Design.dll
        //{
        //    public MyCollectionEditor(Type type)
        //        : base(type)
        //    {
        //    }

        //    protected override Type[] CreateNewItemTypes()
        //    {
        //        return new[] { typeof(RegionH) };
        //    }
        //}

        //[Editor(typeof(MyCollectionEditor), typeof(UITypeEditor))]

        public ChipDetect ChipDetectPar { get; set; }
        public ChipModel ChipModelPar { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 晶圆的编号信息
        /// </summary>
        public string Id { get; set; }
        public double DieWidth { get; set; }
        public double DieHeight { get; set; }
        public int BestZPulse { get; set; }
        public Die[,] Mapping { get; set; }
        public ObjectiveLense ObjectiveLense { get; set; }
        /// <summary>
        /// 是否带环片
        /// </summary>
        public HaveRingPiece HaveRingPiece { get; set; }

        /// <summary>
        /// 排除边沿芯片的个数
        /// </summary>
        public int ExcludeEdgeNum { get; set; }

        /// <summary>
        /// 晶圆的半径
        /// </summary>
        public double WaferRadius { get; set; }
        /// <summary>
        /// 切割道像素宽度
        /// </summary>
        public double CutRoadWidthPixel { get; set; }
        /// <summary>
        /// 切割道宽度微米
        /// </summary>
        public double CutRoadWidth { get; set; }
        //晶圆中心
        public Point WaferCenter { get; set; }
        /// <summary>
        /// 晶圆的厚度
        /// </summary>
        public double WaferThickness { get; set; }
        /// <summary>
        /// Die的总数
        /// </summary>
        public int DieCount { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public WaferSize WaferSize { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WaferType WaferType { get; set; }

        public TraitType TraitType { get; set; }
        public TraitLocation TraitLocation { get; set; }

        public string ModelSavePath { get; set; }
        public string ExportPath { get; set; }
        /// <summary>
        /// 获取物镜的倍率
        /// </summary>
        /// <returns></returns>
        public int GetLenseMag()
        {
            return ((int)ObjectiveLense) + 1;
        }
        /// <summary>
        /// 结果保存路径
        /// </summary>
        /// <returns></returns>
        public string GetExportFileName()
        {
            return Path.Combine(ExportPath, "config.zyn");
        }

        /// <summary>
        /// 获取本身的文件名
        /// </summary>
        /// <returns></returns>
        public string GetThisFileName()
        {
            return Path.Combine(ModelSavePath, "config.zyn");
        }

        /// <summary>
        /// Mapping图谱信息
        /// </summary>
        /// <returns></returns>
        public string GetMappingFileName()
        {
            return Path.Combine(ModelSavePath, "mapping.zyn");
        }
        public string GetVisibleMappingFileName()
        {
            return Path.Combine(ExportPath, "mapping.vzyn");
        }
        /// <summary>
        /// 获取芯片的4个脚模板文件名
        /// </summary>
        /// <returns></returns>
        public string GetChipModelFileName()
        {
           return  Path.Combine(ModelSavePath, "chip.model");
        }

        /// <summary>
        /// 获取芯片的4个脚模Region区域文件名
        /// </summary>
        /// <returns></returns>
        public string GetChipModelRegionFileName()
        {
            return Path.Combine(ModelSavePath, "chipModelRegion.hobj");
        }
        /// <summary>
        /// 获取晶圆的mark点模板文件名
        /// </summary>
        /// <returns></returns>
        public string GetWaferMarkModelFileName()
        {
            return Path.Combine(ModelSavePath, "WaferMark.model");
        }
        /// <summary>
        /// 获取晶圆的mark点区域文件名
        /// </summary>
        /// <returns></returns>
        public string GetWaferMarkModelRegionFileName()
        {
            return Path.Combine(ModelSavePath, "WaferMark.hobj");
        }

        /// <summary>
        /// 获取训练的模型文件名
        /// </summary>
        /// <returns></returns>
        public string GetChipVariationModelFileName()
        {
            return Path.Combine(ModelSavePath, "Variation.model");
        }

        /// <summary>
        /// 主要用于飞拍矫正用的
        /// </summary>
        /// <returns></returns>
        public string GetFlyModelFileName()
        {
            return Path.Combine(ModelSavePath, "fly.model");
        }

        /// <summary>
        /// XXXXXX轴获取实际的长度通过像素(um微米)
        /// </summary>
        /// <param name="offsetPixel">像素差值</param>
        /// <param name="pixelLenght">像元</param>
        /// <param name="lense">物镜倍率</param>
        /// <returns></returns>
        public static int GetXPulseByPixel(int offsetPixel, float pixelLenght, ObjectiveLense Oblense = ObjectiveLense.X1)
        {
            int lense = ((int)Oblense) + 1;
            return Convert.ToInt32(offsetPixel * ( (pixelLenght / lense)));
        }


        /// <summary>
        /// YYYYYY轴获取实际的长度通过像素(um微米)
        /// </summary>
        /// <param name="offsetPixel">像素差值</param>
        /// <param name="pixelLenght">像元</param>
        /// <param name="lense">物镜倍率</param>
        /// <returns></returns>
        public static int GetYPulseByPixel(int offsetPixel, float pixelLenght, ObjectiveLense Oblense = ObjectiveLense.X1)
        {
            int lense = ((int)Oblense) + 1;
            return Convert.ToInt32(offsetPixel  * ( (-pixelLenght / lense))); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
        }
    }
}
