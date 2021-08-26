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

        /// <summary>
        /// 整机宏观的状态
        /// </summary>
        public enum MacroStatus
        {
            INIT,       //初始态
            STOP,       //停止态
            RUN,        //运行态, 微观的状态都在宏观RUN的状态可以运行
            RESET,      //复位态
            SCRAM,      //急停态
            PAUSE,      //暂停态
            ALARM,      //警告状态，不需要复位
            ERROR,      //错误停止，需要复位
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

        static Config config;
        static CommunicationManager nosepieceCom;
        public FiniteStateMachine<Status, Action> machine = FiniteStateMachine<Status, Action>.FromEnum();

        public FsmHelper()
        {
            machine.Begin(Status.Off);
            machine.AddTransition(Status.Off, Status.On, Action.On, On)
                .AddTransition(Status.On, Status.Off, Action.Off, () => { MotorsControl.CloseDevice(); return true; })
                .AddTransition(Status.On, Status.Initialized, Action.Initialize, Initialize)
                .AddTransition(Status.Initialized, Status.Freed, Action.Free, Free);
        }

        public void IssueCommand(Action ac)
        {
            machine.IssueCommand(ac);
        }

        public Status CurrentState()
        {
            return machine.CurrentState;
        }

        public Status PreviousState()
        {
            return machine.PreviousState;
        }

        public void UpdateConfig()
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
        }

        #region func

        Func<bool> On = () =>
        {
            if (!MotorsControl.OpenDevice(true))
            {
                DarkMessageBox.ShowError("打开运动控制器出错，即将退出程序，请联系技术人员！");
                Application.Exit();
                return false;
            }
            return true;
        };

        /// <summary>
        /// 初始化
        /// </summary>
        Func<bool> Initialize = () =>
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
            if (config != null)
            {
                foreach (var i in config.Axes)
                {
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

        /// <summary>
        /// 回原点操作
        /// </summary>
        Func<bool> Free = () =>
        {
            config = JsonHelper.DeserializeByFile<Config>("yining.config");
            nosepieceCom.WriteData(NosepieceData.Hole1Hex);
            if (config != null)
            {
                foreach (var i in config.Axes)
                {
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

        #endregion
    }
}
