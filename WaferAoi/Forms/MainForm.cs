using YiNing.UI.Docking;
using YiNing.UI.Forms;
using YiNing.UI.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using System.Threading;
using System.Linq;
using YiNing.UI.Controls;
using HalconDotNet;

namespace WaferAoi
{
    public partial class MainForm : DarkForm
    {

        #region Field Region
        MVCameraHelper mVCameraHelper;
        public FsmHelper fsmHelper = new FsmHelper();
        private List<DarkDockContent> _toolWindows = new List<DarkDockContent>();

        public DockWaferList _dockWaferList;
        public DockProperties _dockProperties;
        public DockControl _dockControl;
        public DockLayers _dockLayers;
        public DockHistory _dockHistory;
        public DockWorkSpace _dockWorkSpace;


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
            fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_ON);
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
            mVCameraHelper = new MVCameraHelper(2);
            // Build the tool windows and add them to the dock panel
            _dockWaferList = new DockWaferList(this);
            _dockProperties = new DockProperties();
            _dockControl = new DockControl(this);
            _dockLayers = new DockLayers();
            _dockHistory = new DockHistory();
            _dockWorkSpace = new DockWorkSpace(this, mVCameraHelper, "工作站", Icons.ChipOutline) { ShowCloseButton = false };
            // Add the tool windows to a list
            _toolWindows.Add(_dockWaferList);
            _toolWindows.Add(_dockProperties);
            _toolWindows.Add(_dockControl);
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
            DockPanel.AddContent(_dockWorkSpace);
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
            DebugToolStripMenuItem.Click += DebugToolStripMenuItem_Click;
            Load += MainForm_Load;
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
            mnuProject.Checked = DockPanel.ContainsContent(_dockWaferList);
            mnuProperties.Checked = DockPanel.ContainsContent(_dockProperties);
            mnuConsole.Checked = DockPanel.ContainsContent(_dockControl);
            mnuLayers.Checked = DockPanel.Contains(_dockLayers);
            mnuHistory.Checked = DockPanel.Contains(_dockHistory);
        }

        #endregion

        #region Event Handler Region
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (DarkMessageBox.ShowInformation("是否需要一键回原点", buttons: DarkDialogButton.YesNo) == DialogResult.Yes)
            {
#if DEBUG
                MotorsControl.IoExtSignalEXO(EMUMS.IOPointsOutExt.DieAir, 1);
#endif
                fsmHelper.IssueCommand(FsmHelper.MacroAction.DO_INIT);
            }
            var aa = GetLatestFiles(@"D:\WaferDataIn\mapping");
        }

        private string[] GetLatestFiles(string Path)
        {
            var query = from f in Directory.GetFiles(Path, "*.txt")
                        let fi = new FileInfo(f)
                        orderby fi.CreationTime descending
                        select fi.Name;
            return query.ToArray();
        }
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
            MotorsControl.CloseDevice();
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
        private void DebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockPanel.AddContent(new DockDebugs("开发功能测试", Icons.smile, ref mVCameraHelper));
        }

        private void Dialog_Click(object sender, EventArgs e)
        {
            var test = new DialogControls();
            test.ShowDialog();
        }

        private void Project_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockWaferList);
        }

        private void Properties_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockProperties);
        }

        private void Console_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockControl);
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


        Axis GetDefaultAxis(int id, string remark, int vel = 10)
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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //fsmHelper.IssueCommand(FsmHelper.Action.ManualFeed);
        }

        private void 新建程式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form1 form1 = new Form1();
            //form1.ShowDialog();
            DialogNewProject dialogNewProject = new DialogNewProject();
            dialogNewProject.ShowDialog(this);
            if (dialogNewProject.DialogResult == DialogResult.OK)
            {
                DockPanel.AddContent(new DockSoftwareEdit("程式制作", Icons.Cup, ref mVCameraHelper, (ProgramConfig)dialogNewProject.Tag));
            }
        }

        private void 计算像元ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DialogCalPixel(ref mVCameraHelper).ShowDialog();
        }

        private void 飞拍矫正测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DialogFlyPhoto(ref mVCameraHelper).Show();
        }

        private void 检测结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DialogDetectResult().ShowDialog();
        }

        private void toolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = sender as ToolStripButton;
            switch (toolStripButton.Text)
            {
                case "运行程式":

                    break;
            }
        }

        private void 运行程式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogSelectProgram aa =  new DialogSelectProgram();
            aa.ShowDialog();
            if (aa.DialogResult == DialogResult.OK)
            {
                string[] programName = aa.Tag.ToString().Split('@');
                _dockWorkSpace.SetProgress(programName[0], programName[1]);
            }
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramConfig cccc = JsonHelper.DeserializeByFile<ProgramConfig>(@"D:\QTWaferProgram\fofofofofof\config.zyn");
            HOperatorSet.ReadImage(out HObject ho, @"D:\QTWaferProgram\fofofofofof\main.bmp");
            new DialogCreateModel(ho, cccc, new System.Drawing.Point(1013, 498), new System.Drawing.Point(3052, 2550)).Show();
        }

        private void 模型测试测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DialogTest().Show();
        }
    }
}
