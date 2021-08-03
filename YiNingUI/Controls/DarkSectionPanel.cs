using YiNing.UI.Config;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class DarkSectionPanel : Panel
    {
        #region Field Region
        private bool _dragEnable = false;
        private bool isMouseDown = false;
        private Point mouseOffset; //记录鼠标指针的坐标

        private string _sectionHeader;
        public delegate void _HeaderMouseDownHandle(MouseEventArgs e);
        // 标题栏点击的事件
        public event _HeaderMouseDownHandle HeaderMouseDownHandle;
        // 标题栏区域
        private Rectangle _headerRect;
        #endregion

        #region Property Region

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get { return base.Padding; }
        }

        [Category("Appearance")]
        [Description("The section header text associated with this control.")]
        public string SectionHeader
        {
            get { return _sectionHeader; }
            set
            {
                _sectionHeader = value;
                Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("是否支持拖拽")]
        [DefaultValue(false)]
        public bool DragEnable
        {
            get { return _dragEnable; }
            set
            {
                _dragEnable = value;
                if (_dragEnable) BorderStyle = BorderStyle.FixedSingle;
                else BorderStyle = BorderStyle.None;
                Invalidate();
            }
        }
        #endregion

        #region Constructor Region

        public DarkSectionPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            base.Padding = new Padding(1, 25, 1, 1);
        }

        #endregion

        #region Event Handler Region

        protected override void OnEnter(System.EventArgs e)
        {
            base.OnEnter(e);

            Invalidate();
        }

        protected override void OnLeave(System.EventArgs e)
        {
            base.OnLeave(e);

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (Controls.Count > 0)
                Controls[0].Focus();

            if (_headerRect.Contains(new Point(e.X, e.Y)))
            {
                if (DragEnable)
                {
                    mouseOffset.X = e.X;
                    mouseOffset.Y = e.Y;
                    isMouseDown = true;
                    BringToFront();
                }
                if (HeaderMouseDownHandle != null) HeaderMouseDownHandle(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DragEnable && _headerRect.Contains(new Point(e.X, e.Y)))
            {
                Cursor = Cursors.NoMove2D;
            } else
            {
                Cursor = Cursors.Default;
            }

            if (isMouseDown && DragEnable)
            {
                int left = Left + e.X - mouseOffset.X;
                int top = Top + e.Y - mouseOffset.Y;

                left = left > 0 ? left : 0;
                top = top > 0 ? top : 0;

                int maxLeft = Parent.Width - Width;
                int maxTop = Parent.Height - Height;


                Location = new Point(left > maxLeft ? maxLeft : left, top > maxTop ? maxTop : top);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isMouseDown && DragEnable)
            {
                isMouseDown = false;
            }
        }

        #endregion

        #region Paint Region

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = ClientRectangle;

            // Fill body
            using (var b = new SolidBrush(Colors.GreyBackground))
            {
                g.FillRectangle(b, rect);
            }

            // Draw header
            var bgColor = ContainsFocus ? Colors.BlueBackground : Colors.HeaderBackground;
            var darkColor = ContainsFocus ? Colors.DarkBlueBorder : Colors.DarkBorder;
            var lightColor = ContainsFocus ? Colors.LightBlueBorder : Colors.LightBorder;

            using (var b = new SolidBrush(bgColor))
            {
                _headerRect = new Rectangle(0, 0, rect.Width, 25);
                g.FillRectangle(b, _headerRect);
            }

            using (var p = new Pen(darkColor))
            {
                g.DrawLine(p, rect.Left, 0, rect.Right, 0);
                g.DrawLine(p, rect.Left, 25 - 1, rect.Right, 25 - 1);
            }

            using (var p = new Pen(lightColor))
            {
                g.DrawLine(p, rect.Left, 1, rect.Right, 1);
            }

            var xOffset = 3;

            using (var b = new SolidBrush(Colors.LightText))
            {
                var textRect = new Rectangle(xOffset, 0, rect.Width - 4 - xOffset, 25);

                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(SectionHeader, Font, b, textRect, format);
            }

            // Draw border
            using (var p = new Pen(Colors.DarkBorder, 1))
            {
                var modRect = new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

                g.DrawRectangle(p, modRect);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Absorb event
        }

        #endregion
    }
}
