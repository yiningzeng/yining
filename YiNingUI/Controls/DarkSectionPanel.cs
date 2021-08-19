using YiNing.UI.Config;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class DarkSectionPanel : Panel
    {
        #region Field Region
        private Point startPoint;
        private bool _dragEnable = false;
        private string _sectionHeader;
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

        //public new DockStyle Dock
        //{
        //    get { return Dock; }
        //    set
        //    {
        //        Dock = value;
            
        //        Invalidate();
        //    }
        //}


        [Description("是否可以拖动")]
        [DefaultValue(false)]
        public bool DragEnable
        {
            get { return _dragEnable; }
            set
            {
                if (Dock == DockStyle.None)
                {
                    _dragEnable = value;
                    Invalidate();
                }
                else
                {
                    throw new System.Exception("Dock属性只有是None的时候才可以拖动");
                }
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
            startPoint.X = e.X;
            startPoint.Y = e.Y;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragEnable) return;
            var bgRect = new Rectangle(0, 0, ClientRectangle.Width, 25);
            if (bgRect.Contains(new Point(e.X, e.Y)))
            {
                Cursor.Current = Cursors.NoMove2D;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }

            if (e.Button == MouseButtons.Left)
            {
                Point mousePositon = Control.MousePosition;
                mousePositon.Offset(-startPoint.X, -startPoint.Y);
                Point point = Parent.PointToClient(mousePositon);
                if (point.X < 0) point.X = 0;
                if (point.Y < 0) point.Y = 0;
                if (point.X + Width > Parent.Width) point.X = Parent.Width - Width;
                if (point.Y + Height > Parent.Height) point.Y = Parent.Height - Height;
                Location = point;
            }
            Invalidate();
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
                var bgRect = new Rectangle(0, 0, rect.Width, 25);
                g.FillRectangle(b, bgRect);
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
