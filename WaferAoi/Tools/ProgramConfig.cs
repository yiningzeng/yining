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
        X10v2,
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
        Flat = 0, // 凹槽
        Notch = 1 // 切面
    }
    public enum TraitLocation : int
    {
        TOP = 0,
        BOTTOM = 1,
        LEFT = 2,
        RIGHT = 3,
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

    public class ProgramConfig
    {
        public string Name { get; set; }
        public double DieWidth { get; set; }
        public double DieHeight { get; set; }

        /// <summary>
        /// 是否带环片
        /// </summary>
        public HaveRingPiece HaveRingPiece { get; set; }

        /// <summary>
        /// 晶圆的半径
        /// </summary>
        public double WaferRadius { get; set; }
        /// <summary>
        /// 切割道宽度
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
            return Path.Combine(ModelSavePath, "chip.model");
        }
        /// <summary>
        /// 获取晶圆的mark点区域文件名
        /// </summary>
        /// <returns></returns>
        public string GetWaferMarkModelRegionFileName()
        {
            return Path.Combine(ModelSavePath, "mark.hobj");
        }
    }
}
