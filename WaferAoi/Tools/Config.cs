using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
    public class Config
    {
        /// <summary>
        /// 运动轴
        /// </summary>
        public List<Axis> Axes { get; set; }
        /// <summary>
        /// 6寸相应的保存位置
        /// </summary>
        public List<PointInfo> Inch6SavePoints { get; set; }
        /// <summary>
        /// 8寸相应的保存位置
        /// </summary>
        public List<PointInfo> Inch8SavePoints { get; set; }
        /// <summary>
        /// 物镜转盘Com端口
        /// </summary>
        public string NosepieceCom { get; set; }
        /// <summary>
        /// 奥普特光源Com端口
        /// </summary>
        public string OPTLightCom { get; set; }
        public Config()
        {
            Axes = new List<Axis>();
            Inch6SavePoints = new List<PointInfo>();
            Inch8SavePoints = new List<PointInfo>();
        }
    }

    #region 保存的点位
    public class PointInfo
    {
        /// <summary>
        /// 点位X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 点位Y
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 点位Z
        /// </summary>
        public int Z { get; set; }
        /// <summary>
        /// 点位备注信息
        /// </summary>
        public string Remark { get; set; }
    }
    #endregion

    #region 轴相关类
    public class Axis
    {
        /// <summary>
        /// 轴号
        /// </summary>
        public int Id { get; set; }
        public string Remarks { get; set; }
        public AxisJogPrm JogPrm { get; set; }
        public AxisTrapPrm TrapPrm { get; set; }
        public AxisGoHomePar GoHomePar { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
    }

    public class AxisJogPrm
    {
        /// <summary>
        /// 获取Jog运动参数
        /// </summary>
        /// <returns></returns>
        public GSN.TJogPrm Get()
        {
            return new GSN.TJogPrm() { acc = Acc, dec = Dec, smooth = Smooth };
        }
        /// <summary>
        /// acc:点位运动的加速度。正数，单位：pulse/ms2。
        /// </summary>
        public double Acc { get; set; }
        /// <summary>
        /// dec:点位运动的减速度。正数，单位：pulse/ms2。未设置减速度时，默认减速度和加速度相同。
        /// </summary>
        public double Dec { get; set; }
        /// <summary>
        /// jog运动 smooth：平滑系数。取值范围：[0, 1)。平滑系数的数值越大，加减速过程越平稳
        /// </summary>
        public double Smooth { get; set; }
        /// <summary>
        /// 运动速度
        /// </summary>
        public double Vel { get; set; }
    }

    public class AxisTrapPrm
    {
        /// <summary>
        /// 获取点位运动参数
        /// </summary>
        /// <returns></returns>
        public GSN.TTrapPrm Get()
        {
            return new GSN.TTrapPrm() { velStart = VelStart, acc = Acc, dec = Dec, smoothTime = SmoothTime };
        }
        /// <summary>
        /// acc:点位运动的加速度。正数，单位：pulse/ms2。
        /// </summary>
        public double Acc { get; set; }
        /// <summary>
        /// dec:点位运动的减速度。正数，单位：pulse/ms2。未设置减速度时，默认减速度和加速度相同。
        /// </summary>
        public double Dec { get; set; }
        /// <summary>
        /// 点位运动 velStart：起跳速度。正数，单位：pulse/ms。默认值为 0。
        /// </summary>
        public double VelStart { get; set; }
        /// <summary>
        /// 点位运动 smoothTime：平滑时间。正整数，取值范围：[0, 50]，单位 ms。平滑时间的数值越大，加减速过程越平稳。
        /// </summary>
        public short SmoothTime { get; set; }
        /// <summary>
        /// 运动速度
        /// </summary>
        public double Vel { get; set; }
    }

    /// <summary>
    /// 轴回原点参数
    /// </summary>
    public class AxisGoHomePar
    {
        /// <summary>
        /// 获取回原点参数
        /// </summary>
        /// <returns></returns>
        public GSN.THomePrm Get()
        {
            return new GSN.THomePrm {
                mode = mode,
                moveDir = moveDir,
                indexDir = indexDir,
                edge = edge,
                triggerIndex = triggerIndex,
                velHigh = velHigh,
                velLow = velLow,
                acc = acc,
                dec = dec,
                smoothTime =smoothTime,
                homeOffset = homeOffset,
                searchHomeDistance = searchHomeDistance,
                searchIndexDistance =searchIndexDistance,
                escapeStep =escapeStep,
                pad10 = pad10,
                pad11 = pad11,
                pad12 = pad12,
                pad20 = pad20,
                pad21 = pad21,
                pad22 = pad22,
                pad31 = pad31,
                pad32 = pad32,
                pad33 = pad33,
            };
        }
        public string modeText { get; set; }
        /// <summary>
        ///  回原点模式，参考下面的回原点模式宏定义 默认都采用 HOME_MODE_LIMIT_HOME_INDEX这种方式回原点
        /// </summary>
        public short mode { get; set; }
        /// <summary>
        ///  设置启动搜索原点时的运动方向：非正数-负方向，正整数-正方向 
        /// </summary>
        public short moveDir { get; set; }
        /// <summary>
        ///  设置搜索Index的运动方向：非正数-负方向，正整数-正方向 
        /// </summary>
        public short indexDir { get; set; }
        /// <summary>
        ///  设置捕获沿：设置0，0-下降沿，非0值-上升沿 
        /// </summary>
        public short edge { get; set; }
        /// <summary>
        ///  用于设置触发器：默认设置为-1即可 取值-1和[1,8]，-1表示使用的触发器和轴号对应，其它值表示使用其它轴的触发器，触发器用于实现高速硬件捕获
        /// </summary>
        public short triggerIndex { get; set; }
        /// <summary>
        ///  搜索Home速度（单位：pulse/ms） 
        /// </summary>
        public double velHigh { get; set; }
        /// <summary>
        ///  搜索Index速度（单位：pulse/ms） 
        /// </summary>
        public double velLow { get; set; }
        /// <summary>
        ///  回原点运动的加速度（单位：pulse/ms^2） 
        /// </summary>
        public double acc { get; set; }
        /// <summary>
        ///  回原点运动的减速度（单位：pulse/ms^2） 
        /// </summary>
        public double dec { get; set; }
        /// <summary>
        ///  回原点运动的平滑时间：取值[0,1)，具体含义与GTS系列控制器点位运动相似 
        /// </summary>
        public short smoothTime { get; set; }
        /// <summary>
        ///  最终停止的位置相对于原点的偏移量 
        /// </summary>
        public int homeOffset { get; set; }
        /// <summary>
        ///  Home最大搜索距离， 0表示不限制搜索距离，默认为805306368或-805306368
        /// </summary>
        public int searchHomeDistance { get; set; }
        /// <summary>
        ///  Index最大搜索距离， 0表示不限制搜索距离，默认为805306368或-805306368
        /// </summary>
        public int searchIndexDistance { get; set; }
        /// <summary>
        ///  采用“限位回原点”方式时，反方向离开限位的脱离步长 
        /// </summary>
        public int escapeStep { get; set; }

        #region 预留参数
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad10 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad11 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad12 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad20 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad21 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad22 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad31 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad32 { get; set; }
        /// <summary>
        /// 预留参数
        /// </summary>
        public short pad33 { get; set; }
        #endregion
    }
    #endregion
}
