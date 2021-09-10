using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class RegionH
    {
        public double Row1 { get; set; }
        public double Col1 { get; set; }
        public double Row2 { get; set; }
        public double Col2 { get; set; }

        public RegionH(double Row1, double Col1, double Row2, double Col2)
        {
            this.Row1 = Row1;
            this.Col1 = Col1;
            this.Row2 = Row2;
            this.Col2 = Col2;
        }
    }

    public class ProgramConfig
    {
        public RegionH DetectRegion { get; set; }
        public string Name { get; set; }
        public double DieWidth { get; set; }
        public double DieHeight { get; set; }

        public ObjectiveLense ObjectiveLense { get; set; }
        /// <summary>
        /// 是否带环片
        /// </summary>
        public HaveRingPiece HaveRingPiece { get; set; }

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

        /// <summary>
        /// 获取本身的文件名
        /// </summary>
        /// <returns></returns>
        public string GetThisFileName()
        {
            return Path.Combine(ModelSavePath, "config.zyn");
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
            return Convert.ToInt32(offsetPixel * 3.2 / (3.2 * (pixelLenght / lense)));
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
            return Convert.ToInt32(offsetPixel * 3.2 / (3.2 * (-pixelLenght / lense))); // *相机像元 / 物镜的倍率 (物镜的倍率 = 相机像元 * 实际的算出来的像元大小)
        }
    }
}
