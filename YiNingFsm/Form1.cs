using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YiNing.Fsm
{
    public partial class Form1 : Form
    {
        class HotKey
        {
            //如果函数执行成功，返回值不为0。 
            //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。 
            [DllImport("user32.dll ", SetLastError = true)]
            public static extern bool RegisterHotKey(
                    IntPtr hWnd,                                 //要定义热键的窗口的句柄 
                    int id,                                           //定义热键ID（不能与其它ID重复）                       
                    KeyModifiers fsModifiers,       //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效 
                    Keys vk                                           //定义热键的内容 
                    );

            [DllImport("user32.dll ", SetLastError = true)]
            public static extern bool UnregisterHotKey(
                    IntPtr hWnd,                                 //要取消热键的窗口的句柄 
                    int id                                             //要取消热键的ID 
                    );

            //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值） 
            [Flags()]
            public enum KeyModifiers
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
            }
        }

        WafermapImpl wmap = new WafermapImpl();
        public enum DoorStates
        {
            OPEN,
            CLOSED,
            LOCKED
        }

        public enum DoorCommand
        {
            DoOpen,
            DoClose,
            DoLock,
            DoUnlock
        }
        FiniteStateMachine<DoorStates, DoorCommand> fsm = FiniteStateMachine<DoorStates, DoorCommand>.FromEnum();
        //FiniteStateMachine<string, DoorCommand> fsm2 = new FiniteStateMachine<string, DoorCommand>("","","");
        public Form1()
        {
            InitializeComponent();
            HotKey.RegisterHotKey(this.Handle, 100, HotKey.KeyModifiers.Ctrl, Keys.F1);
            fsm.AddTransition(DoorStates.CLOSED, DoorStates.OPEN, DoorCommand.DoOpen, () => {
                MessageBox.Show("DoOpen");
                return true;
            })
               .AddTransition(DoorStates.LOCKED, DoorStates.CLOSED, DoorCommand.DoUnlock, () => true) // using a condition
               .AddTransition(DoorStates.OPEN, DoorStates.CLOSED, DoorCommand.DoClose, () => {
                   MessageBox.Show("DoClose");
                   return true;
               })
               .AddTransition(DoorStates.CLOSED, DoorStates.LOCKED, DoorCommand.DoLock,()=> {
                   DialogResult result = MessageBox.Show("确定要锁门么", "notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                   if (result == DialogResult.OK)
                   {
                       return true;
                   }
                   else
                   {
                       return false;
                   }
               }).OnChange(DoorStates.CLOSED, DoorStates.LOCKED, ()=> { Debug.WriteLine("我正在转换，从关闭到上锁"); })
               .OnEnter(DoorStates.LOCKED, ()=>MessageBox.Show("enter locked"))
               .OnEnter(DoorStates.OPEN, () => Debug.WriteLine("enter open!"))
               .OnExit(DoorStates.CLOSED, () => Debug.WriteLine("we exit closed!"));
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键   
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            MessageBox.Show("Press   F1 ");
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fsm.Begin(DoorStates.CLOSED);
            DoorStates aa = fsm.CurrentState;
            var bb = fsm.PreviousState;
            fsm.IssueCommand(DoorCommand.DoLock); // door should now be open
        
             aa = fsm.CurrentState;
            bb = fsm.PreviousState;
            fsm.IssueCommand(DoorCommand.DoOpen); // nothing will happen (no transition from open using lock command)

            aa = fsm.CurrentState;
            bb = fsm.PreviousState;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int waferNum = 0; waferNum < 1; waferNum++)
            {
                // Create sample dataset
                int[,] data = new int[41, 40];
                Random binGenerator = new Random(waferNum);
                for (int x = 0; x < 41; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        data[x, y] = binGenerator.Next(8);
                    }
                }
                int a = data.Length;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                wmap.Dataset = data;
                wmap.Notchlocation = 0;
                //wmap.MinimumSize = new Size(500, 500);
                wmap.Interactive = true;
                wmap.Dock = DockStyle.Fill;
                wmap.SelectX = 21;
                wmap.SelectY = 14;
                wmap.SelectBincode = data[21,14];
                //wmap.NoDataString = "没有数据";
                //this.Controls.Add(wmap);
                panel1.Controls.Add(wmap);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }

        }
         
        private void button3_Click(object sender, EventArgs e)
        {
            wmap.SelectX = int.Parse(tbX.Text);
            wmap.SelectY = int.Parse(tbY.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int a = 4 / 3;
            a = 4 % 4;
        }
    }
}
