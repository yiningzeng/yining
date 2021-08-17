using YiNing.UI.Config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class DarkPointsIn : CheckBox
    {
        #region Field Region
        private bool _Enable = false;
        private Color _CheckedColor = Color.Lime;
        private Color _UnCheckedColor = Color.Red;
        #endregion

        #region Property Region

        [Description("选中的颜色")]
        [DefaultValue(false)]
        public Color CheckedColor
        {
            get { return _CheckedColor; }
            set
            {
                _CheckedColor = value;
                Invalidate();
            }
        }

        [Description("未选中的颜色")]
        [DefaultValue(false)]
        public Color UnCheckedColor
        {
            get { return _UnCheckedColor; }
            set
            {
                _UnCheckedColor = value;
                Invalidate();
            }
        }


        [Description("是否可操作")]
        [DefaultValue(false)]
        public new bool Enabled
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Appearance Appearance
        {
            get { return base.Appearance; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool AutoEllipsis
        {
            get { return base.AutoEllipsis; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage
        {
            get { return base.BackgroundImage; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool FlatAppearance
        {
            get { return false; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FlatStyle FlatStyle
        {
            get { return base.FlatStyle; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image
        {
            get { return base.Image; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment ImageAlign
        {
            get { return base.ImageAlign; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int ImageIndex
        {
            get { return base.ImageIndex; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string ImageKey
        {
            get { return base.ImageKey; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageList ImageList
        {
            get { return base.ImageList; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment TextAlign
        {
            get { return base.TextAlign; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TextImageRelation TextImageRelation
        {
            get { return base.TextImageRelation; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool ThreeState
        {
            get { return base.ThreeState; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseCompatibleTextRendering
        {
            get { return false; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseVisualStyleBackColor
        {
            get { return false; }
        }

        #endregion

        #region Constructor Region

        public DarkPointsIn()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);
        }

        #endregion

        protected override void OnMouseDown(MouseEventArgs e) { }

        protected override void OnMouseUp(MouseEventArgs e) { }


        #region Paint Region

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            var size = Consts.CheckBoxSize;

            var textColor = Colors.LightText;
            var borderColor = Colors.LightText;
            var fillColor = Colors.LightestBackground;

            textColor = Colors.DisabledText;
            borderColor = Colors.GreyHighlight;


            using (var b = new SolidBrush(Colors.GreyBackground))
            {
                g.FillRectangle(b, rect);
            }

            using (var p = new Pen(borderColor))
            {
                var boxRect = new Rectangle(0, (rect.Height / 2) - (size / 2) - 1, size + 3, size + 3);
                g.DrawRectangle(p, boxRect);
            }

            if (Checked)
            {
                using (var b = new SolidBrush(CheckedColor))
                {
                    Rectangle boxRect = new Rectangle(2, (rect.Height / 2) - ((size - 1) / 2), size, size);
                    g.FillRectangle(b, boxRect);
                }
            }
            else
            {
                using (var b = new SolidBrush(UnCheckedColor))
                {
                    Rectangle boxRect = new Rectangle(2, (rect.Height / 2) - ((size - 1) / 2), size, size);
                    g.FillRectangle(b, boxRect);
                }

            }

            using (var b = new SolidBrush(textColor))
            {
                var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near
                };

                var modRect = new Rectangle(size + 6,  1, rect.Width - size, rect.Height);
                g.DrawString(Text, Font, b, modRect, stringFormat);
            }
        }

        #endregion
    }
}
