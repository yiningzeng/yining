﻿using YiNing.UI.Docking;
using YiNing.UI.Forms;
using YiNing.UI.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;

namespace WaferAoi
{
    public partial class MainForm : DarkForm
    {

        #region Field Region

        private List<DarkDockContent> _toolWindows = new List<DarkDockContent>();

        private DockProject _dockProject;
        private DockProperties _dockProperties;
        private DockConsole _dockConsole;
        private DockLayers _dockLayers;
        private DockHistory _dockHistory;


        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;
        #endregion

        #region Constructor Region

        public MainForm()
        {
            InitializeComponent();
            // Add the control scroll message filter to re-route all mousewheel events
            // to the control the user is currently hovering over with their cursor.
            Application.AddMessageFilter(new ControlScrollFilter());

            // Add the dock content drag message filter to handle moving dock content around.
            Application.AddMessageFilter(DockPanel.DockContentDragFilter);

            // Add the dock panel message filter to filter through for dock panel splitter
            // input before letting events pass through to the rest of the application.
            Application.AddMessageFilter(DockPanel.DockResizeFilter);

            // Hook in all the UI events manually for clarity.
            HookEvents();

            // Build the tool windows and add them to the dock panel
            _dockProject = new DockProject();
            _dockProperties = new DockProperties();
            _dockConsole = new DockConsole();
            _dockLayers = new DockLayers();
            _dockHistory = new DockHistory();

            // Add the tool windows to a list
            _toolWindows.Add(_dockProject);
            _toolWindows.Add(_dockProperties);
            _toolWindows.Add(_dockConsole);
            _toolWindows.Add(_dockLayers);
            _toolWindows.Add(_dockHistory);

            // Deserialize if a previous state is stored
            if (File.Exists("dockpanel.config"))
            {
                DeserializeDockPanel("dockpanel.config");
            }
            else
            {
                // Add the tool window list contents to the dock panel
                foreach (var toolWindow in _toolWindows)
                    DockPanel.AddContent(toolWindow);

                // Add the history panel to the layer panel group
                DockPanel.AddContent(_dockHistory, _dockLayers.DockGroup);
            }

            // Check window menu items which are contained in the dock panel
            BuildWindowMenu();

            // Add dummy documents to the main document area of the dock panel
            //DockPanel.AddContent(new DockDocument("Document 2", Icons.document_16xLG) { ShowCloseButton = true });
            //DockPanel.AddContent(new DockDocument("Document 3", Icons.document_16xLG) { ShowCloseButton = true });
            DockPanel.AddContent(new DockWorkSpace("工作站", Icons.ChipOutline) { ShowCloseButton = false });
        }

        #endregion

        #region Method Region

        private void HookEvents()
        {
            mnuMain.MouseDown += Move_MouseDown;

            FormClosing += MainForm_FormClosing;

            DockPanel.ContentAdded += DockPanel_ContentAdded;
            DockPanel.ContentRemoved += DockPanel_ContentRemoved;

            mnuNewFile.Click += NewFile_Click;
            mnuClose.Click += Close_Click;

            btnNewFile.Click += NewFile_Click;

            mnuDialog.Click += Dialog_Click;

            mnuProject.Click += Project_Click;
            mnuProperties.Click += Properties_Click;
            mnuConsole.Click += Console_Click;
            mnuLayers.Click += Layers_Click;
            mnuHistory.Click += History_Click;

            mnuAbout.Click += About_Click;
            参数设置ToolStripMenuItem.Click += 参数设置ToolStripMenuItem_Click;
            点位测试ToolStripMenuItem.Click += 点位测试ToolStripMenuItem_Click;
        }

        private void ToggleToolWindow(DarkToolWindow toolWindow)
        {
            if (toolWindow.DockPanel == null)
                DockPanel.AddContent(toolWindow);
            else
                DockPanel.RemoveContent(toolWindow);
        }

