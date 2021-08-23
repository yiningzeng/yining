using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YiNing.UI.Config;

namespace YiNing.UI.Controls
{
    public class DarkWaferList : DarkListView
    {

        private Color _NotNullTextColor = Color.Silver; // 不是空的Item 文字颜色
        private Color _SelectBackgroundColor = Color.Chocolate; // 不是空的Item 选中颜色
        private List<int> _selectItems;
        private string _WaferPath = @"D:\WaferDataIn\mapping";
        private string _WaferSuffix = "txt";
        private char _WaferIdSeparator = '\0';
        private readonly int _iconSize = 16;

        private bool _ReadOnly = false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new List<int> SelectedIndices
        {
            get { return _selectItems; }
        }


        [Description("晶圆图谱保存的路径")]
        [DefaultValue(@"D:\WaferDataIn\mapping")]
        public string WaferPath
        {
            get { return _WaferPath; }
            set
            {
                if (!Directory.Exists(value)) throw new Exception("晶圆图谱目录不存在");
                _WaferPath = value;
                RefreshWafer();
                Invalidate();
            }
        }

        [Description("晶圆图谱文件的后缀")]
        [DefaultValue("txt")]
        public string WaferSuffix
        {
            get { return _WaferSuffix; }
            set
            {
                _WaferSuffix = value;
                RefreshWafer();
                Invalidate();
            }
        }

        [Description("晶圆图谱文件名编号的分隔符，如果为空那么就直接截取最后两位")]
        [DefaultValue('\0')]
        public char WaferIdSeparator
        {
            get { return _WaferIdSeparator; }
            set
            {
                _WaferIdSeparator = value;
                RefreshWafer();
                Invalidate();
            }
        }

        public DarkWaferList()
        {
            this.ItemHeight = 30;
            this.ShowIcons = true;
            this.MultiSelect = true;
            _selectItems = new List<int>();
            RefreshWafer();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_ReadOnly) return;
            if (Items.Count == 0)
                return;

            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
                return;

            var pos = OffsetMousePosition;

            var range = ItemIndexesInView().ToList();

            var top = range.Min();
            var bottom = range.Max();
            var width = Math.Max(ContentSize.Width, Viewport.Width);

            for (var i = top; i <= bottom; i++)
            {
                var rect = new Rectangle(0, i * ItemHeight, width, ItemHeight);

                if (rect.Contains(pos) && Items[i].Enabled)
                {
                    if (Items[i].Tag != null)
                    {
                        //Items[i].Text = "select";
                        if (Items[i].Icon == null)
                        {
                            Items[i].Icon = DarkWaferListIcons.select;
                            Items[i].TextColor = Color.White;
                            Items[i].BackgroundColor = _SelectBackgroundColor;
                            _selectItems.Add(i);
                        }
                        else
                        {
                            Items[i].Icon = null;
                            Items[i].TextColor = _NotNullTextColor;
                            Items[i].BackgroundColor = Colors.GreyBackground;
                            _selectItems.Remove(i);
                        }
                    }
                }
            }
        }

        public new void SelectItem(int index)
        {
            if (index < 0 || index > Items.Count - 1)
                throw new IndexOutOfRangeException($"Value '{index}' is outside of valid range.");

            if (Items[index].Tag != null)
            {
                //Items[i].Text = "select";
                if (Items[index].Icon == null)
                {
                    Items[index].Icon = DockIcons.选择;
                    _selectItems.Add(index);
                }
            }
            Invalidate();
        }


