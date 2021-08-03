using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using WaferAoi.Tools;
using YiNing.Tools;
using System.Threading;
using System.Threading.Tasks;

namespace WaferAoi
{
    public partial class DockSetting : DarkDocument
    {
        #region Field Region
        Config config;
        #endregion

        #region Constructor Region
        public DockSetting()
        {
            InitializeComponent();
            HookEvents();
            Initialize();
            //MotorsControl.OpenDevice(true);
            //Control[] a = dgrpGoHomeAction.Controls.
        }

        public DockSetting(string text, Image icon):this()
        {
            DockText = text;
            Icon = icon;
        }
        #endregion

        #region Method Region
        private void HookEvents()
        {
            timerUpdateUi.Tick += TimerUpdateUi_Tick;
            dlvwAxes.SelectedIndicesChanged += DlvwAxes_SelectedIndicesChanged;
            
            ControlHelper.SetToggleSwitchCheckedChanged(new Control[] { this }, Ts_CheckedChanged);
            //运动控制
            ControlHelper.SetButtonMouseDown(new Control[] { dgrpVehicleAxisRun, dgrpCameraAxisRun }, Button_MouseDown);
            ControlHelper.SetButtonMouseUp(new Control[] { dgrpVehicleAxisRun, dgrpCameraAxisRun }, Button_MouseUp);

            //所有darkbutton
            ControlHelper.SetDarkButtonClick(new Control[] { this }, DarkButton_Click);
        }

        

        private void Initialize()
        {
            timerUpdateUi.Start();
            config = JsonHelper.DeserializeByFile<Config>("yining.config");

            foreach(var axis in config.Axes)
            {
                var item = new DarkListItem(axis.Remarks);
                item.Icon = Icons.x轴;
                item.Tag = axis;
                dlvwAxes.Items.Add(item);
            }
            dlvwAxes.SelectItem(0);
        }

