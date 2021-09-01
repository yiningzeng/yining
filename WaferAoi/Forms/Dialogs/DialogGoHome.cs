using System;
using System.Diagnostics;
using WaferAoi.Tools;
using YiNing.UI.Forms;

namespace WaferAoi
{
    public partial class DialogGoHome : DarkDialog
    {
        Axis axis = null;
        #region Constructor Region

        public DialogGoHome(Axis ax)
        {
            this.axis = ax;
            InitializeComponent();
            this.Text = axis.Remarks + "-回零状态";
            this.Load += DialogGoHome_Load;
        }

        private void DialogGoHome_Load(object sender, EventArgs e)
        {
            MotorsControl.GoHomeAsync(axis.Id, axis.GoHomePar.Get(), new Action<short, GSN.THomeStatus>((id, par) =>
            {
                this.BeginInvoke(new Action<GSN.THomeStatus>((homepar) =>
                {
                    lbStatus.Text = homepar.run == 1 ? "正在回原点" : "已停止运动";
                    switch (homepar.stage)
                    {
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_IDLE: lbStage.Text = "未启动Smart Home回原点"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_START: lbStage.Text = "启动Smart Home回原点"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_LIMIT: lbStage.Text = "正在寻找限位"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_LIMIT_STOP: lbStage.Text = "触发限位停止"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_LIMIT_ESCAPE: lbStage.Text = "反方向运动脱离限位"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_LIMIT_RETURN: lbStage.Text = "重新回到限位"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_LIMIT_RETURN_STOP: lbStage.Text = "重新回到限位停止"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_HOME: lbStage.Text = "正在搜索Home"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_HOME_RETURN: lbStage.Text = "搜索到Home后运动到捕获的Home位置"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_SEARCH_INDEX: lbStage.Text = "正在搜索Index"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_GO_HOME: lbStage.Text = "正在执行回原点过程"; break;
                        case (short)EMUMS.AxisGoHomeStage.HOME_STAGE_END: lbStage.Text = "回原点结束"; break;
                    }
                    switch (homepar.error)
                    {
                        case (short)EMUMS.AxisGoHomeError.HOME_ERROR_NONE: lbError.Text = "未发生错误"; break;
                        case (short)EMUMS.AxisGoHomeError.HOME_ERROR_NOT_TRAP_MODE: lbError.Text = "回原点的轴不是处于点位运动模式"; break;
                    }
                    lbCapturePos.Text = homepar.capturePos.ToString();
                    lbTargetPos.Text = homepar.targetPos.ToString();
                    if (homepar.run == 0) Done(DarkDialogButton.Ok);
                }), par);
            }));
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            int iAxisSts = MotorsControl.GetAxisStatusInt(axis.Id);
            darkInputSignal1.IsSuccess = (iAxisSts & EMUMS.AxisStatus.PLimOn) == 0;
            darkInputSignal2.IsSuccess = (iAxisSts & EMUMS.AxisStatus.NLimOn) == 0;
        }
    }
}
