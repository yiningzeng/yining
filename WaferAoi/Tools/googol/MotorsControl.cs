using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YiNing.Tools;

namespace WaferAoi.Tools
{
    public class MotorsControl
    {
        const short CORE = 1;
        const short DriveEnable = 10;
        const short AlarGSNlear = 11;
        const short GeneralOutput = 12;
        /// <summary>
        /// 控制卡接口调用出错统一记录
        /// </summary>
        public static string logErrorMsg(string action, short nRetVal)
        {
            #region ***** 控制卡接口调用出错统一记录函数 *****

            string strErrorInfo = string.Empty;

            switch (nRetVal)
            {
                case 1:
                    strErrorInfo = "指令执行错误！";
                    break;
                case 2:
                    strErrorInfo = "license不支持！";
                    break;
                case 7:
                    strErrorInfo = "指令参数错误！";
                    break;
                case -1:
                    strErrorInfo = "主机和运动控制器通讯失败！";
                    break;
                case -6:
                    strErrorInfo = "打开控制器失败！";
                    break;
                case -7:
                    strErrorInfo = "运动控制器没有响应！";
                    break;
                case -10:
                    strErrorInfo = "主机和运动控制器通讯失败！";
                    break;
                case -11:
                    strErrorInfo = "动态库加载失败！";
                    break;
                case -13:
                    strErrorInfo = "编码器初始化失败！";
                    break;
                case -14:
                    strErrorInfo = "编码器初始化失败！";
                    break;
                case -15:
                    strErrorInfo = "动态库版本不匹配！";
                    break;
                case 15:
                    strErrorInfo = "动态库版本不匹配！";
                    break;
                case 16:
                    strErrorInfo = "不具备版本匹配功能！";
                    break;
                case -131:
                    strErrorInfo = "网络初始化失败！";
                    break;
                case -133:
                    strErrorInfo = "用户设置网络模块配置信息和实际所接网络模块不一致！";
                    break;
                default:
                    break;
            }
            LogHelper.WriteLog(action + "出错，错误代码：" + nRetVal.ToString() + "，错误信息：" + strErrorInfo, new Exception());
            return strErrorInfo;
            #endregion
        }


        /// <summary>
        /// 打开运动控制卡，仅在程序启动时调用一次
        /// </summary>
        /// <param name="hasExtModule">是否包含扩展</param>
        /// <returns></returns>
        public static bool OpenDevice(bool hasExtModule = true)//wells0029
        {
            #region ***** 打开运动控制卡 *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_Open(5, 1);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Open", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_Reset(CORE);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Reset", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_LoadConfig(CORE, "gtn_core1.cfg");
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Reset", nRetVal);
                return false;
            }

            if (hasExtModule)//wells0029
            {
                nRetVal = GSN.GTN_ExtModuleInit(CORE, 1);
                if (nRetVal != 0)
                {
                    logErrorMsg("GTN_ExtModuleInit", nRetVal);
                    return false;
                }
                //bHasExtModule = hasExtModule;
            }
            return true;

            #endregion
        }
        /// <summary>
        /// 关闭运动控制卡，在程序结束前调用一次，销毁卡资源
        /// </summary>
        public static bool CloseDevice()
        {
            #region ***** 关闭运动控制卡 *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_Close();
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Close", nRetVal);
                return false;
            }

            return true;

