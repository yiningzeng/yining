using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
    public class EMUMS
    {
        public enum AxisGoHome
        {
            //        回原点模式宏定义：
            HOME_MODE_LIMIT = 10, //限位回原点
            HOME_MODE_LIMIT_HOME = 11, //限位+Home回原点
            HOME_MODE_LIMIT_INDEX = 12, //限位+Index回原点
            HOME_MODE_LIMIT_HOME_INDEX = 13, //限位+Home+Index回原点
            HOME_MODE_HOME = 20, //Home回原点
            HOME_MODE_HOME_INDEX = 22, //Home+Index回原点
            HOME_MODE_INDEX = 30, //Index回原点
        }

        public enum AxisGoHomeStage
        {
            HOME_STAGE_IDLE = 0, //未启动Smart Home回原点
            HOME_STAGE_START = 1, //启动Smart Home回原点
            HOME_STAGE_SEARCH_LIMIT = 10, //正在寻找限位
            HOME_STAGE_SEARCH_LIMIT_STOP = 11, //触发限位停止
            HOME_STAGE_SEARCH_LIMIT_ESCAPE = 13, //反方向运动脱离限位
            HOME_STAGE_SEARCH_LIMIT_RETURN = 15, //重新回到限位
            HOME_STAGE_SEARCH_LIMIT_RETURN_STOP = 16, //重新回到限位停止
            HOME_STAGE_SEARCH_HOME = 20, //正在搜索Home
            HOME_STAGE_SEARCH_HOME_RETURN = 25, //搜索到Home后运动到捕获的Home位置
            HOME_STAGE_SEARCH_INDEX = 30, //正在搜索Index
            HOME_STAGE_GO_HOME = 80, //正在执行回原点过程
            HOME_STAGE_END = 100, //回原点结束
        }

        public enum AxisGoHomeError
        {
            HOME_ERROR_NONE = 0, //未发生错误
            HOME_ERROR_NOT_TRAP_MODE = 1, //回原点的轴不是处于点位运动模式
        }


        /// <summary>
        /// 轴状态
        /// </summary>
        public class AxisStatus
        {
            public const short ConnectError = -1; // "主机和运动控制器通讯失败"
            public const short OpenError = -6; //打开运动控制器失败
            public const short Alarm = 0x2; //驱动器报警
            public const short PosError = 0x10; //跟随误差越限，走的目标位置和实际位置误差大于设定极限
            public const short PLimOn = 0x20; //正限位信号有效
            public const short NLimOn = 0x40; //负限位信号有效
            public const short StopDec = 0x80; //平滑停止信号有效，自动平滑停止电机，如果设置了该io的话
            public const short StopEmg = 0x100; //急停停止信号有效，自动急停停止电机，如果设置该io的话
            public const short ServoOn = 0x200; //伺服使能信号有效
            public const short Running = 0x400; //伺服正在运动
            public const short Arrive = 0x800; //伺服到位信号
        }
        public enum IOPointsOut
        {
            StartLight = 1,
            ResetLight = 2,
            StopLight = 3,
            ThreeLightYellow = 4,
            ThreeLightRed = 5,
            ThreeLightGreen = 6,
            Buzzer = 7,
        }
        public enum IOPointsIn
        {
            EmergencyStop = 1,
            Start = 2,
            Reset = 3,
            Stop = 4,
            Door = 5,
            PositivePressure = 8,
            NegativePressure1 = 9,
            NegativePressure2 = 10,
            NegativePressure3 = 11,
        }

        public enum IOPointsOutExt
        {
            JackingAir = 1,
            DieAir = 2,
            SixInchesAir = 3,
            EightInchesAir = 4,
        }
    }


    public class NosepieceData
    {
        public static string PortName;
        public const string Parity = "None"; //{ get; set; }
        public const string StopBits = "One";
        public const string DataBits = "8";
        public const string BaudRate = "115200";
        public const string Select5BingHoleHex = "5A A5 06 83 10 05 01 00 04 A3"; // 选择5个大洞模式
        public const string Hole1Hex = "5A A5 06 83 10 03 01 00 01 9E"; // 洞1
        public const string Hole2Hex = "5A A5 06 83 10 03 01 00 02 9F"; // 洞2
        public const string Hole3Hex = "5A A5 06 83 10 03 01 00 03 A0"; // 洞3
        public const string Hole4Hex = "5A A5 06 83 10 03 01 00 04 A1"; // 洞4
        public const string Hole5Hex = "5A A5 06 83 10 03 01 00 05 A2"; // 洞5
    }
}
