using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YiNing.Fsm;
using YiNing.Tools;
using YiNing.UI.Forms;

namespace WaferAoi.Tools
{
    public class FsmHelper
    {
        static Config config;
        static CommunicationManager nosepieceCom;
        public FiniteStateMachine<MacroStatus, MacroAction> MacroFsm = FiniteStateMachine<MacroStatus, MacroAction>.FromEnum();
        /// <summary>
        /// 整机宏观的状态
        /// </summary>
        public enum MacroStatus
        {
            On,         //控制卡被打开
            Off,        //控制卡关闭
            INIT,       //初始态
            STOP,       //停止态
            RUN,        //运行态, 微观的状态都在宏观RUN的状态可以运行
            RESET,      //复位态
            SCRAM,      //急停态
            PAUSE,      //暂停态
            ALARM,      //警告状态，不需要复位
            ERROR,      //错误停止，需要复位
        }

        public enum MacroAction
        {
            DO_ON,
            DO_OFF,
            DO_INIT,
            DO_STOP,
            DO_RUN,
            DO_RESET,
            DO_SCRAM,
            DO_PAUSE,
            DO_ALARM,
            DO_ERROR,
        }
        Func<bool> DO_ON = () =>
        {
            if (!MotorsControl.OpenDevice(true))
            {
                DarkMessageBox.ShowError("打开运动控制器出错，即将退出程序，请联系技术人员！");
                Application.Exit();
                return false;
            }
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
            if (config != null)
            {
                foreach (var i in config.Axes)
                {
                    MotorsControl.ClearSts(i.Id);
                    MotorsControl.IoSignalEXO(i.Id, 1, 11);
                    MotorsControl.ServoOn(i.Id);
                }
            }
            else
            {
                DarkMessageBox.ShowError("获取控制参数失败，请联系技术人员！");
                return false;
            }
            nosepieceCom = new CommunicationManager();
            nosepieceCom.PortName = NosepieceData.PortName = config.NosepieceCom;
            nosepieceCom.Parity = "None";
            nosepieceCom.StopBits = "One";
            nosepieceCom.DataBits = "8";
            nosepieceCom.BaudRate = "115200";
            if (!nosepieceCom.OpenPort()) { DarkMessageBox.ShowError("连接物镜旋转盘出错！"); return false; }
            nosepieceCom.CurrentTransmissionType = CommunicationManager.TransmissionType.Hex;
            nosepieceCom.WriteData(NosepieceData.Select5BingHoleHex);
            return true;
        };
        Func<bool> DO_OFF = () => { MotorsControl.CloseDevice(); return true; };
        /// <summary>
        /// 初始化
        /// </summary>
        Func<bool> DO_INIT = () =>
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
            nosepieceCom.WriteData(NosepieceData.Hole1Hex);
            if (config != null)
            {
                foreach (var i in config.Axes)
                {
                    //if (!i.Remarks.Contains("Z")) continue;
                   MotorsControl.GoHomeAsync(i.Id, i.GoHomePar.Get(), new Action<short, GSN.THomeStatus>((a, b) =>
                    {

                    }));
                }
            }
            else
            {
                DarkMessageBox.ShowError("获取控制参数失败，请联系技术人员！");
                return false;
            }
            return true;
        };
        Func<bool> DO_STOP = () => { return true; };
        Func<bool> DO_RUN = () => { return true; };
        Func<bool> DO_RESET = () => { return true; };
        Func<bool> DO_SCRAM = () => { return true; };
        Func<bool> DO_PAUSE = () => { return true; };
        Func<bool> DO_ALARM = () => { return true; };
        Func<bool> DO_ERROR = () => { return true; };

        public FsmHelper()
        {
            MacroFsm.Begin(MacroStatus.Off);
            MacroFsm.AddTransition(MacroStatus.Off, MacroStatus.On, MacroAction.DO_ON, DO_ON)
                .AddTransition(MacroStatus.On, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)
                .AddTransition(MacroStatus.INIT, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)
                .AddTransition(MacroStatus.INIT, MacroStatus.ERROR, MacroAction.DO_ERROR, DO_ERROR)

                .AddTransition(MacroStatus.INIT, MacroStatus.SCRAM, MacroAction.DO_SCRAM)
                .AddTransition(MacroStatus.INIT, MacroStatus.RESET, MacroAction.DO_RESET, DO_RESET)
                .AddTransition(MacroStatus.INIT, MacroStatus.RUN, MacroAction.DO_RUN, DO_RUN)

                .AddTransition(MacroStatus.RUN, MacroStatus.PAUSE, MacroAction.DO_PAUSE, DO_PAUSE)
                .AddTransition(MacroStatus.RUN, MacroStatus.STOP, MacroAction.DO_STOP, DO_STOP)
                .AddTransition(MacroStatus.RUN, MacroStatus.SCRAM, MacroAction.DO_SCRAM, DO_SCRAM)
                .AddTransition(MacroStatus.RUN, MacroStatus.ERROR, MacroAction.DO_ERROR, DO_ERROR)
                .AddTransition(MacroStatus.RUN, MacroStatus.ALARM, MacroAction.DO_ALARM, DO_ALARM)
                .AddTransition(MacroStatus.RUN, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)
                .AddTransition(MacroStatus.INIT, MacroStatus.Off, MacroAction.DO_OFF, DO_OFF)


                .AddTransition(MacroStatus.PAUSE, MacroStatus.RUN, MacroAction.DO_RUN, DO_RUN)
                .AddTransition(MacroStatus.STOP, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)
                .AddTransition(MacroStatus.SCRAM, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)
                .AddTransition(MacroStatus.ERROR, MacroStatus.INIT, MacroAction.DO_INIT, DO_INIT)

                .AddTransition(MacroStatus.ALARM, MacroStatus.RUN, MacroAction.DO_RUN, DO_RUN); // 这个有待争议，警告了之后是否只是暂停还可继续
        }

        public void IssueCommand(MacroAction ac)
        {
            MacroFsm.IssueCommand(ac);
        }
        public void UpdateConfig()
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
        }

        public static Config GetConfig()
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
            return config;
        }


        /// <summary>
        /// 改变物镜
        /// </summary>
        /// <param name="id"></param>
        public static void ChangeLense(int id)
        {
            string hex = "";
            switch (id)
            {
                case 0:
                    hex = NosepieceData.Hole1Hex;
                    break;
                case 1:
                    hex = NosepieceData.Hole2Hex;
                    break;
                case 2:
                    hex = NosepieceData.Hole3Hex;
                    break;
                case 3:
                    hex = NosepieceData.Hole4Hex;
                    break;
                case 4:
                    hex = NosepieceData.Hole5Hex;
                    break;
            }
            nosepieceCom.WriteData(hex);
        }




        public enum Status
        {
            On,             //控制卡被打开
            Off,            //控制卡关闭
            Initialized,    //已经初始化
            Freed,          //空闲
            Warned,         //
            DoorOpened,     //
            ManualFeeded,   //
            RobotFeeded,    //
            EditReadied,    //
            WorkReadied,    //
            Working,        //
        }

        public enum Action
        {
            On,
            Off,
            Initialize,
            Free,
            Warn,
            OpenDoor,
            ManualFeed,
            RobotFeed,
            Edit,
            Work,
        }
    }
}
