using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YiNing.UI.Controls;
using static System.Windows.Forms.Control;

namespace YiNing.Tools
{
    public class ControlHelper
    {
        /// <summary>
        /// 获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        private static List<T> GetControls<T>(T t, ControlCollection controls)
        {
            List<T> controlList = new List<T>();
            foreach (var c in controls)
            {
                if (c is T)
                {
                    controlList.Add((T)c);
                }
            }
            return controlList;
        }

        /// <summary>
        /// 批量设置ToggleSwitch的CheckedChanged
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SetToggleSwitchCheckedChanged(Control[] controls, JCS.ToggleSwitch.CheckedChangedDelegate action)
        {
            try
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    foreach (Control c in controls[i].Controls)
                    {
                        if (c is JCS.ToggleSwitch)
                        {
                            JCS.ToggleSwitch button = c as JCS.ToggleSwitch;
                            if (button.Tag != null) button.CheckedChanged += action;
                        }
                        else if (c.Controls.Count > 0)
                        {
                            SetToggleSwitchCheckedChanged(new Control[] { c }, action);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }


        /// <summary>
        /// 批量设置按钮Click
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SetDarkButtonClick(Control[] controls, EventHandler action)
        {
            try
            {
                for(int i=0; i < controls.Length; i++)
                {
                    foreach (Control c in controls[i].Controls)
                    {
                        if (c is DarkButton)
                        {
                            DarkButton button = c as DarkButton;
                            if (button.Tag != null) button.Click += action;
                        }
                        else if (c.Controls.Count > 0)
                        {
                            SetDarkButtonClick(new Control[] { c }, action);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 批量设置按钮MouseDown
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SetButtonMouseDown(Control[] controls, MouseEventHandler action)
        {
            try
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    foreach (Control c in controls[i].Controls)
                    {
                        if (c is Button)
                        {
                            Button button = c as Button;
                            if (button.Tag != null) button.MouseDown += action;
                        }
                        else if (c.Controls.Count > 0)
                        {
                            SetButtonMouseDown(new Control[] { c }, action);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 批量设置按钮MouseUp
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SetButtonMouseUp(Control[] controls, MouseEventHandler action)
        {
            try
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    foreach (Control c in controls[i].Controls)
                    {
                        if (c is Button)
                        {
                            Button button = c as Button;
                            if (button.Tag != null) button.MouseUp += action;
                        }
                        else if (c.Controls.Count > 0)
                        {
                            SetButtonMouseUp(new Control[] { c }, action);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

    }
}
