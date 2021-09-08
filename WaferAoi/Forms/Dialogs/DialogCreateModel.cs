using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Forms;
using static WaferAoi.Tools.EMUMS;
using static WaferAoi.Tools.Utils;

namespace WaferAoi
{
    public partial class DialogCreateModel : DarkDialog
    {
        private string ChipModelRegionFileName, ChipModelFileName;
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(3));
        HDevProgramHelper hDevProgramHelper;
        HObject hObject;
        InterLayerDraw ilMain;
        public DialogCreateModel()
        {
            InitializeComponent();
            Ini();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ho"></param>
        /// <param name="_ChipModelRegionFileName">模型的区域保存地址</param>
        /// <param name="_ChipModelFileName">魔心保存地址</param>
        /// <param name="Title"></param>
        public DialogCreateModel(HObject ho, string _ChipModelRegionFileName, string _ChipModelFileName, string Title = "模型制作") : this()
        {
            hObject = ho;
            ChipModelFileName = _ChipModelFileName;
            ChipModelRegionFileName = _ChipModelRegionFileName;
            this.Text = Title;
        }

        public void Ini()
        {
            ilMain = new InterLayerDraw(hswcMain);
            this.btnOk.Text = "保存";
            this.btnCancel.Text = "取消";
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.Load += DialogCreateModel_Load;
            this.FormClosed += DialogCreateModel_FormClosed;
            hswcMain.MouseWheel += hswcMain.HSmartWindowControl_MouseWheel;

            hswcMain.HMouseMove += HswcMain_HMouseMove;
            dlvwProgress.Items.Clear();
            dlvwProgress.Items.Add(new DarkListItem("框选模板区域"));
            dlvwProgress.Items.Add(new DarkListItem("保存匹配的模板"));
            dlvwProgress.Items.Add(new DarkListItem("框选检测区域"));
            dlvwProgress.Items.Add(new DarkListItem("保存"));
            dlvwProgress.SetStartNum(0);
        }

        private void DialogCreateModel_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ilMain.Dispose();
            }
            catch { }
        }

        private void HswcMain_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                ilMain.X = Convert.ToInt32(e.X);
                ilMain.Y = Convert.ToInt32(e.Y);
            }
            catch (Exception er) { }
        }
        private void 画一个矩形区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilMain.DrawRectange1();
        }

        private void DialogCreateModel_Load(object sender, EventArgs e)
        {
            ilMain.ShowImg(hObject);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = DarkMessageBox.ShowWarning(@"确定要关闭么", @"提醒", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;

            base.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SaveModel_Click(object sender, EventArgs e)
        {
            _ = CreateModelAsync();
        }
        private async Task CreateModelAsync()
        {
            await Task.Run(() => CreateModel());
        }

        private void CreateModel(bool save = true)
        {
            HOperatorSet.GenEmptyObj(out HObject region);
            foreach (var obj in ilMain.drawingObjects)
            {
                HOperatorSet.Union2(obj.GetDrawingObjectIconic(), region, out region);
            }

            HOperatorSet.Rgb1ToGray(ilMain.hImage, out HObject grayImage);
            HOperatorSet.ReduceDomain(grayImage, region, out HObject imageReduced);
            HOperatorSet.AreaCenter(region, out HTuple area, out HTuple regionCenterRow, out HTuple regionCenterCol);


            // create_shape_model (ImageReduced, 'auto', -0.39, 0.79, 'auto', 'auto', 'use_polarity', 'auto', 'auto', ModelID1)
            HOperatorSet.CreateShapeModel(imageReduced, "auto", -0.39, 0.79, "auto", "auto", "use_polarity", "auto", "auto", out HTuple ModelId);

            if (save)
            {
                HOperatorSet.WriteRegion(region, ChipModelRegionFileName);
                HOperatorSet.WriteShapeModel(ModelId, ChipModelFileName);
            }
            grayImage.Dispose();
            imageReduced.Dispose();
            region.Dispose();
        }
        /*
         *矫正
 Angle5:=0
 *输入
 *GrayImage1 为采集的图像灰度图
 *Row Column Angle模板匹配的结果
 *Row5 Column5 Angle5 为原始模板的中心坐标，Angle5这里一般为0
 
 *输出
 *ImageAffineTrans 为输出的图像，可以跟原模板图像对比一下，感受一下
 *HoMat2D 这里为转换矩阵*/
        private void ImageTrans(HObject GrayImage1, out HObject ImageAffineTrans, HTuple Row, HTuple Column, HTuple Angle, HTuple Row5, HTuple Column5, HTuple Angle5, out HTuple HomMat2D)
        {
            HOperatorSet.GenEmptyObj(out ImageAffineTrans);
            HomMat2D = new HTuple();
            try
            {
                //vector_angle_to_rigid(Row, Column, Angle, Row5, Column5, Angle5, HomMat2D)
                HOperatorSet.VectorAngleToRigid(Row, Column, Angle, Row5, Column5, Angle5, out HomMat2D);
                //affine_trans_image(GrayImage1, ImageAffineTrans, HomMat2D, 'constant', 'false')
                HOperatorSet.AffineTransImage(GrayImage1, out ImageAffineTrans, HomMat2D, "constant", "false");
            }
            catch (HalconException er) { }
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            //DialogTestModel dialogTestModel = new DialogTestModel(programConfig);
            //dialogTestModel.ShowDialog();
            if (!File.Exists(ChipModelFileName) || !File.Exists(ChipModelRegionFileName))
            {
                DarkMessageBox.ShowError("模型相关文件不存在，请重新保存");
                return;
            }
            string filePath;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择图片";
            fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = Path.GetFullPath(fileDialog.FileName);
                new HDevelopExportTest(ChipModelFileName, ChipModelRegionFileName, filePath);
                fileDialog.Dispose();
            }
            else
            {
                fileDialog.Dispose();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.CloseWindow(HDevWindowStack.Pop());
            }
        }

              /*
object[] objectArray = new object[6];//这里的2就是改成你要传递几个参数
objectArray[0] = "";
object param = (object)objectArray;
tasklst.Add(fac.StartNew(obs =>
{
return true;
}, param));
*/
    }


    public partial class HDevelopExportTest
    {

#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
        public HDevelopExportTest(string _ModelFileName, string _ModelRegionFileName, string testFileName)
        {
            // Default settings used in HDevelop
            HOperatorSet.SetSystem("width", 512);
            HOperatorSet.SetSystem("height", 512);
            if (HalconAPI.isWindows)
                HOperatorSet.SetSystem("use_window_thread", "true");
            action(_ModelFileName, _ModelRegionFileName, testFileName);
        }
#endif

        #region thr
        // Procedures 
        // Chapter: Matching / Shape-Based
        // Short Description: Display the results of Shape-Based Matching. 
        public void dev_display_shape_matching_results(HTuple hv_ModelID, HTuple hv_Color,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_ScaleR, HTuple hv_ScaleC,
            HTuple hv_Model)
        {



            // Local iconic variables 

            HObject ho_ModelContours = null, ho_ContoursAffinTrans = null;

            // Local control variables 

            HTuple hv_NumMatches = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Match = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DScale = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_HomMat2DTranslate = new HTuple();
            HTuple hv_Model_COPY_INP_TMP = new HTuple(hv_Model);
            HTuple hv_ScaleC_COPY_INP_TMP = new HTuple(hv_ScaleC);
            HTuple hv_ScaleR_COPY_INP_TMP = new HTuple(hv_ScaleR);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
            //This procedure displays the results of Shape-Based Matching.
            //
            hv_NumMatches.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumMatches = new HTuple(hv_Row.TupleLength()
                    );
            }
            if ((int)(new HTuple(hv_NumMatches.TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_ScaleR_COPY_INP_TMP.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleR_COPY_INP_TMP, out ExpTmpOutVar_0);
                        hv_ScaleR_COPY_INP_TMP.Dispose();
                        hv_ScaleR_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_ScaleC_COPY_INP_TMP.TupleLength())).TupleEqual(
                    1))) != 0)
                {
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_ScaleC_COPY_INP_TMP, out ExpTmpOutVar_0);
                        hv_ScaleC_COPY_INP_TMP.Dispose();
                        hv_ScaleC_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    hv_Model_COPY_INP_TMP.Dispose();
                    HOperatorSet.TupleGenConst(hv_NumMatches, 0, out hv_Model_COPY_INP_TMP);
                }
                else if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength()
                    )).TupleEqual(1))) != 0)
                {
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_Model_COPY_INP_TMP, out ExpTmpOutVar_0);
                        hv_Model_COPY_INP_TMP.Dispose();
                        hv_Model_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ModelID.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ModelContours.Dispose();
                        HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID.TupleSelect(
                            hv_Index), 1);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_Color.TupleSelect(
                                hv_Index % (new HTuple(hv_Color.TupleLength()))));
                        }
                    }
                    HTuple end_val18 = hv_NumMatches - 1;
                    HTuple step_val18 = 1;
                    for (hv_Match = 0; hv_Match.Continue(end_val18, step_val18); hv_Match = hv_Match.TupleAdd(step_val18))
                    {
                        if ((int)(new HTuple(hv_Index.TupleEqual(hv_Model_COPY_INP_TMP.TupleSelect(
                            hv_Match)))) != 0)
                        {
                            hv_HomMat2DIdentity.Dispose();
                            HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HomMat2DScale.Dispose();
                                HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_ScaleR_COPY_INP_TMP.TupleSelect(
                                    hv_Match), hv_ScaleC_COPY_INP_TMP.TupleSelect(hv_Match), 0, 0, out hv_HomMat2DScale);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HomMat2DRotate.Dispose();
                                HOperatorSet.HomMat2dRotate(hv_HomMat2DScale, hv_Angle.TupleSelect(hv_Match),
                                    0, 0, out hv_HomMat2DRotate);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HomMat2DTranslate.Dispose();
                                HOperatorSet.HomMat2dTranslate(hv_HomMat2DRotate, hv_Row.TupleSelect(
                                    hv_Match), hv_Column.TupleSelect(hv_Match), out hv_HomMat2DTranslate);
                            }
                            ho_ContoursAffinTrans.Dispose();
                            HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans,
                                hv_HomMat2DTranslate);
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ContoursAffinTrans, HDevWindowStack.GetActive()
                                    );
                            }
                        }
                    }
                }
            }
            ho_ModelContours.Dispose();
            ho_ContoursAffinTrans.Dispose();

            hv_Model_COPY_INP_TMP.Dispose();
            hv_ScaleC_COPY_INP_TMP.Dispose();
            hv_ScaleR_COPY_INP_TMP.Dispose();
            hv_NumMatches.Dispose();
            hv_Index.Dispose();
            hv_Match.Dispose();
            hv_HomMat2DIdentity.Dispose();
            hv_HomMat2DScale.Dispose();
            hv_HomMat2DRotate.Dispose();
            hv_HomMat2DTranslate.Dispose();

            return;
        }

        // Chapter: Develop
        // Short Description: Open a new graphics window that preserves the aspect ratio of the given image. 
        public void dev_open_window_fit_image(HObject ho_Image, HTuple hv_Row, HTuple hv_Column,
            HTuple hv_WidthLimit, HTuple hv_HeightLimit, out HTuple hv_WindowHandle)
        {




            // Local iconic variables 

            // Local control variables 

            HTuple hv_MinWidth = new HTuple(), hv_MaxWidth = new HTuple();
            HTuple hv_MinHeight = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_ResizeFactor = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_TempWidth = new HTuple();
            HTuple hv_TempHeight = new HTuple(), hv_WindowWidth = new HTuple();
            HTuple hv_WindowHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowHandle = new HTuple();
            //This procedure opens a new graphics window and adjusts the size
            //such that it fits into the limits specified by WidthLimit
            //and HeightLimit, but also maintains the correct image aspect ratio.
            //
            //If it is impossible to match the minimum and maximum extent requirements
            //at the same time (f.e. if the image is very long but narrow),
            //the maximum value gets a higher priority,
            //
            //Parse input tuple WidthLimit
            if ((int)((new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(0))).TupleOr(
                new HTuple(hv_WidthLimit.TupleLess(0)))) != 0)
            {
                hv_MinWidth.Dispose();
                hv_MinWidth = 500;
                hv_MaxWidth.Dispose();
                hv_MaxWidth = 800;
            }
            else if ((int)(new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(
                1))) != 0)
            {
                hv_MinWidth.Dispose();
                hv_MinWidth = 0;
                hv_MaxWidth.Dispose();
                hv_MaxWidth = new HTuple(hv_WidthLimit);
            }
            else
            {
                hv_MinWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinWidth = hv_WidthLimit.TupleSelect(
                        0);
                }
                hv_MaxWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MaxWidth = hv_WidthLimit.TupleSelect(
                        1);
                }
            }
            //Parse input tuple HeightLimit
            if ((int)((new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(0))).TupleOr(
                new HTuple(hv_HeightLimit.TupleLess(0)))) != 0)
            {
                hv_MinHeight.Dispose();
                hv_MinHeight = 400;
                hv_MaxHeight.Dispose();
                hv_MaxHeight = 600;
            }
            else if ((int)(new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(
                1))) != 0)
            {
                hv_MinHeight.Dispose();
                hv_MinHeight = 0;
                hv_MaxHeight.Dispose();
                hv_MaxHeight = new HTuple(hv_HeightLimit);
            }
            else
            {
                hv_MinHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinHeight = hv_HeightLimit.TupleSelect(
                        0);
                }
                hv_MaxHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MaxHeight = hv_HeightLimit.TupleSelect(
                        1);
                }
            }
            //
            //Test, if window size has to be changed.
            hv_ResizeFactor.Dispose();
            hv_ResizeFactor = 1;
            hv_ImageWidth.Dispose(); hv_ImageHeight.Dispose();
            HOperatorSet.GetImageSize(ho_Image, out hv_ImageWidth, out hv_ImageHeight);
            //First, expand window to the minimum extents (if necessary).
            if ((int)((new HTuple(hv_MinWidth.TupleGreater(hv_ImageWidth))).TupleOr(new HTuple(hv_MinHeight.TupleGreater(
                hv_ImageHeight)))) != 0)
            {
                hv_ResizeFactor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ResizeFactor = (((((hv_MinWidth.TupleReal()
                        ) / hv_ImageWidth)).TupleConcat((hv_MinHeight.TupleReal()) / hv_ImageHeight))).TupleMax()
                        ;
                }
            }
            hv_TempWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TempWidth = hv_ImageWidth * hv_ResizeFactor;
            }
            hv_TempHeight.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TempHeight = hv_ImageHeight * hv_ResizeFactor;
            }
            //Then, shrink window to maximum extents (if necessary).
            if ((int)((new HTuple(hv_MaxWidth.TupleLess(hv_TempWidth))).TupleOr(new HTuple(hv_MaxHeight.TupleLess(
                hv_TempHeight)))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_ResizeFactor = hv_ResizeFactor * ((((((hv_MaxWidth.TupleReal()
                            ) / hv_TempWidth)).TupleConcat((hv_MaxHeight.TupleReal()) / hv_TempHeight))).TupleMin()
                            );
                        hv_ResizeFactor.Dispose();
                        hv_ResizeFactor = ExpTmpLocalVar_ResizeFactor;
                    }
                }
            }
            hv_WindowWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_WindowWidth = hv_ImageWidth * hv_ResizeFactor;
            }
            hv_WindowHeight.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_WindowHeight = hv_ImageHeight * hv_ResizeFactor;
            }
            //Resize window
            HOperatorSet.SetWindowAttr("background_color", "black");
            HOperatorSet.OpenWindow(hv_Row, hv_Column, hv_WindowWidth, hv_WindowHeight, 0, "visible", "", out hv_WindowHandle);
            HDevWindowStack.Push(hv_WindowHandle);
            if (HDevWindowStack.IsOpen())
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_ImageHeight - 1, hv_ImageWidth - 1);
                }
            }

            hv_MinWidth.Dispose();
            hv_MaxWidth.Dispose();
            hv_MinHeight.Dispose();
            hv_MaxHeight.Dispose();
            hv_ResizeFactor.Dispose();
            hv_ImageWidth.Dispose();
            hv_ImageHeight.Dispose();
            hv_TempWidth.Dispose();
            hv_TempHeight.Dispose();
            hv_WindowWidth.Dispose();
            hv_WindowHeight.Dispose();

            return;
        }

        // Local procedures 
        public void image_trans(HObject ho_GrayImage1, out HObject ho_ImageAffineTrans,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_Row5, HTuple hv_Column5,
            HTuple hv_Angle5, out HTuple hv_HomMat2D)
        {



            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            hv_HomMat2D = new HTuple();

            hv_HomMat2D.Dispose();
            HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Angle, hv_Row5, hv_Column5,
                hv_Angle5, out hv_HomMat2D);
            ho_ImageAffineTrans.Dispose();
            HOperatorSet.AffineTransImage(ho_GrayImage1, out ho_ImageAffineTrans, hv_HomMat2D,
                "constant", "false");



            return;
        }

        #endregion



