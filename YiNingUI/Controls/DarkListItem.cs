using YiNing.UI.Config;
using System;
using System.Drawing;

namespace YiNing.UI.Controls
{
    public class DarkListItem
    {
        #region Event Region

        public event EventHandler TextChanged;

        #endregion

        #region Field Region

        private string _text;

        private bool _Enabled = true;

        #endregion

        #region Property Region

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                if (TextChanged != null)
                    TextChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 是否可选中和点击
        /// </summary>
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
            }
        }


        public Rectangle Area { get; set; }

        public Color TextColor { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackgroundColor { get; set; }


        public FontStyle FontStyle { get; set; }

        public Bitmap Icon { get; set; }

        public object Tag { get; set; }

        #endregion

        #region Constructor Region

        public DarkListItem()
        {
            TextColor = Colors.LightText;
            FontStyle = FontStyle.Regular;
        }

        public DarkListItem(string text)
            : this()
        {
            Text = text;
        }

        #endregion
    }
}