        public void StartWorking()
        {
            if (_selectItems.Count == 0) return;
            _ReadOnly = true;
            var list = (from f in _selectItems
                        orderby f ascending
                        select f).ToList<int>();
            Items[list[0]].Icon = DarkWaferListIcons.start;
            Items[list[0]].BackgroundColor = Color.LimeGreen;
            for (int i = 1; i < list.Count; i++)
            {
                Items[list[i]].Icon = DarkWaferListIcons.wait;
            }
            Invalidate();
        }
        public void StopWorking()
        {
            if (_selectItems.Count == 0) return;
            _ReadOnly = false;
            for (int i = 0; i < _selectItems.Count; i++)
            {
                Items[_selectItems[i]].BackgroundColor = _SelectBackgroundColor;
                Items[_selectItems[i]].Icon = DarkWaferListIcons.select;
                Items[_selectItems[i]].TextColor = Color.White;
            }
            Invalidate();
        }
        /// <summary>
        /// 刷新图谱文件数据
        /// </summary>
        public void RefreshWafer()
        {
            _ReadOnly = false;
            _selectItems.Clear();
            string[] files = GetLatestFiles();
            Items.Clear();
            for (int i = 25; i > 0; i--)
            {
                DarkListItem item = new DarkListItem();
                item.TextColor = Color.DimGray;
                string id = i.ToString().Length == 1 ? "0" + i.ToString() : i.ToString();
                item.Text = "NO." + id + "　　" + "-";
                var list = Analysis(files);
                var one = list.Find(v => v.Id.Equals(id));
                item.Enabled = false;
                item.BackgroundColor = Colors.GreyBackground;

                if (one != null) {
                    item.Text = "NO." + id + "　　" + one.Prefix;
                    item.TextColor = _NotNullTextColor;
                    item.Enabled = true;
                    item.Tag = one;
                    //item.BackgroundColor = Color.Chocolate;
                }
                Items.Add(item);
            }
            SelectItem(0);
        }


        private List<DarkWaferListItemInfo> Analysis(string[] files)
        {
            List<DarkWaferListItemInfo> lists = new List<DarkWaferListItemInfo>();
            Debug.WriteLine(WaferPath + " " + WaferSuffix + " 文件:" + files);
            foreach (var f in files)
            {
                DarkWaferListItemInfo one = new DarkWaferListItemInfo();
                one.FileName = f;
                one.FullFileName = Path.Combine(WaferPath, f);
                one.Suffix = WaferSuffix;
                one.Prefix = f.Replace("." + WaferSuffix, "");
                if (WaferIdSeparator == '\0')
                {
                    one.Id = one.Prefix.Substring(one.Prefix.Length - 2);
                }
                else
                {
                    try
                    {
                        one.Id = one.Prefix.Split(WaferIdSeparator)[1];
                    }
                    catch (Exception er)
                    {
                        one.Id = "Err";
                    }
                }
                lists.Add(one);
            }
            return lists;
        }

        private string[] GetLatestFiles()
        {
            var query = from f in Directory.GetFiles(WaferPath, "*." + WaferSuffix)
                        let fi = new FileInfo(f)
                        orderby fi.CreationTime descending
                        select fi.Name;
            return query.ToArray();
        }


        protected override void PaintContent(Graphics g)
        {
            var range = ItemIndexesInView().ToList();

            if (range.Count == 0)
                return;

            var top = range.Min();
            var bottom = range.Max();

            for (var i = top; i <= bottom; i++)
            {
                var width = Math.Max(ContentSize.Width, Viewport.Width);
                var rect = new Rectangle(0, i * ItemHeight, width, ItemHeight);

                // Background
                var odd = i % 2 != 0;
                var bgColor = !odd ? Colors.HeaderBackground : Colors.GreyBackground;

                if (SelectedIndices.Count > 0 && SelectedIndices.Contains(i))
                    bgColor = Items[i].BackgroundColor;// Color.Chocolate;

                using (var b = new SolidBrush(bgColor))
                {
                    g.FillRectangle(b, rect);
                }

                // DEBUG: Border
                /*using (var p = new Pen(Colors.DarkBorder))
                {
                    g.DrawLine(p, new Point(rect.Left, rect.Bottom - 1), new Point(rect.Right, rect.Bottom - 1));
                }*/

                // Icon
                if (ShowIcons && Items[i].Icon != null)
                {
                    g.DrawImageUnscaled(Items[i].Icon, new Point(rect.Left + 5, rect.Top + (rect.Height / 2) - (_iconSize / 2)));
                }

                // Text
                using (var b = new SolidBrush(Items[i].TextColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                    var modFont = new Font(Font, Items[i].FontStyle);

                    var modRect = new Rectangle(rect.Left + 5, rect.Top, rect.Width, rect.Height);

                    if (ShowIcons)
                        modRect.X += _iconSize + 8;

                    g.DrawString(Items[i].Text, modFont, b, modRect, stringFormat);
                }
            }
        }


    }
}