#if !NO_EXPORT_MAIN
        // Main procedure 
        private void action(string _ModelFileName, string _ModelRegionFileName, string testFileName)
        {


            // Local iconic variables 

            HObject ho_RegionUnion2, ho_Image1, ho_GrayImage1;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row5 = new HTuple(), hv_Column5 = new HTuple();
            HTuple hv_ModelID1 = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_Angle1 = new HTuple();
            HTuple hv_Score1 = new HTuple(), hv_Angle5 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_GrayImage1);
            //read_image (Image, 'C:/Program Files (x86)/MindVision/Image/Camera MV-XG1205GC#A9FE71EB-Snapshot-20210906191252-4599218613494.BMP')

            //rgb1_to_gray (Image, GrayImage)

            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.CloseWindow(HDevWindowStack.Pop());
            }
            //dev_open_window_fit_image (GrayImage, 0, 0, -1, -1, WindowHandle)
            if (HDevWindowStack.IsOpen())
            {
                //dev_display (GrayImage)
            }





            //生成4个方块，软件里面画
            //draw_rectangle1 (WindowHandle, Row1, Column1, Row2, Column2)
            //* gen_rectangle1 (Rectangle, Row1, Column1, Row2, Column2)
            //draw_rectangle1 (WindowHandle, Row11, Column11, Row21, Column21)
            //* gen_rectangle1 (Rectangle1, Row11, Column11, Row21, Column21)
            //draw_rectangle1 (WindowHandle, Row12, Column12, Row22, Column22)
            //* gen_rectangle1 (Rectangle2, Row12, Column12, Row22, Column22)
            //draw_rectangle1 (WindowHandle, Row13, Column13, Row23, Column23)
            //* gen_rectangle1 (Rectangle3, Row13, Column13, Row23, Column23)

            //* union2 (Rectangle, Rectangle1, RegionUnion)
            //* union2 (Rectangle2, RegionUnion, RegionUnion)
            //* union2 (Rectangle3, RegionUnion, RegionUnion)

            //将所有方块转化为一个region
            //* union2 (Rectangle, Rectangle1, RegionUnion)
            //* union2 (Rectangle2, Rectangle3, RegionUnion1)
            //* union2 (RegionUnion, RegionUnion1, RegionUnion2)
            //reduce_domain (GrayImage, RegionUnion2, ImageReduced)

            //这里记录region中心位置，之后会用到
            ho_RegionUnion2.Dispose();
            HOperatorSet.ReadRegion(out ho_RegionUnion2, _ModelRegionFileName);
            hv_Area.Dispose(); hv_Row5.Dispose(); hv_Column5.Dispose();
            HOperatorSet.AreaCenter(ho_RegionUnion2, out hv_Area, out hv_Row5, out hv_Column5);

            hv_ModelID1.Dispose();
            HOperatorSet.ReadShapeModel(_ModelFileName, out hv_ModelID1);

            //*  create_ncc_model (ImageReduced, 'auto', -0.1, 0.2, 'auto', 'use_polarity', ModelID)
            //*  create_shape_model (ImageReduced, 'auto', -0.39, 0.79, 'auto', 'auto', 'use_polarity', 'auto', 'auto', ModelID1)

            ho_Image1.Dispose();
            HOperatorSet.ReadImage(out ho_Image1, testFileName);
            ho_GrayImage1.Dispose();
            HOperatorSet.Rgb1ToGray(ho_Image1, out ho_GrayImage1);
            hv_WindowHandle.Dispose();
            dev_open_window_fit_image(ho_Image1, 0, 0, -1, -1, out hv_WindowHandle);
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_Image1, HDevWindowStack.GetActive());
            }
            //find_ncc_model (GrayImage1, ModelID, -0.39, 0.79, 0.8, 1, 0.5, 'true', 4, Row, Column, Angle, Score)

            hv_Row3.Dispose(); hv_Column3.Dispose(); hv_Angle1.Dispose(); hv_Score1.Dispose();
            HOperatorSet.FindShapeModel(ho_GrayImage1, hv_ModelID1, -0.39, 0.79, 0.5, 1,
                0.5, "least_squares", 4, 0.9, out hv_Row3, out hv_Column3, out hv_Angle1,
                out hv_Score1);

            //dev_display_ncc_matching_results (ModelID, 'red', Row, Column, Angle, 0)
            // stop(...); only in hdevelop
            dev_display_shape_matching_results(hv_ModelID1, "red", hv_Row3, hv_Column3, hv_Angle1,
                1, 1, 0);


            //矫正
            hv_Angle5.Dispose();
            hv_Angle5 = 0;
            //输入
            //GrayImage1 为采集的图像灰度图
            //Row Column Angle模板匹配的结果
            //Row5 Column5 Angle5 为原始模板的中心坐标，Angle5这里一般为0

            //输出
            //ImageAffineTrans 为输出的图像，可以跟原模板图像对比一下，感受一下
            //HoMat2D 这里为转换矩阵
            //image_trans (GrayImage1, ImageAffineTrans, Row3, Column3, Angle1, Row5, Column5, Angle5, HomMat2D)


            ho_RegionUnion2.Dispose();
            ho_Image1.Dispose();
            ho_GrayImage1.Dispose();

            hv_WindowHandle.Dispose();
            hv_Area.Dispose();
            hv_Row5.Dispose();
            hv_Column5.Dispose();
            hv_ModelID1.Dispose();
            hv_Row3.Dispose();
            hv_Column3.Dispose();
            hv_Angle1.Dispose();
            hv_Score1.Dispose();
            hv_Angle5.Dispose();

        }

#endif


    }
}
