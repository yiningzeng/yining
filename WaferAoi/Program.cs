using HalconDotNet;
using System;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using YiNing.UI.Forms;

namespace WaferAoi
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            HOperatorSet.SetSystem("do_low_error", "false"); ///少报错
            HOperatorSet.SetSystem("clip_region", "true"); //region在图像外不切掉
            HOperatorSet.SetSystem("border_shape_models", "true"); //依然匹配边缘的图形
            HOperatorSet.SetSystem("use_window_thread", "true");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            bool Exist;//定义一个bool变量，用来表示是否已经运行
                       //创建Mutex互斥对象
            System.Threading.Mutex newMutex = new System.Threading.Mutex(true, "WaferAoi-轻蜓视觉", out Exist);
            if (Exist)//如果没有运行
            {
                newMutex.ReleaseMutex();//运行新窗体
                MainForm mainForm = new MainForm();
                mainForm.FormClosing += MainForm_FormClosing;
                Application.Run(mainForm);

            }
            else
            {
                DarkMessageBox.ShowError("本程序一次只能运行一个实例！", "提示");
            }
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MotorsControl.CloseDevice();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.WriteLog("AppDomain中遇到未处理异常：" + e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogHelper.WriteLog("Application中遇到未处理异常：" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
    }
}