        private Axis GetSelectAxis()
        {
            try
            {
                if (dlvwAxes.SelectedIndices.Count > 0)
                {
                    Axis axis = dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag as Axis;
                    return axis;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception er) { return null; }
        }

        /// <summary>
        /// 更新页面上的轴状态信息
        /// </summary>
        /// <param name="axis"></param>
        private void UpdateAxisUi(Axis axis)
        {
            #region 查询轴状态
            try
            {
                if (axis == null) return;
                if (!this.Created) return;
                int iAxisSts = MotorsControl.GetAxisStatusInt(axis.Id);
                int exi = MotorsControl.IoSignalEXI(4);
                this.BeginInvoke(new Action<Axis, int, int>((ax, axisSts, exiSts) =>
                 {
                     dsepAxisStatus.SectionHeader = ax.Remarks + "-轴状态";

                     toggleSwitch5.Checked = darkInputSignal5.IsSuccess = (axisSts & EMUMS.AxisStatus.ServoOn) != 0;
                     darkInputSignal6.IsSuccess = (axisSts & EMUMS.AxisStatus.Alarm) == 0;
                     darkInputSignal8.IsSuccess = (axisSts & EMUMS.AxisStatus.PLimOn) == 0;
                     darkInputSignal7.IsSuccess = (axisSts & EMUMS.AxisStatus.NLimOn) == 0;
                     MotorsControl.TAxisInfo axisReadInfo = MotorsControl.AxisReadInfo(axis.Id);
                     dtbPlanrfpos.Text = axisReadInfo.prfpos.ToString();
                     dtbEncpos.Text = axisReadInfo.encpos.ToString();
                     dtbPlanVel.Text = axisReadInfo.prfvel.ToString();
                     dtbEncVel.Text = axisReadInfo.encvel.ToString();

                     isEmergencyStop.IsSuccess = (exi & (1 << 0)) == 0;
                     isStart.IsSuccess = (exi & (1 << 1)) != 0;
                     isReset.IsSuccess = (exi & (1 << 2)) != 0;
                     isStop.IsSuccess = (exi & (1 << 3)) != 0;
                     isDoor.IsSuccess = (exi & (1 << 4)) != 0;
                     isPositivePressure.IsSuccess = (exi & (1 << 7)) != 0;
                     isNegativePressure1.IsSuccess = (exi & (1 << 8)) != 0;
                     isNegativePressure2.IsSuccess = (exi & (1 << 9)) != 0;
                     isNegativePressure3.IsSuccess = (exi & (1 << 10)) != 0;
                 }), axis, iAxisSts, exi);
            }
            catch (Exception er) { }

            #endregion
        }
        /// <summary>
        /// 保存轴相关信息
        /// </summary>
        /// <param name="type"></param>
        private void SaveAxisConfig(string type)
        {
            if (dlvwAxes.SelectedIndices != null && dlvwAxes.SelectedIndices.Count > 0)
            {
                Axis axis = dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag as Axis;
                switch (type)
                {
                    case "JOG运动参数保存":
                        #region
                        axis.JogPrm.Acc = double.Parse(dtbJogAcc.Text);
                        axis.JogPrm.Dec = double.Parse(dtbJogDec.Text);
                        axis.JogPrm.Smooth = double.Parse(dtbJogSmooth.Text);
                        axis.JogPrm.Vel = double.Parse(dtbJogVel.Text);
                        #endregion
                        break;
                    case "点位运动参数保存":
                        #region
                        axis.TrapPrm.Acc = double.Parse(dtbTrapAcc.Text);
                        axis.TrapPrm.Dec = double.Parse(dtbTrapDec.Text);
                        axis.TrapPrm.SmoothTime = Int16.Parse(dtbTrapSmoothTime.Text);
                        axis.TrapPrm.Vel = double.Parse(dtbTrapVel.Text);
                        axis.TrapPrm.VelStart = double.Parse(dtbTrapVel.Text) / 2;
                        #endregion
                        break;
                    case "回原点参数保存":
                        #region
                        axis.GoHomePar.modeText = cmbGoHomeType.Text;
                        axis.GoHomePar.moveDir = rdoGoHomeStartPositive.Checked ? (short)1 : (short)-1;
                        axis.GoHomePar.indexDir = rdoGoHomeSearchPositive.Checked ? (short)1 : (short)-1;
                        axis.GoHomePar.homeOffset = int.Parse(dtbGoHomeOffset.Text);
                        axis.GoHomePar.acc = double.Parse(dtbGoHomeAcc.Text);
                        axis.GoHomePar.dec = double.Parse(dtbGoHomeDec.Text);
                        axis.GoHomePar.velLow = double.Parse(dtbGoHomeLocationVel.Text);
                        axis.GoHomePar.velHigh = double.Parse(dtbGoHomeSearchVel.Text);
                        #endregion
                        break;
                    case "轴位置1保存":
                        #region
                        axis.StartPoint = int.Parse(dtbPlanrfpos.Text);
                        #endregion
                        break;
                    case "轴位置2保存":
                        #region
                        axis.EndPoint = int.Parse(dtbPlanrfpos.Text);
                        #endregion
                        break;
                }
                dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag = axis;
                JsonHelper.Serialize(config, "yining.config");
            }
        }
        #endregion

        #region Event Handler Region
        /// <summary>
        /// 实时更新页面数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUpdateUi_Tick(object sender, EventArgs e)
        {
            UpdateAxisUi(GetSelectAxis());
        }
        private void DlvwAxes_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (dlvwAxes.SelectedIndices != null && dlvwAxes.SelectedIndices.Count > 0)
            {
                try
                {
                    toggleSwitch5.CheckedChanged -= Ts_CheckedChanged;

                    Debug.WriteLine(dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Text);
                    Axis axis = dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag as Axis;
                    #region 参数显示
                    dtbJogAcc.Text = axis.JogPrm.Acc.ToString();
                    dtbJogDec.Text = axis.JogPrm.Dec.ToString();
                    dtbJogSmooth.Text = axis.JogPrm.Smooth.ToString();
                    dtbJogVel.Text = axis.JogPrm.Vel.ToString();

                    dtbTrapAcc.Text = axis.TrapPrm.Acc.ToString();
                    dtbTrapDec.Text = axis.TrapPrm.Dec.ToString();
                    dtbTrapSmoothTime.Text = axis.TrapPrm.SmoothTime.ToString();
                    dtbTrapVel.Text = axis.TrapPrm.Vel.ToString();


                    cmbGoHomeType.Text = axis.GoHomePar.modeText;
                    if (axis.GoHomePar.moveDir > 0) rdoGoHomeStartPositive.Checked = true; else rdoGoHomeStartNegative.Checked = true;
                    if (axis.GoHomePar.indexDir > 0) rdoGoHomeSearchPositive.Checked = true; else rdoGoHomeSearchNegative.Checked = true;
                    dtbGoHomeOffset.Text = axis.GoHomePar.homeOffset.ToString();
                    dtbGoHomeAcc.Text = axis.GoHomePar.acc.ToString();
                    dtbGoHomeDec.Text = axis.GoHomePar.dec.ToString();
                    dtbGoHomeLocationVel.Text = axis.GoHomePar.velLow.ToString();
                    dtbGoHomeSearchVel.Text = axis.GoHomePar.velHigh.ToString();
                    #endregion
                    UpdateAxisUi(axis);
                }
                catch (Exception er) { LogHelper.WriteLog("err", er); }
                finally
                {
                    toggleSwitch5.CheckedChanged += Ts_CheckedChanged;
                }
            
            }
        }
        /// <summary>
        /// 所有的darkbutton点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkButton_Click(object sender, EventArgs e)
        {
            Axis axis = GetSelectAxis();

            DarkButton btn = sender as DarkButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                #region 运动参数设置和单轴的操作
                case "单轴清除状态":
                    if (axis != null) MotorsControl.ClearSts(axis.Id);
                    break;
                case "单轴位置清零":
                    axis = GetSelectAxis();
                    if (axis != null) MotorsControl.ZeroPos(axis.Id);
                    break;
                case "单轴启动回零":
                    axis = GetSelectAxis();
                    if (axis != null) new DialogGoHome(axis).ShowDialog();
                    break;
                case "JOG运动参数保存":
                case "点位运动参数保存":
                case "回原点参数保存":
                case "轴位置1保存":
                case "轴位置2保存":
                    SaveAxisConfig(type);
                    break;
                    #endregion
            }
            UpdateAxisUi(axis);
        }
        /// <summary>
        /// 运动控制停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("停止运动");
            Axis axis = GetSelectAxis();
            MotorsControl.StopAxis(axis.Id, 0);
        }

        /// <summary>
        /// 运动控制，开始运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            switch (btn.Tag.ToString())
            {
                case "前进": axisId = 1; direction = -1; break; // 晶圆载具X轴状态
                case "后退": axisId = 1; direction = 1; break;// 晶圆载具X轴状态
                case "左移": axisId = 2; direction = 1; break;//"晶圆载具Y轴状态";
                case "右移": axisId = 2; direction = -1; break;//晶圆载具Y轴状态";
                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            int index = config.Axes.FindIndex(a=>a.Id == axisId);
            dlvwAxes.SelectItem(index);
            dsepAxisStatus.SectionHeader = axis.Remarks + "-状态";
            MotorsControl.MoveJog(axis.Id, axis.JogPrm.Get(), axis.JogPrm.Vel, direction);
        }

        /// <summary>
        /// 输出控制操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ts_CheckedChanged(object sender, EventArgs e)
        {
            var Ts = sender as JCS.ToggleSwitch;
            switch (Ts.Tag.ToString())
            {
                case "启动灯":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.StartLight, Ts.Checked ? 0 : 1);
                    break;
                case "复位灯":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.ResetLight, Ts.Checked ? 0 : 1);
                    break;
                case "停止灯":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.StopLight, Ts.Checked ? 0 : 1);
                    break;
                case "三色灯(黄)":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.ThreeLightYellow, Ts.Checked ? 0 : 1);
                    break;
                case "三色灯(红)":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.ThreeLightRed, Ts.Checked ? 0 : 1);
                    break;
                case "三色灯(绿)":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.ThreeLightGreen, Ts.Checked ? 0 : 1);
                    break;
                case "蜂鸣器":
                    MotorsControl.IoSignalEXO((int)EMUMS.IOPointsOut.Buzzer, Ts.Checked ? 0 : 1);
                    break;
                case "顶升气缸":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.JackingAir, Ts.Checked ? 1 : 0);                    break;
                case "裸片阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, Ts.Checked ? 1 : 0);
                    break;
                case "6寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.SixInchesAir, Ts.Checked ? 1 : 0);
                    break;
                case "8寸阀":
                    MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.EightInchesAir, Ts.Checked ? 1 : 0);
                    break;
                case "所有轴使能开关":
                    break;
                case "单轴使能开关":
                    Axis axis = dlvwAxes.Items[dlvwAxes.SelectedIndices[0]].Tag as Axis;
                    if (Ts.Checked) MotorsControl.ServoOn(axis.Id);
                    else MotorsControl.ServoOff(axis.Id);
                    break;
            }
        }
        public override void Close()
        {
            var result = DarkMessageBox.ShowWarning(@"确定要关闭么", @"提醒", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;

            base.Close();
        }

        #endregion

        //private void darkButton9_Click(object sender, EventArgs e)
        //{
        //    DarkMessageBox.ShowWarning(darkSectionPanel3.Location.X + "," + darkSectionPanel3.Location.Y, "location", DarkDialogButton.Ok);
        //}
    }
}
