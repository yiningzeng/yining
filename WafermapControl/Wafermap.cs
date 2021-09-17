using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using YiNing.WafermapDisplay.WafermapControl;
using Newtonsoft.Json;
using System.IO;

namespace YiNing.WafermapDisplay
{
 
	
    public partial class Wafermap : UserControl
    {
        /// <summary>
        /// Die鼠标点击事件
        /// </summary>
        public event EventHandler<Die> OnDieClick; //定义一个委托类型的事件  
        private String tooSmallString = "TOO SMALL";
        private int _DieAlpha = 150;
        public String TooSmallString
        {
            get { return tooSmallString; }
            set { tooSmallString = value; }
        }

        private String noDataString = "NO DATA";

        [Description("芯片的透明度")]
        [DefaultValue(150)]
        public int DieAlpha
        {
            get { return _DieAlpha; }
            set
            {
                _DieAlpha = value;
                Invalidate();
            }
        }

        public String NoDataString
        {
            get { return noDataString; }
            set { noDataString = value; }
        }

        private int _SelectX = 0, _SelectY = 0;
        private Die _SelectOneDie;
        private List<Die> _SelectDies;
        private Die[] _SelectRegionDiagonalDie;
        private Die _selectStartDie, _selectEndDie; // 这两个是多选die的开始和结束die
        private bool SelectMode = false; //是否是选中模式,鼠标up就为false
        private bool _SelectMultiple = true; //是否是选中模式,鼠标up就为false

        private int _ExcludeEdgeNum = 0; //边沿排除个数
        /// <summary>
        /// 是否多选
        /// </summary>
        [Description("是否多选")]
        [DefaultValue(true)]
        public bool SelectMultiple
        {
            get { return _SelectMultiple; }
            set
            {
                _SelectMultiple = value;
            }
        }


