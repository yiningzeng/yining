using YiNing.UI.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class DarkProgressReminder : DarkScrollView
    {
        #region Event Region

        public event EventHandler SelectedIndicesChanged;

        #endregion

        #region Field Region

        /// <summary>
        /// 进度开始的地方
        /// </summary>
        private int _startNum = 0;

        private int _itemHeight = 20;

        private readonly int _iconSize = 16;

        private ObservableCollection<DarkListItem> _items;
        private List<int> _selectedIndices;
        private Color _selectFocusItemColor = Colors.BlueSelection;
        #endregion

        #region Property Region

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<DarkListItem> Items
        {
            get { return _items; }
            set
            {
                if (_items != null)
                    _items.CollectionChanged -= Items_CollectionChanged;

                _items = value;

                _items.CollectionChanged += Items_CollectionChanged;

                UpdateListBox();
                SetStartNum(0);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedIndices
        {
            get { return _selectedIndices; }
        }

        [Category("Appearance")]
        [Description("Determines the height of the individual list view items.")]
        [DefaultValue(20)]
        public int ItemHeight
        {
            get { return _itemHeight; }
            set
            {
                _itemHeight = value;
                UpdateListBox();
            }
        }
        #endregion

        #region Constructor Region

        public DarkProgressReminder()
        {
            Items = new ObservableCollection<DarkListItem>();
            _selectedIndices = new List<int>();
            //SetStartNum(0);
        }

        #endregion

        #region Event Handler Region

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                using (var g = CreateGraphics())
                {
                    // Set the area size of all new items
                    foreach (DarkListItem item in e.NewItems)
                    {
                        item.TextChanged += Item_TextChanged;
                        UpdateItemSize(item, g);
                    }
                }

                // Find the starting index of the new item list and update anything past that
                if (e.NewStartingIndex < (Items.Count - 1))
                {
                    for (var i = e.NewStartingIndex; i <= Items.Count - 1; i++)
                    {
                        UpdateItemPosition(Items[i], i);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (DarkListItem item in e.OldItems)
                    item.TextChanged -= Item_TextChanged;

                // Find the starting index of the old item list and update anything past that
                if (e.OldStartingIndex < (Items.Count - 1))
                {
                    for (var i = e.OldStartingIndex; i <= Items.Count - 1; i++)
                    {
                        UpdateItemPosition(Items[i], i);
                    }
                }
            }

            if (Items.Count == 0)
            {
                if (_selectedIndices.Count > 0)
                {
                    _selectedIndices.Clear();

                    if (SelectedIndicesChanged != null)
                        SelectedIndicesChanged(this, null);
                }
            }

            UpdateContentSize();
        }

        private void Item_TextChanged(object sender, EventArgs e)
        {
            var item = (DarkListItem)sender;

            UpdateItemSize(item);
            UpdateContentSize(item);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) { }
        #endregion

        #region Method Region

        /// <summary>
        /// 获取当前的id
        /// </summary>
        /// <returns></returns>
        public int NowId()
        {
            return _startNum;
        }
        /// <summary>
        /// 获取当前的Item
        /// </summary>
        /// <returns></returns>
        public DarkListItem NowItem()
        {
            if (_startNum >= Items.Count) return new DarkListItem("已全部完成");
            return Items[_startNum];
        }
        /// <summary>
        /// 上一个进度
        /// </summary>
        public void Previous()
        {
            _startNum--;
            if (_startNum > Items.Count + 1) _startNum = Items.Count + 1;
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.完成打钩;
                    Items[i].TextColor = Color.Chocolate;
                }
                else if (i == _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.进度指向;
                    Items[i].TextColor = Color.Chocolate;
                }
                else
                {
                    Items[i].Icon = null;
                    Items[i].TextColor = Color.DimGray;
                }
            }
            Invalidate();
        }
        /// <summary>
        /// 执行下一个进度
        /// </summary>
        public void Next()
        {
            _startNum++;
            if (_startNum > Items.Count + 1) _startNum = Items.Count + 1;
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.完成打钩;
                    Items[i].TextColor = Color.Chocolate;
                }
                else  if (i == _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.进度指向;
                    Items[i].TextColor = Color.Chocolate;
                }
                else
                {
                    Items[i].Icon = null;
                    Items[i].TextColor = Color.DimGray;
                }
            }
            Invalidate();
        }
        /// <summary>
        /// 重置当前进度Index
        /// </summary>
        /// <param name="index"></param>
        public void SetStartNum(int index)
        {
            _startNum = index;
            if (_startNum > Items.Count + 1) _startNum = Items.Count + 1;
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.完成打钩;
                    Items[i].TextColor = Color.Chocolate;
                }
                else if (i == _startNum)
                {
                    Items[i].Icon = DarkProgressReminderIcons.进度指向;
                    Items[i].TextColor = Color.Chocolate;
                }
                else
                {
                    Items[i].Icon = null;
                    Items[i].TextColor = Color.DimGray;
                }
            }
            Invalidate();
        }

        /// <summary>
        /// 设置当前的进度错误
        /// </summary>
        public void Error()
        {
            Items[_startNum].Icon = DarkProgressReminderIcons.错误;
            Items[_startNum].TextColor = Color.Red;
            Invalidate();
        }

        /// <summary>
        /// 当前的进度设置暂停
        /// </summary>
        public void Stop()
        {
            Items[_startNum].Icon = DarkProgressReminderIcons.停止;
            Items[_startNum].TextColor = Color.Yellow;
            Invalidate();
        }

        /// <summary>
        /// 表示是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            return _startNum >= Items.Count;
        }  
        private void UpdateListBox()
        {
            using (var g = CreateGraphics())
            {
                for (var i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i];
                    UpdateItemSize(item, g);
                    UpdateItemPosition(item, i);
                }
            }

            UpdateContentSize();
        }

        private void UpdateItemSize(DarkListItem item)
        {
            using (var g = CreateGraphics())
            {
                UpdateItemSize(item, g);
            }
        }

        private void UpdateItemSize(DarkListItem item, Graphics g)
        {
            var size = g.MeasureString(item.Text, Font);
            size.Width++;

            size.Width += _iconSize + 8;

            item.Area = new Rectangle(item.Area.Left, item.Area.Top, (int)size.Width, item.Area.Height);
        }

        private void UpdateItemPosition(DarkListItem item, int index)
        {
            item.Area = new Rectangle(2, (index * ItemHeight), item.Area.Width, ItemHeight);
        }

        private void UpdateContentSize()
        {
            var highestWidth = 0;

            foreach (var item in Items)
            {
                if (item.Area.Right + 1 > highestWidth)
                    highestWidth = item.Area.Right + 1;
            }

            var width = highestWidth;
            var height = Items.Count * ItemHeight;

            if (ContentSize.Width != width || ContentSize.Height != height)
            {
                ContentSize = new Size(width, height);
                Invalidate();
            }
        }

        private void UpdateContentSize(DarkListItem item)
        {
            var itemWidth = item.Area.Right + 1;

            if (itemWidth == ContentSize.Width)
            {
                UpdateContentSize();
                return;
            }

            if (itemWidth > ContentSize.Width)
            {
                ContentSize = new Size(itemWidth, ContentSize.Height);
                Invalidate();
            }
        }

        protected IEnumerable<int> ItemIndexesInView()
        {
            var top = (Viewport.Top / ItemHeight) - 1;

            if (top < 0)
                top = 0;

            var bottom = ((Viewport.Top + Viewport.Height) / ItemHeight) + 1;

            if (bottom > Items.Count)
                bottom = Items.Count;

            var result = Enumerable.Range(top, bottom - top);
            return result;
        }

        private IEnumerable<DarkListItem> ItemsInView()
        {
            var indexes = ItemIndexesInView();
            var result = indexes.Select(index => Items[index]).ToList();
            return result;
        }

        #endregion

        #region Paint Region

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
                var bgColor = Colors.GreyBackground;

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
                if (Items[i].Icon != null)
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

                    var modRect = new Rectangle(rect.Left + 2, rect.Top, rect.Width, rect.Height);

                    modRect.X += _iconSize + 8;

                    g.DrawString(Items[i].Text, modFont, b, modRect, stringFormat);
                }
            }
        }

        #endregion
    }
}
