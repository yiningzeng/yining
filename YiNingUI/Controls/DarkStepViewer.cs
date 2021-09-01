using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class DarkStepViewer: UserControl
    {
        /// <summary>
        /// 进度改变触发事件
        /// </summary>
        public event EventHandler OnStepChanged; //定义一个委托类型的事件  
        private Color _Gray = Color.Gray;
        private Color _DarkGray = Color.DarkGray;
        private Color _CompletedColor = Color.Green;
        private Color _WaitingColor = Color.Orange;
        private Color _Red = Color.Red;
        private int _CurrentStep = 0;
        private List<DarkStepViewerItem> _dataSourceList = null;
       
        [Browsable(true), Category("StepViewer")]
        public List<DarkStepViewerItem> ListDataSource
        {
            get
            {
                return _dataSourceList;
            }
            set
            {
                if (_dataSourceList != value)
                {
                    _dataSourceList = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true), Category("StepViewer")]
        [DefaultValue(0)]
        public int CurrentStep
        {
            get
            {
                return _CurrentStep;
            }
            set
            {
                if (ListDataSource != null && value > ListDataSource.Count + 1) value = ListDataSource.Count + 1;
                if (ListDataSource != null && value < 1) value = 1;
                _CurrentStep = value;
                Invalidate();
            }
        }

        public DarkStepViewer()
        {
            InitializeComponent();
            this.Height = 68;
        }
    
        protected override void OnPaint(PaintEventArgs e)
        {
            //if (DesignMode && (ListDataSource == null || ListDataSource.Count ==0))
            //{
            //    ListDataSource = new List<DarkStepViewerItem>();
            //    ListDataSource.Add(new DarkStepViewerItem("1", "放置晶圆", 1, "请放置晶圆", null));
            //    ListDataSource.Add(new DarkStepViewerItem("2", "晶圆定位", 2, "精准的定位到晶圆的位置", null));
            //    ListDataSource.Add(new DarkStepViewerItem("3", "制作程式", 3, "这里是该步骤的描述信息", null));
            //    ListDataSource.Add(new DarkStepViewerItem("4", "程式测试", 4, "这里是该步骤的描述信息", null));
            //    ListDataSource.Add(new DarkStepViewerItem("5", "保存程式", 5, "这里是该步骤的描述信息", null));
            //}
            if (this.ListDataSource != null && ListDataSource.Count > 0)
            {
                int CenterY = 50;
                int index = 1;
                int count = ListDataSource.Count;
            
                int lineWidth = 100;
                int StepNodeWH = 28; // 节点的图标
                int defaultTxtWidth = 80; // 节点的默认计算的标题文字宽度
                lineWidth = (Parent.Width - (StepNodeWH + defaultTxtWidth) * count) / count;
                int initX  = (Parent.Width - (lineWidth + StepNodeWH + defaultTxtWidth) * count) / 2 + 50;
                CenterY = 20;
                //this.Width = 32 * count + lineWidth * (count - 1) + 6+300;
                //defalut pen & brush
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                Brush brush = new SolidBrush(_Gray);
                Pen p = new Pen(brush, 1f);
                Brush brushNode = new SolidBrush(_DarkGray);
                Pen penNode = new Pen(brushNode, 1f);

                Brush brushNodeCompleted = new SolidBrush(_CompletedColor);
                Pen penNodeCompleted = new Pen(brushNodeCompleted, 1f);


                //string
                Font nFont = new Font("微软雅黑", 12);
                Font stepFont = new Font("微软雅黑", 11, FontStyle.Bold);
                int NodeNameWidth = 0;

                foreach (var item in ListDataSource)
                {

                    //round

                    Rectangle rec = new Rectangle(initX, CenterY - StepNodeWH / 2, StepNodeWH, StepNodeWH);
                    if (CurrentStep == item.StepOrder)
                    {
                        e.Graphics.DrawEllipse(new Pen(_WaitingColor, 1f), rec);
                        e.Graphics.FillEllipse(new SolidBrush(_WaitingColor), rec);

                        //白色字体
                        SizeF fTitle = e.Graphics.MeasureString(index.ToString(), stepFont);
                        Point pTitle = new Point(initX + StepNodeWH / 2 - (int)Math.Round(fTitle.Width) / 2, CenterY - (int)Math.Round(fTitle.Height / 2));
                        e.Graphics.DrawString(index.ToString(), stepFont, Brushes.White, pTitle);


                        //nodeName
                        SizeF sNode = e.Graphics.MeasureString(item.StepName, nFont);
                        Point pNode = new Point(initX + StepNodeWH, CenterY - (int)Math.Round(sNode.Height / 2) + 2);

                        e.Graphics.DrawString(item.StepName, new Font(nFont, FontStyle.Bold), brushNode, pNode);
                        NodeNameWidth = (int)Math.Round(sNode.Width);
                        if (index < count)
                        {
                            e.Graphics.DrawLine(p, initX + StepNodeWH + NodeNameWidth, CenterY, initX + StepNodeWH + NodeNameWidth + lineWidth, CenterY);
                        }

                    }
                    else if (item.StepOrder < CurrentStep)
                    {
                        //completed
                        e.Graphics.DrawEllipse(penNodeCompleted, rec);
                        //image
                        RectangleF recRF = new RectangleF(rec.X + 6, rec.Y + 6, rec.Width - 12, rec.Height - 12);
                        e.Graphics.FillEllipse(brushNodeCompleted, rec);
                        e.Graphics.DrawImage(DockIcons.hook, recRF);
                 
                        //nodeName
                        SizeF sNode = e.Graphics.MeasureString(item.StepName, nFont);
                        Point pNode = new Point(initX + StepNodeWH, CenterY - (int)Math.Round(sNode.Height / 2) + 2);
                        e.Graphics.DrawString(item.StepName, nFont, brushNode, pNode);
                        NodeNameWidth = (int)Math.Round(sNode.Width);

                        if (index < count)
                        {
                            e.Graphics.DrawLine(penNodeCompleted, initX + StepNodeWH + NodeNameWidth, CenterY, initX + StepNodeWH + NodeNameWidth + lineWidth, CenterY);
                        }

                    }
                    else
                    {
                        e.Graphics.DrawEllipse(p, rec);
                        //
                        SizeF fTitle = e.Graphics.MeasureString(index.ToString(), stepFont);
                        Point pTitle = new Point(initX + StepNodeWH / 2 - (int)Math.Round(fTitle.Width) / 2, CenterY - (int)Math.Round(fTitle.Height / 2));
                        e.Graphics.DrawString(index.ToString(), stepFont, brush, pTitle);
                        //nodeName
                        SizeF sNode = e.Graphics.MeasureString(item.StepName, nFont);
                        Point pNode = new Point(initX + StepNodeWH, CenterY - (int)Math.Round(sNode.Height / 2) + 2);
                        e.Graphics.DrawString(item.StepName, nFont, brushNode, pNode);
                        NodeNameWidth = (int)Math.Round(sNode.Width);
                        if (index < count)
                        {
                            //line
                            e.Graphics.DrawLine(p, initX + StepNodeWH + NodeNameWidth, CenterY, initX + StepNodeWH + NodeNameWidth + lineWidth, CenterY);
                        }
                    }

                    //描述信息
                    if (item.StepDesc != "")
                    {
                        Point pNode = new Point(initX + StepNodeWH, CenterY + 10);
                        StringFormat sf = new StringFormat();

                        sf.Alignment = StringAlignment.Near;

                        sf.LineAlignment = StringAlignment.Near;

                        e.Graphics.DrawString(item.StepDesc, new Font(nFont.FontFamily, 10), brush, new RectangleF(pNode, new Size(lineWidth + NodeNameWidth, 100)), sf);
                    }



                    index++;
                    //8 is space width
                    initX = initX + lineWidth + StepNodeWH + NodeNameWidth + 8;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DarkStepViewer
            // 
            this.Name = "DarkStepViewer";
            this.Size = new System.Drawing.Size(637, 258);
            this.ResumeLayout(false);

        }

        public void Complete()
        {
            CurrentStep++;
            if (OnStepChanged != null) OnStepChanged(this, new EventArgs());
        }
        public void PreviousStep()
        {
            CurrentStep--;
            if (OnStepChanged != null) OnStepChanged(this, new EventArgs());
        }
    }
}
