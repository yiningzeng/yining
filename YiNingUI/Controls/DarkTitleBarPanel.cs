using YiNing.UI.Config;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace YiNing.UI.Docking
{
    public class DarkTitleBarPanel : Panel
    {
        #region Field Region

        private Rectangle _headerRect;
        private Image _icon;
        private string _headerText;
        private bool _showHeader = true;
        #endregion

        #region Property Region
        [Category("Appearance")]
        [Description("")]
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("标题")]
        [DefaultValue("YiNing")]
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("是否显示标题栏")]
        [DefaultValue(true)]
        public bool ShowHeader
        {
            get { return _showHeader; }
            set
            {
                _showHeader = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get { return base.Padding; }
        }

        #endregion

        #region Constructor Region

        public DarkTitleBarPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            BackColor = Colors.GreyBackground;
            base.Padding = new Padding(0, Consts.ToolWindowHeaderSize, 0, 0);
            UpdateCloseButton();
        }

        #endregion

        #region Method Region

        private void UpdateCloseButton()
        {
            _headerRect = new Rectangle
            {
                X = ClientRectangle.Left,
                Y = ClientRectangle.Top,
                Width = ClientRectangle.Width,
                Height = Consts.ToolWindowHeaderSize
            };
        }

        #endregion

        #region Event Handler Region

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateCloseButton();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Invalidate();
        }

        #endregion

        #region Paint Region

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Fill body
            using (var b = new SolidBrush(Colors.GreyBackground))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            var isActive = true;




            // Draw icon
            if (ShowHeader)
            {
                // Draw header
                var bgColor = isActive ? Colors.BlueBackground : Colors.HeaderBackground;
                var darkColor = isActive ? Colors.DarkBlueBorder : Colors.DarkBorder;
                var lightColor = isActive ? Colors.LightBlueBorder : Colors.LightBorder;

                using (var b = new SolidBrush(bgColor))
                {
                    var bgRect = new Rectangle(0, 0, ClientRectangle.Width, Consts.ToolWindowHeaderSize);
                    g.FillRectangle(b, bgRect);
                }
                using (var p = new Pen(darkColor))
                {
                    g.DrawLine(p, ClientRectangle.Left, 0, ClientRectangle.Right, 0);
                    g.DrawLine(p, ClientRectangle.Left, Consts.ToolWindowHeaderSize - 1, ClientRectangle.Right, Consts.ToolWindowHeaderSize - 1);
                }

                using (var p = new Pen(lightColor))
                {
                    g.DrawLine(p, ClientRectangle.Left, 1, ClientRectangle.Right, 1);
                }

                var xOffset = 2;
                if (Icon != null)
                {
                    g.DrawImageUnscaled(Icon, ClientRectangle.Left + 5, ClientRectangle.Top + (Consts.ToolWindowHeaderSize / 2) - (Icon.Height / 2) + 1);
                    xOffset = Icon.Width + 8;
                }

                // Draw text
                using (var b = new SolidBrush(Colors.LightText))
                {
                    var textRect = new Rectangle(xOffset, 0, ClientRectangle.Width - 4 - xOffset, Consts.ToolWindowHeaderSize);

                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(_headerText, Font, b, textRect, format);
                }
            }


        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Absorb event
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DarkTitleBarPanel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Name = "DarkTitleBarPanel";
            this.Size = new System.Drawing.Size(493, 436);
            this.ResumeLayout(false);

        }
    }
}