        private void BuildWindowMenu()
        {
            mnuProject.Checked = DockPanel.ContainsContent(_dockProject);
            mnuProperties.Checked = DockPanel.ContainsContent(_dockProperties);
            mnuConsole.Checked = DockPanel.ContainsContent(_dockConsole);
            mnuLayers.Checked = DockPanel.Contains(_dockLayers);
            mnuHistory.Checked = DockPanel.Contains(_dockHistory);
        }

        #endregion

        #region Event Handler Region
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Move_MouseDown(object sender, MouseEventArgs e)
        {
            //为当前应用程序释放鼠标捕获
            ReleaseCapture();
            //发送消息 让系统误以为在标题栏上按下鼠标
            SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SerializeDockPanel("dockpanel.config");
        }

        private void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            if (_toolWindows.Contains(e.Content))
                BuildWindowMenu();
        }

        private void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            if (_toolWindows.Contains(e.Content))
                BuildWindowMenu();
        }

        private void NewFile_Click(object sender, EventArgs e)
        {
            var newFile = new DockDocument("New document", Icons.document_16xLG);
            DockPanel.AddContent(newFile);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region menu
        private void 参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockPanel.AddContent(new DockSetting("运动轴设置", Icons.Tools));
        }
        private void 点位测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockPanel.AddContent(new DockPoints("点位测试", Icons.smile));
        }

        private void Dialog_Click(object sender, EventArgs e)
        {
            var test = new DialogControls();
            test.ShowDialog();
        }

        private void Project_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockProject);
        }

        private void Properties_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockProperties);
        }

        private void Console_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockConsole);
        }

        private void Layers_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockLayers);
        }

        private void History_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockHistory);
        }

        private void About_Click(object sender, EventArgs e)
        {
            var about = new DialogAbout();
            about.ShowDialog();
        }
        #endregion
        #endregion

        #region Serialization Region

        private void SerializeDockPanel(string path)
        {
            var state = DockPanel.GetDockPanelState();
            SerializerHelper.Serialize(state, path);
        }

        private void DeserializeDockPanel(string path)
        {
            var state = SerializerHelper.Deserialize<DockPanelState>(path);
            DockPanel.RestoreDockPanelState(state, GetContentBySerializationKey);
        }

        private DarkDockContent GetContentBySerializationKey(string key)
        {
            foreach (var window in _toolWindows)
            {
                if (window.SerializationKey == key)
                    return window;
            }

            return null;
        }

        #endregion


        Axis GetDefaultAxis(int id, string remark, int vel = 20)
        {
            return new Axis()
            {
                Id = id,
                Remarks = remark,
                StartPoint = 100,
                EndPoint = 100,
                JogPrm = new AxisJogPrm() { Acc = 1, Dec = 1, Smooth = 0.9, Vel = vel },
                TrapPrm = new AxisTrapPrm() { Acc = 1, Dec = 1, Vel = vel, VelStart = 5, SmoothTime = 30 },
                GoHomePar = new AxisGoHomePar()
                {
                    acc = 1,
                    dec = 1,
                    edge = 1,
                    escapeStep = 1000,
                    homeOffset = 0,
                    indexDir = 0,
                    mode = (short)EMUMS.AxisGoHome.HOME_MODE_LIMIT_HOME_INDEX,
                    modeText = "限位+Home+Index回原点(13)",
                    moveDir = 1,
                    searchHomeDistance = 0,
                    searchIndexDistance = 0,
                    smoothTime = (short)0.8,
                    triggerIndex = -1,
                    velHigh = vel * 2,
                    velLow = vel
                }
            };
        }
        private void 恢复默认运动参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = DarkMessageBox.ShowWarning(@"继续操作会丢失您之前设置的运动参数，确定要继续么?", @"提醒", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;
            Config config = new Config();
            config.Axes.Add(GetDefaultAxis(1, "载具X轴"));
            config.Axes.Add(GetDefaultAxis(2, "载具Y轴"));
            config.Axes.Add(GetDefaultAxis(3, "载具旋转轴", 2));
            config.Axes.Add(GetDefaultAxis(4, "载具Z轴"));
            JsonHelper.Serialize(config, "yining.config");
        }
    }
}