            #endregion
        }


        /// <summary>
        /// DI信号监控,IO类型 "0"=正限位，"1"=负限位，"2"=驱动报警，"3"=原点开关，
        /// "4"=通用输入，"5"=电机到位信号
        /// </summary>
        /// <param name="CORE">轴卡</param>
        /// <param name="diTepy"></param>
        /// <returns></returns>
        public static int IoSignalEXI(short diTepy)
        {
            int PdiValue = -1;
            switch (diTepy)
            {
                case 0:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
                case 1:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
                case 2:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
                case 3:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
                case 4:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
                case 5:
                    GSN.GTN_GetDi(CORE, diTepy, out PdiValue);
                    break;
            }
            return PdiValue;
        }
        /// <summary>
        /// 信号输出
        /// </summary>
        /// <param name="CORE"></param>
        /// <param name="diTepy">"10"=驱动器使能，"11"=报警清除，"12"=通用输出</param>
        /// <param name="PdoValue">"1"高电平，"0"低电平</param>
        /// <returns></returns>
        public static int IoSignalEXO(int Index, int PdoValue, short diTepy = GeneralOutput)
        {
            int setStart = -1;
            switch (diTepy)
            {
                case DriveEnable:
                    setStart = GSN.GTN_SetDo(CORE, diTepy, PdoValue);
                    break;
                case AlarGSNlear:
                    setStart = GSN.GTN_SetDoBit(CORE, diTepy, (short)Index, 0);
                    Thread.Sleep(50);
                    setStart = GSN.GTN_SetDoBit(CORE, diTepy, (short)Index, 1);
                    Thread.Sleep(50);
                    ClearSts(Index);
                    break;
                case GeneralOutput:
                    setStart = GSN.GTN_SetDoBit(CORE, diTepy, (short)Index, (short)PdoValue);
                    break;
            }
            return setStart;
        }

        /// <summary>
        /// 扩展信号输出
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PdoValue"></param>
        /// <returns></returns>
        public static bool IoExtSignalEXO(EMUMS.IOPointsOutExt iOPointsOutExt, int PdoValue)
        {
            short nRetVal = -1;
            nRetVal = GSN.GTN_SetExtDoBit(CORE, (short)iOPointsOutExt, (short)PdoValue);//(CORE, diTepy, (short)Index, (short)PdoValue);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_SetExtDoBit", nRetVal);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 伺服使能开启
        /// </summary>
        public static bool ServoOn(int nAxisNumber)
        {
            #region ***** 打开轴使能  *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_AxisOn(CORE, (short)nAxisNumber);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_AxisOn", nRetVal);
                return false;
            }

            return true;

            #endregion
        }

        /// <summary>
        /// 伺服使能关闭
        /// </summary>
        public static bool ServoOff(int nAxisNumber)
        {
            #region ***** 关闭轴使能  *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_AxisOff(CORE, (short)nAxisNumber);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_AxisOff", nRetVal);
                return false;
            }

            return true;

            #endregion
        }

        /// <summary>
        /// JOG运动模式
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="jog"></param>
        /// <param name="vel"></param>
        /// <param name="direction">如果大于0则正向运动，小于0反向运动</param>
        public static void MoveJog(int axis, GSN.TJogPrm jog, double vel, int direction = 1)
        {
            short nAxisNumber = (short)axis;
            GSN.GTN_PrfJog(CORE, nAxisNumber); // 设置为Jog运动模式
            GSN.GTN_SetJogPrm(CORE, nAxisNumber, ref jog); // 设置Jog运动参数
            GSN.GTN_SetVel(CORE, nAxisNumber, direction > 0 ? vel : -vel);  // 设置目标速度
            GSN.GTN_Update(CORE, 1 << (axis - 1));    // 更新轴运动
            GSN.GTN_ClrSts(CORE, nAxisNumber, 1);
        }

        /// <summary>
        /// 2维运动
        /// </summary>
        /// <param name="vel">速度</param>
        /// <param name="px">点位x</param>
        /// <param name="py">点位y</param>
        public static bool MovePoint2D(double vel, int px, int py, Axis ax, Axis ay, bool listen = false)
        {
            try
            {
                //Axis ax = config.Axes.Find(v => v.Id == 2);
                //Axis ay = config.Axes.Find(v => v.Id == 1);
                Parallel.Invoke(() => MoveTrap(ax.Id, ax.TrapPrm.Get(), vel, px, listen), () => MoveTrap(ay.Id, ay.TrapPrm.Get(), vel, py, listen));
                return true;    
            }
            catch (Exception er) { return false; }
        }

        /// <summary>
        /// 点位运动模式
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="trapPrm"></param>
        /// <param name="vel"></param>
        /// <param name="pos">点位脉冲</param>
        /// <returns></returns>
        public static bool MoveTrap(int axis, GSN.TTrapPrm trapPrm, double vel, int pos, bool listen = false)
        {
            short nAxisNumber = (short)axis;
            #region ***** 绝对位置移动  *****

            short nRetVal = 0;
            short Srtn = GSN.GTN_SetAxisBand(CORE, nAxisNumber, 20, 5);
            nRetVal = GSN.GTN_ClrSts(CORE, nAxisNumber, 1);///清除状态

            nRetVal = GSN.GTN_PrfTrap(CORE, nAxisNumber);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_PrfTrap", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_SetTrapPrm(CORE, nAxisNumber, ref trapPrm);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_SetTrapPrm", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_SetPos(CORE, nAxisNumber, pos);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_SetPos", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_SetVel(CORE, nAxisNumber, vel);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_SetVel", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_Update(CORE, 1 << (nAxisNumber - 1));
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Update", nRetVal);
                return false;
            }

            if (listen)
            {
                int state = -1;
                DateTime dateBegin = DateTime.Now;
                while ((state & EMUMS.AxisStatus.Running) == EMUMS.AxisStatus.Running)
                {
                    GSN.GTN_GetSts(CORE, nAxisNumber, out state, 1, out uint pc);
                    var timeSpan = DateTime.Now - dateBegin;
                    if (timeSpan.TotalSeconds > 40)
                    {
                        break;
                    }
                };
            }
            return true;

            #endregion
        }

        /// <summary>
        /// 改变速度，即时生效
        /// </summary>
        public static bool ChangeVel(short nAxisNumber, double vel)
        {
            #region ***** 改变速度，即时生效 *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_SetVel(1, nAxisNumber, vel);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_SetVel", nRetVal);
                return false;
            }

            nRetVal = GSN.GTN_Update(1, 1 << (nAxisNumber - 1));
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Update", nRetVal);
                return false;
            }

            return true;

            #endregion
        }

        /// <summary>
        /// 获取轴状态int返回值
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int GetAxisStatusInt(int axis, short count = 1)
        {
            int stn = GSN.GTN_GetSts(CORE, (short)axis, out int iAxisSts, count, out uint uiClock);

            switch (stn)
            {
                case -1:
                case -6:
                    return stn;
            }
            return iAxisSts;
        }
        /// <summary>
        /// 轴的运行状态字符串
        /// CORE 卡号
        /// AXIS 运行轴
        /// count 数量
        /// </summary>
        /// <param name="CORE"></param>
        /// <param name="axis"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetAxisStatusString(int axis, short count = 1)
        {
            int stn = GSN.GTN_GetSts(CORE, (short)axis, out int iAxisSts, count, out uint uiClock);

            switch (stn)
            {
                case -1:
                    return "主机和运动控制器通讯失败";

                case -6:
                    return "打开运动控制器失败";
            }

            // 驱动器报警标志
            if ((iAxisSts & 0x1) != 0)
            {
                return "驱动器报警";
            }
            // 伺服报警标志
            if ((iAxisSts & EMUMS.AxisStatus.Alarm) != 0)
            {
                return "伺服报警";
            }
            // 跟随误差越限标志
            if ((iAxisSts & EMUMS.AxisStatus.PosError) != 0)
            {
                return "运动出错";
            }
            // 正向限位
            if ((iAxisSts & EMUMS.AxisStatus.PLimOn) != 0)
            {
                return "正限位触发";
            }
            // 负向限位
            if ((iAxisSts & EMUMS.AxisStatus.NLimOn) != 0)
            {
                return "负限位触发";
            }
            // 平滑停止
            if ((iAxisSts & EMUMS.AxisStatus.StopDec) != 0)
            {
                return "平滑停止触发";
            }

            // 急停标志
            if ((iAxisSts & EMUMS.AxisStatus.StopEmg) != 0)
            {
                return "急停触发";
            }
            // 伺服使能标志
            if ((iAxisSts & EMUMS.AxisStatus.ServoOn) != 0)
            {
                return "伺服使能";
            }
            // 规划器正在运动标志
            if ((iAxisSts & EMUMS.AxisStatus.Running) != 0)
            {
                return "正在运动";
            }
            if (iAxisSts == 0)
            {
                return "初始化成功";
            }
            else
            {
                return "未知指令";
            }
        }

        /// <summary>
        /// 单独获取轴的使能状态
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool GetAxisEnable(int axis)
        {
            int stn = GSN.GTN_GetSts(CORE, (short)axis, out int iAxisSts, 1, out uint uiClock);
            switch (stn)
            {
                case -1: // 主机和运动控制器通讯失败
                    return false;

                case -6: // 打开运动控制器失败
                    return false;
            }
            if ((iAxisSts & EMUMS.AxisStatus.ServoOn) != 0) return true; else return false;
        }
        /// <summary>
        /// 清除轴状态
        /// </summary>
        public static bool ClearSts(int nAxisNumber)
        {
            #region ***** 清除轴状态，轴的限位等状态，在离开限位回到安全运动范围后，需要手动清除状态才能重置该状态，否则轴无法继续运动  *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_ClrSts(CORE, (short)nAxisNumber, 1);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_ClrSts", nRetVal);
                return false;
            }

            return true;

            #endregion
        }

        /// <summary>
        /// 位置清零
        /// </summary>
        public static bool ZeroPos(int nAxisNumber)
        {
            #region 位置清零
            short nRetVal = 0;

            nRetVal = GSN.GTN_ZeroPos(CORE, (short)nAxisNumber, 1);
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_ZeroPos", nRetVal);
                return false;
            }

            return true;

            #endregion
        }


        /// <summary>
        /// 停止运动轴，0表示缓停，1表示急停
        /// </summary>
        public static bool StopAxis(int axis, int type)
        {
            short nAxisNumber = (short)axis;
            #region ***** 停止运动轴，0表示缓停，1表示急停 *****

            short nRetVal = 0;

            int status = 0;                                                     ///轴状态值
            uint pClock = 0;                                                  ///轴状态中间值

            nRetVal = GSN.GTN_Stop(1, 1 << (nAxisNumber - 1), type << (nAxisNumber - 1));
            if (nRetVal != 0)
            {
                logErrorMsg("GTN_Stop", nRetVal);
                return false;
            }

            do
            {
                nRetVal = GSN.GTN_GetSts(1, nAxisNumber, out status, 1, out pClock);///获取轴状态
            } while (EMUMS.AxisStatus.Running == (status & EMUMS.AxisStatus.Running));///等待轴停止

            return true;

            #endregion
        }

        public struct TAxisInfo
        {
            public double prfpos; //规划位置
            public double prfvel; //规划速度
            public double encpos; //实际位置
            public double encvel; //实际速度
        }
        /// <summary>
        /// 获取轴规划和实际的速度&位置
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="prfpos">规划位置</param>
        /// <param name="prfvel"></param>
        /// <param name="encpos"></param>
        /// <param name="encvel"></param>
        public static TAxisInfo AxisReadInfo(int nAxisNumber)
        {
            short axis = (short)nAxisNumber;
            uint clk = 0;
            TAxisInfo axisInfo = new TAxisInfo();
            GSN.GTN_GetPrfPos(CORE, axis, out axisInfo.prfpos, 1, out clk);
            GSN.GTN_GetPrfVel(CORE, axis, out axisInfo.prfvel, 1, out clk);
            GSN.GTN_GetEncPos(CORE, axis, out axisInfo.encpos, 1, out clk);
            GSN.GTN_GetEncVel(CORE, axis, out axisInfo.encvel, 1, out clk);
            return axisInfo;
        }

        /// <summary>
        /// 获取X，Y，z当前的坐标
        /// </summary>
        /// <param name="axisXId"></param>
        /// <param name="axisYid"></param>
        /// <param name="axisZid"></param>
        /// <returns></returns>
        public static PointInfo GetXYZEncPos(int axisXId, int axisYid, int axisZid)
        {
            try
            {
                double encX = 20181121, encY = 20181121, encZ = 20181121;
                uint clk = 0;
                if (axisXId != -1) GSN.GTN_GetEncPos(CORE, (short)axisXId, out encX, 1, out clk);
                if (axisYid != -1) GSN.GTN_GetEncPos(CORE, (short)axisYid, out encY, 1, out clk);
                if (axisZid != -1) GSN.GTN_GetEncPos(CORE, (short)axisZid, out encZ, 1, out clk);
                if (encX.Equals(20181121) || encY.Equals(20181121) || encZ.Equals(20181121))
                    return new PointInfo() { Remark = "失败" };
                else
                    return new PointInfo() { X = Convert.ToInt32(encX), Y = Convert.ToInt32(encY), Z = Convert.ToInt32(encZ), Remark = "成功" };
            }
            catch { return new PointInfo() { Remark = "失败" }; }
        }
        /// <summary>
        /// 获取旋转轴当前的位置
        /// </summary>
        /// <param name="axisRid"></param>
        /// <returns></returns>
        public static int GetREncPos(int axisRid)
        {
            try
            {
                double encR;
                uint clk = 0;
                GSN.GTN_GetEncPos(CORE, (short)axisRid, out encR, 1, out clk);
                return Convert.ToInt32(encR);
            }
            catch { return 20181121; }
        }
        /// <summary>
        /// 获取X，Y，z当前的坐标
        /// </summary>
        /// <param name="axisXId"></param>
        /// <param name="axisYid"></param>
        /// <param name="axisZid"></param>
        /// <returns></returns>
        public static int GetZEncPos(int axisZid)
        {
            try
            {
                double encZ;
                uint clk = 0;
                GSN.GTN_GetEncPos(CORE, (short)axisZid, out encZ, 1, out clk);
                return Convert.ToInt32(encZ);
            }
            catch { return 20181121; }
        }



        public static short GoHome(int nAxisNumber, GSN.THomePrm goHomePrm, out GSN.THomeStatus homeStatus)
        {
            ClearSts(nAxisNumber);
            IoSignalEXO(nAxisNumber, 1, 11);
            short axis = (short)nAxisNumber;
            GSN.GTN_GetSts(CORE, axis, out int pSta, 1, out uint pC);
            if ((pSta & 0x40) == 0x40)
            {
                GSN.GTN_GetPos(CORE, 1, out int pos);
                GSN.GTN_ClrSts(CORE, axis, 1);
                GSN.GTN_PrfTrap(CORE, axis); // 设置为点位运动模式
                GSN.GTN_GetTrapPrm(CORE, axis, out GSN.TTrapPrm trap);
                trap.acc = goHomePrm.acc;
                trap.dec = goHomePrm.dec;
                trap.smoothTime = goHomePrm.smoothTime;
                GSN.GTN_SetTrapPrm(CORE, axis, ref trap); // 设置点位运动参数
                GSN.GTN_SetVel(CORE, axis, 10);  // 设置目标速度
                GSN.GTN_SetPos(CORE, axis, pos + 10000);  // 设置目标位置
                GSN.GTN_Update(CORE, 1 << (axis - 1));    // 更新轴运动              
                do
                {
                    GSN.GTN_GetSts(CORE, axis, out pSta, 1, out pC);
                } while ((pSta & 0x400) == 0x400);
            }
            //没有限位开关则取消限位,取消限位信息
            //GTS.GT_LmtsOn(_cardNum, i, 0);
            //清除轴状态
            GSN.GTN_ClrSts(CORE, axis, 1);
            //
            GSN.GTN_ZeroPos(CORE, axis, 1);
            //启动自动回原点
            short stn = GSN.GTN_GoHome(CORE, axis, ref goHomePrm);
            do
            {
                GSN.GTN_GetHomeStatus(CORE, axis, out homeStatus);
            } while (Convert.ToBoolean(homeStatus.run));

            Thread.Sleep(200);
            GSN.GTN_ZeroPos(CORE, axis, 1);
            return stn;
        }

        public static async void GoHomeAsync(int nAxisNumber, GSN.THomePrm goHomePrm, Action<short, GSN.THomeStatus> goHomeCallback)
        {
            ClearSts(nAxisNumber);
            IoSignalEXO(nAxisNumber, 1, 11);
            object[] objectArray = new object[3];//这里的2就是改成你要传递几个参数
            objectArray[0] = (short)nAxisNumber;
            objectArray[1] = goHomePrm;
            objectArray[2] = goHomeCallback;
            //object param = (object)objectArray;
            await Task.Factory.StartNew(obs =>
             {
                 object[] objArr = (object[])obs;
                 short axis = (short)objArr[0];
                 var _goHomePrm = (GSN.THomePrm)objArr[1];
                 var _goHomeCallback = (Action<short, GSN.THomeStatus>)objArr[2];
                 GSN.GTN_GetSts(CORE, axis, out int pSta, 1, out uint pC);
                 if ((pSta & 0x40) == 0x40)
                 {
                     GSN.GTN_GetPos(CORE, 1, out int pos);
                     GSN.GTN_ClrSts(CORE, axis, 1);
                     GSN.GTN_PrfTrap(CORE, axis); // 设置为点位运动模式
                     GSN.GTN_GetTrapPrm(CORE, axis, out GSN.TTrapPrm trap);
                     trap.acc = _goHomePrm.acc;
                     trap.dec = _goHomePrm.dec;
                     trap.smoothTime = _goHomePrm.smoothTime;
                     GSN.GTN_SetTrapPrm(CORE, axis, ref trap); // 设置点位运动参数
                     GSN.GTN_SetVel(CORE, axis, 10);  // 设置目标速度
                     GSN.GTN_SetPos(CORE, axis, pos + 10000);  // 设置目标位置
                     GSN.GTN_Update(CORE, 1 << (axis - 1));    // 更新轴运动
                     do
                     {
                         GSN.GTN_GetSts(CORE, axis, out pSta, 1, out pC);
                     } while ((pSta & 0x400) == 0x400);
                 }
                 //没有限位开关则取消限位,取消限位信息
                 //GTS.GT_LmtsOn(_cardNum, i, 0);
                 //清除轴状态
                 GSN.GTN_ClrSts(CORE, axis, 1);
                 //
                 GSN.GTN_ZeroPos(CORE, axis, 1);
                 //启动自动回原点
                 short stn = GSN.GTN_GoHome(CORE, axis, ref _goHomePrm);
                 GSN.THomeStatus homeStatus;
                 do
                 {
                     GSN.GTN_GetHomeStatus(CORE, axis, out homeStatus);
                     _goHomeCallback(axis, homeStatus);
                 } while (Convert.ToBoolean(homeStatus.run));
                 Thread.Sleep(200);
                 GSN.GTN_ZeroPos(CORE, axis, 1);
                 _goHomeCallback(axis, homeStatus);
             }, objectArray);
        }

        #region 位置比较
        private static int _cmpMode = 0;//wells0170
        /// <summary>
        /// 保存输出DO类型，【0，GPO；1，HSIO】
        /// </summary>
        private static List<int> _doType = new List<int>();//wells0185//改为列表，支持多通道数据
        //private static int _doType = 1;//wells0170

        /// <summary>
        /// 输出IO索引，默认是3
        /// </summary>
        private static List<int> _doChannel = new List<int>();//wells0185//改为列表，支持多通道数据
        //private static int _doChannel = 3;//wells0170
        /// <summary>
        /// gpo端口
        /// </summary>
        private static ushort _gpo = 0x0;//wells0185

        /// <summary>
        /// hso端口
        /// </summary>
        private static ushort _hso = 0x0;//wells0185
        /// <summary>
        /// 设置比较模式
        /// </summary>
        /// <param name="channel">输出通道</param>
        /// <param name="doType">输出通道类型，【0，GPO；1，HSIO】</param>
        /// <param name="cmpMode">比较模式【0，FIFO模式；1，线性模式；2，等差模式】</param>
        /// <param name="dimension">比较维度【1，一维；2，二维】</param>
        /// <param name="sourceMode">比较源【0，FDB；1，CMD】</param>
        /// <param name="axisXId">比较源X轴号</param>
        /// <param name="axisYId">比较源Y轴号</param>
        /// <param name="outputMode">输出模式【0，脉冲；1，电平】</param>
        /// <param name="outputPulseWidth">输出脉冲宽度</param>
        /// <returns></returns>
        ///2, 2, new short[] { 1, 1}, new short[] { 1, 1 }, 2, 1, 1,  0, 100
        public static bool setCompareMode(int axisXId, int axisYId, short[] channel = null, short[] doType = null, short cmpMode = 2, short dimension = 1, ushort outputPulseWidth = 100, short sourceMode = 1, short outputMode = 0)//wells0173//wells0185//支持多通道比较
        {
            if (channel == null) channel = new short[] { 1, 1 };
            if (doType == null) doType = new short[] { 1, 1 };
            #region ***** 设置比较模式 *****
            //setCompareMode(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 1, 1, 5, 5, 0, 100)
            short nRetVal = 0;

            if (channel == null || doType == null || channel.Length != doType.Length)
                return false;

            nRetVal = GSN.GTN_PosCompareStop(1, 1);//先关闭比较触发功能
            if (nRetVal != 0)
            {
                return false;
            }

            nRetVal = GSN.GTN_PosCompareClear(1, 1);//清空比80较数据缓存
            if (nRetVal != 0)
            {
                return false;
            }

            short permit = 0x2;

            _gpo = 0;
            _hso = 0;

            for (int igg = 0; igg < channel.Length; igg++)
            {
                short dataType = 0;
                ushort tmp = (ushort)(1 << (channel[igg] - 1));
                if (doType[igg] == 0)
                {
                    dataType = GSN.MC_GPO;
                    _gpo |= tmp;
                }
                else
                {
                    dataType = GSN.MC_HSO;
                    _hso |= tmp;
                }
                nRetVal = GSN.GTN_SetTerminalPermitEx(1, 1, dataType, ref permit, channel[igg], 1);//设置比较通道
                if (nRetVal != 0)
                {
                    return false;
                }
            }

            GSN.TPosCompareMode mode;
            nRetVal = GSN.GTN_GetPosCompareMode(1, 1, out mode);//获取比较模式参数
            if (nRetVal != 0)
            {
                return false;
            }

            mode.mode = cmpMode;
            mode.dimension = dimension;
            mode.sourceMode = sourceMode;
            mode.sourceX = (short)axisXId;
            mode.sourceY = (short)axisYId;
            mode.outputMode = outputMode;
            mode.outputCounter = 1;
            mode.outputPulseWidth = outputPulseWidth;
            mode.errorBand = 0;

            nRetVal = GSN.GTN_SetPosCompareMode(1, 1, ref mode);//设置比较模式参数
            if (nRetVal != 0)
            {
                return false;
            }

            _cmpMode = cmpMode;
            _doType.Clear();
            _doChannel.Clear();
            for (int igg = 0; igg < channel.Length; igg++)
            {
                _doType.Add(doType[igg]);
                _doChannel.Add(channel[igg]);
            }

            return true;

            #endregion
        }
        public static bool setCompareData_Pso(int interval)//wells0173
        {
            #region ***** 设置PSO数据 *****

            short nRetVal = 0;

            //nRetVal = GSN.GTN_PosCompareClear(1, 1);//清空比较数据缓存
            //if (nRetVal != 0)
            //{
            //    return false;
            //}

            //GSN.TPosCompareMode mode;
            //nRetVal = GSN.GTN_GetPosCompareMode(1, 1, out mode);//获取比较模式参数
            //if (nRetVal != 0)
            //{
            //    return false;
            //}

            //if (mode.mode != 2 || _cmpMode != 2) //模式不匹配
            //    return false;

            GSN.TPosComparePsoPrm pPrm;
            nRetVal = GSN.GTN_GetPosComparePsoPrm(1, 1, out pPrm);
            if (nRetVal != 0)
            {
                return false;
            }

            pPrm.count = 1;
            pPrm.gpo = _gpo;
            pPrm.hso = _hso;
            pPrm.syncPos = interval;

            nRetVal = GSN.GTN_SetPosComparePsoPrm(1, 1, ref pPrm);
            if (nRetVal != 0)
            {
                return false;
            }

            return true;

            #endregion
        }

        public static bool startCompare()//wells0173
        {
            #region ***** 开始比较功能 *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_PosCompareStart(1, 1);//开启比较触发功能
            if (nRetVal != 0)
            {
                return false;
            }

            return true;

            #endregion
        }

        public static bool stopCompare()//wells0173
        {
            #region ***** 停止比较功能 *****

            short nRetVal = 0;

            nRetVal = GSN.GTN_PosCompareStop(1, 1);//先关闭比较触发功能
            if (nRetVal != 0)
            {
                return false;
            }

            return true;

            #endregion
        }
        #endregion
    }
}