        [Description("边沿排除的个数")]
        [DefaultValue(0)]
        public int ExcludeEdgeNum
        {
            get { return _ExcludeEdgeNum; }
            set
            {
                _ExcludeEdgeNum = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 设置单个选中Die的X
        /// </summary>
        [Description("设置单个选中Die的X")]
        public int SelectX
        {
            get { return _SelectX; }
            set
            {
                if (dataset != null && dataset.Length > 0 && value >=0 && value < dataset.GetLength(0)) _SelectOneDie = dataset[value, _SelectY];
                else return;
                _SelectX = value;
                _SelectOneDie = dataset[_SelectX, _SelectY];
                MouseDownDie(_SelectOneDie);
            }
        }
        /// <summary>
        /// 设置单个选中Die的Y
        /// </summary>
        [Description("设置单个选中Die的Y")]
        public int SelectY
        {
            get { return _SelectY; }
            set
            {
                if (dataset != null && dataset.Length > 0 && value >= 0 && value < dataset.GetLength(1)) _SelectOneDie = dataset[_SelectX, value]; else return;
                _SelectY = value;
                _SelectOneDie = dataset[_SelectX, _SelectY];
                MouseDownDie(_SelectOneDie);
            }
        }

        [Description("数据")]
        public Die[,] Dataset
        {
            get { return dataset; }
            set
            {
                dataset = value;
                visibleDataset.Clear();
                SelectOneDie = null;
                Invalidate();

            }
        }

        /// <summary>
        /// 选中单个die
        /// </summary>
        [Description("选中单个die")]
        public Die SelectOneDie
        {
            get { return _SelectOneDie; }
            set
            {
                _SelectOneDie = value;
                MouseDownDie(_SelectOneDie);
            }
        }

        /// <summary>
        /// 多选区域的对角线2点，会自动计算该点属于区域的那个顶点,Get获取的可能就不是最终传入的两个点，最终保存的是左上角点和右下角点
        /// </summary>
        [Description("多选区域的对角线2点，会自动计算该点属于区域的那个顶点,Get获取的可能就不是最终传入的两个点，最终保存的是左上角点和右下角点")]
        public Die[] SelectRegionDiagonalDie
        {
            get { return _SelectRegionDiagonalDie; }
            set
            {
                if (value != null && value.Length >= 2)
                {
                    _selectStartDie = value[0];
                    _selectEndDie = value[1];
                    DrawSelectDies();
                }
            }
        }

        /// <summary>
        /// 获取多选中的die的列表，暂时设置为只读，如果要动态改变的话，只用设置左上角的点和右下角的点即可
        /// </summary>
        //[Description("获取多选中的die的列表，暂时设置为只读，如果要动态改变的话，只用设置左上角的点和右下角的点即可")]
        private List<Die> SelectDies
        {
            get { return _SelectDies; }
        }

        private int translation_x=0;

        public int TranslationX
        {
            get { return translation_x; }
            set { translation_x = value; }
        }

        private int translation_y=0;

        public int TranslationY
        {
            get { return translation_y; }
            set { translation_y = value; }
        }

        private int rotation;

        /// <summary>
        /// 旋转角度可能会有问题，未测试
        /// </summary>
        [Description("旋转角度可能会有问题，未测试")]
        public int Rotation
        {
            get { return rotation; }
            set { if (value % 90 == 0 && value >= 0 && value < 360)
                    rotation = value;
                else
                    throw new ArgumentException("Rotation has to be 0, 90, 180 or 270 degrees (Is "+value+")");
            }
        }
        
        
        private float zoom;

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }
        private int notchLocation = 0;


        /// <summary>
        /// 缺角位置
        /// </summary>
        [Description("缺角位置")]
        public int Notchlocation
        {
            get { return notchLocation; }
            set {
                if (value % 90 == 0 && value >= 0 && value <= 270)
                    notchLocation = value;
                else
                    throw new ArgumentException("NotchLocation has to be 0, 90, 180 or 270 degrees (Is "+value+")");
                }
        }

        private Die[,] dataset;
        private List<Die> visibleDataset = new List<Die>();

        private string savePath = "";
        [Description("可见数据")]
        public List<Die> VisibleDataset
        {
            get { return visibleDataset; }
        }

        [Description("可见数据保存路径")]
        public string VisibleDatasetSavePath
        {
            get { return savePath; }
            set
            {
                savePath = value;
            }
        }

        private Color[] colors;
        /// <summary>
        /// 所有颜色
        /// </summary>
        [Description("所有颜色")]
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        private bool interactive = true;
        /// <summary>
        /// 是否可交互
        /// </summary>
        [Description("是否可交互")]
        [DefaultValue(true)]
        public bool Interactive
        {
            get { return interactive; }
            set
            {
                interactive = value;
                registerEvents();
            }
        }


        public Wafermap()
        {
            zoom=1f;
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            setupDefaultColors();
            registerEvents();
        }

        public void ReFresh()
        {
            Invalidate();
        }
       

        private void setupDefaultColors()
        {
            // Just some sample colors to get started
            colors = new Color[255];
            colors[0] = Color.FromArgb(_DieAlpha, Color.Green);
            colors[1] = Color.FromArgb(_DieAlpha, Color.Red);
            colors[2] = Color.FromArgb(_DieAlpha, Color.Yellow);
            colors[3] = Color.FromArgb(_DieAlpha, Color.Blue);
            colors[4] = Color.FromArgb(_DieAlpha, Color.Orange);
            colors[5] = Color.FromArgb(_DieAlpha, Color.Magenta);
            colors[6] = Color.FromArgb(_DieAlpha, Color.DarkBlue);
            colors[7] = Color.FromArgb(_DieAlpha, Color.Pink);
            colors[50] = Color.FromArgb(_DieAlpha, Color.Black);
        }

        private void Wafermap_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        private bool isScaled;

        public bool IsScaled
        {
            get { return isScaled; }
            
        }

        private int scaleFactor;

        public int ScaleFactor
        {
            get { return scaleFactor; }
        }
        
        
        // We need some globals to be available for calculations
        RectangleF boundingBox_;
        SizeF dieSize_;
        

        protected override void OnPaint(PaintEventArgs e)
        {
            // set rotation
            e.Graphics.RotateTransform((float)rotation);
            if(rotation!=0)
            {
                // When we rotate, we also have to translate
                switch (rotation)
                {
                    case 90:
                        e.Graphics.TranslateTransform(0, -boundingBox_.Width);
                        break;
                    case 180:
                        e.Graphics.TranslateTransform(-boundingBox_.Width,-boundingBox_.Height);
                        break;
                    case 270:
                        e.Graphics.TranslateTransform(-boundingBox_.Height, 0);
                        break;
                }
            }
            // set additional translation
            e.Graphics.TranslateTransform(translation_x, translation_y); 

            // 使用抗锯齿
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            
            // Here comes everything that has to be calculated on each resize/redraw
            // Just do this calculations once
            // Let's find the best Size for the outline
            float w = this.Width*zoom;
            float h = this.Height*zoom;
           
            float size = w < h ? w : h;
            // Wafersize is size-2 because we're not drawing the first and the last pixels
            SizeF wafersize = new SizeF(size - 2, size - 2);
            PointF starting = new PointF((w - size) / 2f, (h - size) / 2f);
            RectangleF boundingBox = new RectangleF(starting, wafersize);
            boundingBox_ = boundingBox;
            // Create graphics path.
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(boundingBox);
            //// Set clipping region to path.
            e.Graphics.SetClip(clipPath, CombineMode.Replace);
            //clipPath = new GraphicsPath();
            //clipPath.AddRectangle(new RectangleF(new PointF(boundingBox.Left, boundingBox.Bottom -10), new SizeF(boundingBox.Width, boundingBox.Height)));
            //e.Graphics.SetClip(clipPath, CombineMode.Exclude);
            drawNotch(e.Graphics, boundingBox, notchLocation);

            // Let's calculate everything needed for drawing the dies
            if (dataset != null && dataset.Length > 0)
            {
                int maxX = dataset.GetLength(0);
                int maxY = dataset.GetLength(1);
                float sizeX = boundingBox.Width / (float)maxX;
                float sizeY = boundingBox.Height / (float)maxY;
                
                int every = 1;
               
                // If dieSizeX or dieSizeY is less then 2 pixels
                // take only every nth die
                while (sizeX <= 2 || sizeY <= 2)
                {
                    every = every * 2;
                    sizeX = boundingBox.Width / (float)(maxX/every);
                    sizeY = boundingBox.Height / (float)(maxY/every);
                }
                SizeF dieSize = new SizeF(sizeX, sizeY);
                dieSize_ = dieSize;
                // If every != 1 we recalculate the input data
                // Otherwise we pass the original dataset
                // Caveat: We must not overwrite the original dataset ;)
                if (every > 1)
                {
                    // Create a new dataset
                    // Get the highest bin code in x/y to x/y + every as result for x/y
                    // First set the property
                    isScaled = true;
                    scaleFactor = every;
                    drawDies(e.Graphics, boundingBox, WafermapTools.scaleArray(dataset,every), _SelectDies, dieSize);
                    // Print "Too small" message
                    //FontFamily myFontFamily = new FontFamily("Arial");
                    //Font myFont = new Font(myFontFamily,
                    //   10,
                    //   FontStyle.Bold,
                    //   GraphicsUnit.Pixel);

                    //e.Graphics.DrawString(tooSmallString, myFont, new SolidBrush(Color.Red), boundingBox.Location);
                    
                }
                else
                {
                    // Properties
                    isScaled = false;
                    scaleFactor = 1;

                    // Simply draw the die
                    drawDies(e.Graphics, boundingBox, dataset, _SelectDies, dieSize);
                }
            }
            else
            {
                drawCircle(e.Graphics, boundingBox);
            }
            //else
            //{
            //    // Display "No Data" message
            //    FontFamily myFontFamily = new FontFamily("宋体");
            //    Font myFont = new Font(myFontFamily,
            //       10,
            //       FontStyle.Bold);
                
            //    e.Graphics.DrawString(noDataString, myFont,new SolidBrush( Color.Red), boundingBox.Location);
            //}
        }

        // Try to reuse - only instantiated once
        SolidBrush waferFillbrush = new SolidBrush(Color.Silver);
        Pen blackPen = new Pen(Color.Black);
        //底部的缺口
        SolidBrush notchFillBrush = new SolidBrush(Color.FromArgb(255, Color.White));
        /// <summary>
        /// 绘画园
        /// </summary>
        /// <param name="g"></param>
        /// <param name="boundingBox"></param>
        private void drawCircle(Graphics g, RectangleF boundingBox)
        {
            //g.FillPie(Brushes.Pink, new Rectangle(-10, 20, this.Width, this.Height), 180, 180);//绘制半圆
            g.FillEllipse(waferFillbrush, boundingBox);
            //g.DrawEllipse(blackPen, boundingBox);
        }

        /// <summary>
        /// 绘画底部的缺口
        /// </summary>
        /// <param name="g"></param>
        /// <param name="boundingBox"></param>
        /// <param name="location"></param>
        private void drawNotch(Graphics g, RectangleF boundingBox, int location)
        {
            /*
             *  //在晶圆上画凹口（物理性质）以对准。可以是0、90、180、270度
                //从0开始° 底部（逆时针）
                //形状固定在切割圆上
             */
            float size =boundingBox.Width<boundingBox.Height?boundingBox.Width:boundingBox.Height;
            size = size * 0.05f;
            // 计算切口的位置
            // 180°
            float x =boundingBox.X+(boundingBox.Width/2f)-(size/2f);
            float y=boundingBox.Y-(size/2f);
            int start = 0;
            int end = 180;
            switch (location){
                case 0:
                    y = boundingBox.Y +boundingBox.Height-(size / 2f);
                    end = -180;
                    break;
                case 90:
                    x = boundingBox.X - (size / 2f);
                    y = boundingBox.Y +(boundingBox.Height/2f)- (size / 2f);
                    start = 90;
                    end = -180;
                    break;
                case 270:
                    x = boundingBox.X +boundingBox.Width- (size / 2f);
                    y = boundingBox.Y + (boundingBox.Height / 2f) - (size / 2f);
                    start = 90;
                    end = 180;
                    break;
            }

            g.FillPie(notchFillBrush, x, y, size, size, start, end);
        }
        Pen dieOutlinePen = new Pen(Color.White);
        /// <summary>
        /// 绘画单个die的样式和选中的die样式
        /// </summary>
        /// <param name="g"></param>
        /// <param name="boundingBox"></param>
        /// <param name="data"></param>
        /// <param name="selectDies"></param>
        /// <param name="dieSize"></param>
        private void drawDies(Graphics g, RectangleF boundingBox, Die[,] data, List<Die> selectDies, SizeF dieSize)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    Color fill = Color.FromArgb(_DieAlpha, colors[data[x, y].ColorIndex]);
                    PointF position = new PointF(boundingBox.X + (float)x * dieSize.Width, boundingBox.Y + (float)y * dieSize.Height);
                    RectangleF die = new RectangleF(position, dieSize);

                    //判断4个角是否在显示范围内，不在的话不显示
                    if (!g.IsVisible(position.X, position.Y) || !g.IsVisible(position.X, position.Y + dieSize.Height) || !g.IsVisible(position.X + dieSize.Width, position.Y) || !g.IsVisible(position.X + dieSize.Width, position.Y + dieSize.Height))
                    {
                        //g.FillRectangle(new SolidBrush(Color.FromArgb(0, colors[data[x, y].ColorIndex])), die);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.DimGray)), die);
                        g.DrawRectangle(dieOutlinePen, die.X, die.Y, die.Width, die.Height);
                        data[x, y].IsVisible = false;
                    }
                    else
                    {
                        data[x, y].IsVisible = true;
                        //visibleDataset.Add(data[x, y]);
                        // 如果是选中的die的话,这里是填充die的颜色或者改小die的面积
                        if (selectDies != null) // 这里是多选的
                        {
                            Die temp = selectDies.Find(v => v.XIndex == x && v.YIndex == y);
                            if (temp != null && temp.XIndex == x && temp.YIndex == y)
                            {
                                fill = Color.FromArgb(20, colors[temp.ColorIndex]); // 如果要改变选中区域的颜色只用改变selectDies的颜色id就行了
                                position = new PointF(boundingBox.X + (float)x * dieSize.Width, boundingBox.Y + (float)y * dieSize.Height);
                                die = new RectangleF(position, dieSize);
                            }
                        }


                        g.FillRectangle(new SolidBrush(fill), die);
                        g.DrawRectangle(dieOutlinePen, die.X, die.Y, die.Width, die.Height);
                    }
                }
            }
            dataset = data;
            //if (savePath != "")
            //{
            //    string aa = JsonConvert.SerializeObject(visibleDataset);
            //    File.WriteAllText(savePath, aa);
            //    VisibleDatasetSavePath = "";
            //}
            #region 绘画选中die的样式
            if (selectDies != null) // 多个die
            {
                Die[] dies =  GetLeftTopAndRightBottomDie(_selectStartDie, _selectEndDie);
                PointF positionSelect = new PointF(boundingBox_.X + (float)dies[0].XIndex * dieSize_.Width - 1, boundingBox_.Y + (float)dies[0].YIndex * dieSize_.Height - 1);
                RectangleF dieSelect = new RectangleF(positionSelect, new SizeF((dies[1].XIndex - dies[0].XIndex + 1) * dieSize_.Width + 2, (dies[1].YIndex - dies[0].YIndex + 1) * dieSize_.Height + 2));
               
                //四个边角只要右一个不在里面就直接返回不绘制
                if (!g.IsVisible(positionSelect.X, positionSelect.Y) || !g.IsVisible(positionSelect.X, positionSelect.Y + dieSize.Height) || !g.IsVisible(positionSelect.X + dieSize.Width, positionSelect.Y) || !g.IsVisible(positionSelect.X + dieSize.Width, positionSelect.Y + dieSize.Height))
                {
                    return;
                }
                
                g.DrawRectangle(new Pen(Color.White, 4), dieSelect.X + 2, dieSelect.Y + 2, dieSelect.Width - 4, dieSelect.Height - 4);
                g.DrawRectangle(new Pen(Color.Black, 4), dieSelect.X - 2, dieSelect.Y - 2, dieSelect.Width + 4, dieSelect.Height + 4);
            }
            else if (_SelectOneDie !=null) //单个die选中
            {
                // 放到for循环外主要是为了解决右边和下边的边框被重绘了造成部分白色区域
                Color fillSelect = Color.FromArgb(255, colors[_SelectOneDie.ColorIndex]);
                PointF positionSelect = new PointF(boundingBox_.X + (float)_SelectX * dieSize_.Width - 1, boundingBox_.Y + (float)_SelectY * dieSize_.Height - 1);
                RectangleF dieSelect = new RectangleF(positionSelect, new SizeF(dieSize_.Width + 2, dieSize_.Height + 2));
                //四个边角只要右一个不在里面就直接返回不绘制
                if (!g.IsVisible(positionSelect.X, positionSelect.Y) || !g.IsVisible(positionSelect.X, positionSelect.Y + dieSize.Height) || !g.IsVisible(positionSelect.X + dieSize.Width, positionSelect.Y) || !g.IsVisible(positionSelect.X + dieSize.Width, positionSelect.Y + dieSize.Height))
                {
                    return;
                }

                g.FillRectangle(new SolidBrush(fillSelect), dieSelect);
                // 这里是重绘选中的芯片
                g.DrawRectangle(new Pen(Color.White, 4), dieSelect.X + 2, dieSelect.Y + 2, dieSelect.Width - 4, dieSelect.Height - 4);
                g.DrawRectangle(new Pen(Color.Black, 4), dieSelect.X - 2, dieSelect.Y - 2, dieSelect.Width + 4, dieSelect.Height + 4);
            }
            #endregion
        }

        
        private void registerEvents()
        {
            // Event to be registered
            if (interactive)
            {
                //this.MouseClick += Wafermap_MouseClick;
                this.MouseDown += Wafermap_MouseDown;
                this.MouseUp += Wafermap_MouseUp;
                this.MouseMove += Wafermap_MouseMove;
                KeyDown += Wafermap_KeyDown;
            }
            else
            {
                this.MouseDown -= Wafermap_MouseDown;
                this.MouseUp -= Wafermap_MouseUp;
                this.MouseMove -= Wafermap_MouseMove;
                KeyDown -= Wafermap_KeyDown;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        private void Wafermap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                SelectX--;
            }
            else if (e.KeyData == Keys.Right)
            {
                SelectX++;
            }
            else if (e.KeyData == Keys.Up)
            {
                SelectY--;
            }
            else if (e.KeyData == Keys.Down)
            {
                SelectY++;
            }
        }

        private void Wafermap_MouseDown(object sender, MouseEventArgs e)
        {
            // Basically the same as MouseMove, just a few other infos passed
            float x_coord = ((float)e.X - boundingBox_.X) / dieSize_.Width;
            float y_coord = ((float)e.Y - boundingBox_.Y) / dieSize_.Height;
            int x = (int)Math.Floor(x_coord);
            int y = (int)Math.Floor(y_coord);
            try
            {
                if (_SelectDies != null) { _SelectDies.Clear(); _SelectDies = null; }
                SelectMode = true;
                dieMouseDown(dataset[x, y], e);
                _SelectX = x;
                _SelectY = y;
                _SelectOneDie = dataset[x, y];
                
                _selectStartDie = _SelectOneDie;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Not good click..");
                //throw ex;
            }
        }

        private int previousX = -1, previousY = -1;
        private void Wafermap_MouseMove(object sender, MouseEventArgs e)
        {
            // This one is going to be tricky
            // We need to calculate the die coordinates from screen coordinates
            // We have global vars boundingBox_ and dieSize_
            float x_coord=((float)e.X - boundingBox_.X) / dieSize_.Width;
            float y_coord = ((float)e.Y - boundingBox_.Y) / dieSize_.Height;
            int x = (int)Math.Floor(x_coord);
            int y = (int)Math.Floor(y_coord);
            try
            {
                if (dataset != null)
                {
                    Die previousDie = null;
                    if (previousX != -1 && previousY != -1) previousDie = dataset[previousX, previousY];
                    dieEntered(previousDie, dataset[x, y]);
                    if (previousX != x) previousX = x;
                    if (previousY != y) previousY = y;
                }
                if (SelectMode)
                {
                    _selectEndDie = dataset[x, y];
                    DrawSelectDies();
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Not good move..");
            }
        }
        private void Wafermap_MouseUp(object sender, MouseEventArgs e)
        {
            // Basically the same as MouseMove, just a few other infos passed
            float x_coord = ((float)e.X - boundingBox_.X) / dieSize_.Width;
            float y_coord = ((float)e.Y - boundingBox_.Y) / dieSize_.Height;
            int x = (int)Math.Floor(x_coord);
            int y = (int)Math.Floor(y_coord);
            try
            {
                dieMouseUp(dataset[x, y], e);
                SelectMode = false;
                _selectEndDie = dataset[x, y];
                DrawSelectDies();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Not good click..");
                //throw ex;
            }
        }



        // 如果鼠标指针进入格子，此方法将被调用
        private void dieEntered(Die previousDie, Die currentDie){
            if (previousDie == null || currentDie == null) return;
            if (previousDie.XIndex == currentDie.XIndex && previousDie.YIndex == currentDie.YIndex) updateDie(currentDie);
            else updateDie(previousDie, currentDie);
        }
        // This method should get overridden if you want to reakt on clicks on a die
        private void dieMouseDown(Die die, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownDie(die);
            }
            else
            { }
            die.MouseArgs = e;
            if (OnDieClick != null) OnDieClick(this, die);
        }
        private void dieMouseUp(Die die, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseUpDie(die);
            }
            else
            {
            }
        }

        /// <summary>
        /// 获取矩形左上角和右下角两个点
        /// </summary>
        /// <param name="startDie"></param>
        /// <param name="endDie"></param>
        /// <returns></returns>
        private Die[] GetLeftTopAndRightBottomDie(Die startDie, Die endDie)
        {
            Die[] dies = new Die[2];
            if (endDie.XIndex >= startDie.XIndex) // end的X在start的右边
            {
                if (endDie.YIndex >= startDie.YIndex) // 那么end是右下角那个点
                {
                    dies[0] = startDie;
                    dies[1] = endDie;

                }
                else // end是右上角那个点
                {
                    int topLeftX = startDie.XIndex;
                    int topLeftY = endDie.YIndex;
                    int bottomRightX = endDie.XIndex;
                    int bottomRightY = startDie.YIndex;
                    dies[0] = dataset[topLeftX, topLeftY];
                    dies[1] = dataset[bottomRightX, bottomRightY];
                }
            }
            else
            {
                if (endDie.YIndex >= startDie.YIndex) // 那么end是左下角那个点
                {
                    int topLeftX = endDie.XIndex;
                    int topLeftY = startDie.YIndex;
                    int bottomRightX = startDie.XIndex;
                    int bottomRightY = endDie.YIndex;
                    dies[0] = dataset[topLeftX, topLeftY];
                    dies[1] = dataset[bottomRightX, bottomRightY];
                }
                else // end是左上角那个点
                {
                    dies[0] = endDie;
                    dies[1] = startDie;
                }
            }
            return dies;
        }
        /// <summary>
        /// 实时绘制选中的dies
        /// </summary>
        private void DrawSelectDies()
        {
            if (!_SelectMultiple) return;
            if (_selectEndDie == null || _selectStartDie == null) return;
            if (_selectStartDie.XIndex == _selectEndDie.XIndex && _selectStartDie.YIndex == _selectEndDie.YIndex) { _SelectDies = null;return; }


            Die[] dies = GetLeftTopAndRightBottomDie(_selectStartDie, _selectEndDie);
            _SelectRegionDiagonalDie = dies;
            if (dies.Length < 2) return;
            if (_SelectDies != null) _SelectDies.Clear();
            _SelectDies = new List<Die>();
            for (int x = dies[0].XIndex; x <= dies[1].XIndex; x++)
            {
                for (int y = dies[0].YIndex; y <= dies[1].YIndex; y++)
                {
                    _SelectDies.Add(dataset[x, y]);
                }
            }
            Invalidate();
        }

        Pen dieMoveingPen = new Pen(Color.Black);
        // This method should be used to change die coloring of a bin directly
        // This is needed to avoid redraws when not neccessary
        // The updated bins are filled with higher alpha to highlight them
        /// <summary>
        /// 移动的时候显示
        /// </summary>
        /// <param name="previousX"></param>
        /// <param name="previousY"></param>
        /// <param name="previousBincode"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bincode"></param>
        private void updateDie(Die previosDie, Die currentDie)
        {
            //Invalidate();
            //Color fill = Color.FromArgb(80, colors[bincode]);
            PointF position = new PointF(boundingBox_.X + (float)currentDie.XIndex * dieSize_.Width, boundingBox_.Y + (float)currentDie.YIndex * dieSize_.Height);
            RectangleF die = new RectangleF(position, dieSize_);
            Graphics g = this.CreateGraphics();
            // update clipping
            // Create graphics path.
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(boundingBox_);
            // Set clipping region to path.
            g.SetClip(clipPath, CombineMode.Replace);
            // 移动的时候也不显示不在显示范围内的
            if (!g.IsVisible(position.X, position.Y) || !g.IsVisible(position.X, position.Y + dieSize_.Height) || !g.IsVisible(position.X + dieSize_.Width, position.Y) || !g.IsVisible(position.X + dieSize_.Width, position.Y + dieSize_.Height))
            {
                return;
            }
            //Draw previous
            if (previousX != -1 && previousY != -1 && previousX != _SelectX && previousX != _SelectY) // 加了！=selectX 主要是防止已经选中的划过会出现白边框
            {
                Color previousFill = Color.FromArgb(_DieAlpha, colors[previosDie.ColorIndex]);
                PointF previousPosition = new PointF(boundingBox_.X + (float)previousX * dieSize_.Width, boundingBox_.Y + (float)previousY * dieSize_.Height);
                RectangleF pDieRect = new RectangleF(previousPosition, dieSize_);
                g.FillRectangle(new SolidBrush(previousFill), pDieRect);
                g.DrawRectangle(dieOutlinePen, pDieRect.X, pDieRect.Y, pDieRect.Width, pDieRect.Height);
                //g.DrawRectangle(dieOutlinePen, previousDie.X + 5, previousDie.Y + 5, previousDie.Width - 10, previousDie.Height - 10);
            }


            // Draw
            //g.FillRectangle(new SolidBrush(fill), die);

            g.DrawRectangle(dieMoveingPen, die.X, die.Y, die.Width, die.Height);
            
            Invalidate();
            //g.DrawRectangle(dieOutlinePen, die.X - 5, die.Y - 5, die.Width + 10, die.Height + 10);
        }

        /// <summary>
        /// 移动的时候显示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bincode"></param>
        private void updateDie(Die d)
        {
            //Color fill = Color.FromArgb(255, colors[bincode]);
            PointF position = new PointF(boundingBox_.X + (float)d.XIndex * dieSize_.Width, boundingBox_.Y + (float)d.YIndex * dieSize_.Height);
            RectangleF die = new RectangleF(position, dieSize_);
            Graphics g = this.CreateGraphics();
            // update clipping
            // Create graphics path.
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(boundingBox_);
            // Set clipping region to path.
            g.SetClip(clipPath, CombineMode.Replace);
            // 移动的时候也不显示不在显示范围内的
            if (!g.IsVisible(position.X, position.Y) || !g.IsVisible(position.X, position.Y + dieSize_.Height) || !g.IsVisible(position.X + dieSize_.Width, position.Y) || !g.IsVisible(position.X + dieSize_.Width, position.Y + dieSize_.Height))
            {
                return;
            }
            // Draw
            //g.FillRectangle(new SolidBrush(fill), die);

            // 内缩白色区域，更加凸显 
            g.DrawRectangle(new Pen(Color.White, 3), die.X + 2, die.Y + 2, die.Width - 4, die.Height - 4);
            g.DrawRectangle(dieMoveingPen, die.X, die.Y, die.Width, die.Height);
            //g.DrawRectangle(dieOutlinePen, die.X - 5, die.Y - 5, die.Width + 10, die.Height + 10);
        }

        private void MouseDownDie(Die d)
        {
            if (d == null) return;
            Invalidate();
            Color fill = Color.FromArgb(255, colors[d.ColorIndex]);
            PointF position = new PointF(boundingBox_.X + (float)d.XIndex * dieSize_.Width - 2, boundingBox_.Y + (float)d.YIndex * dieSize_.Height - 2);
            RectangleF die = new RectangleF(position, new SizeF(dieSize_.Width + 4, dieSize_.Height + 4));
            Graphics g = this.CreateGraphics();
            // update clipping
            // Create graphics path.
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(boundingBox_);
            // Set clipping region to path.
            g.SetClip(clipPath, CombineMode.Replace);

            PointF tesPos = new PointF(position.X - 2, position.Y - 2);
            // 点击也不会显示的时候也不显示不在显示范围内的
            if (!g.IsVisible(tesPos.X, tesPos.Y) || !g.IsVisible(tesPos.X, tesPos.Y + dieSize_.Height) || !g.IsVisible(tesPos.X + dieSize_.Width, tesPos.Y) || !g.IsVisible(tesPos.X + dieSize_.Width, tesPos.Y + dieSize_.Height))
            {
                return;
            }
            // Draw
            g.FillRectangle(new SolidBrush(fill), die);
            //g.DrawRectangle(dieOutlinePen, die.X - 5, die.Y - 5, die.Width + 10, die.Height + 10);

            //g.DrawRectangle(dieMoveingPen, die.X, die.Y, die.Width, die.Height);
            Invalidate();
        }

        private void MouseUpDie(Die d)
        {
            //return;
            Color fill = Color.FromArgb(255, colors[d.ColorIndex]);
            PointF position = new PointF(boundingBox_.X + (float)d.XIndex * dieSize_.Width + 2, boundingBox_.Y + (float)d.YIndex * dieSize_.Height + 2);
            RectangleF die = new RectangleF(position, new SizeF(dieSize_.Width - 4, dieSize_.Height - 4));
            Graphics g = this.CreateGraphics();
            // update clipping
            // Create graphics path.
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(boundingBox_);
            // Set clipping region to path.
            g.SetClip(clipPath, CombineMode.Replace);
            // 移动的时候也不显示不在显示范围内的
            PointF tesPos = new PointF(position.X - 2, position.Y - 2);
            if (!g.IsVisible(tesPos.X, tesPos.Y) || !g.IsVisible(tesPos.X, tesPos.Y + dieSize_.Height) || !g.IsVisible(tesPos.X + dieSize_.Width, tesPos.Y) || !g.IsVisible(tesPos.X + dieSize_.Width, tesPos.Y+ dieSize_.Height))
            {
                return;
            }
            // Draw
            g.FillRectangle(new SolidBrush(fill), die);
            //g.DrawRectangle(dieOutlinePen, die.X - 5, die.Y - 5, die.Width + 10, die.Height + 10);

            g.DrawRectangle(dieMoveingPen, die.X, die.Y, die.Width, die.Height);
            //Invalidate();
            updateDie(d);
        }
    }
}
