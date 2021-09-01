using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
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
    public enum TraitType: int
    {
        Flat= 0, // 凹槽
        Notch = 1 // 切面
    }
    public enum TraitLocation : int
    {
        TOP = 0,
        BOTTOM = 1,
        LEFT =2,
        RIGHT =3,
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

    }
}
