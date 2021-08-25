using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
    public partial class HDevelopExport
    {
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
        public HDevelopExport()
        {
            // Default settings used in HDevelop
            HOperatorSet.SetSystem("width", 512);
            HOperatorSet.SetSystem("height", 512);
            if (HalconAPI.isWindows)
                HOperatorSet.SetSystem("use_window_thread", "true");
            action();
        }
#endif

        // Procedures 
        // External procedures 
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

#if !NO_EXPORT_MAIN
        // Main procedure 
        private void action()
        {


            // Local iconic variables 

            HObject ho_Image, ho_GrayImage, ho_Rectangle;
            HObject ho_ImageReduced, ho_Image1 = null, ho_GrayImage1 = null;

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_ModelID = new HTuple(), hv_ImageFiles = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Score = new HTuple(), hv_ROW_Trans = new HTuple();
            HTuple hv_Column_trans = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_GrayImage1);
            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, @"C:\Program Files (x86)\MindVision\Image\Camera MV-XG1205GC#A9FE71EB-Snapshot-20210823202232-5197321903826.BMP");
            ho_GrayImage.Dispose();
            HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.CloseWindow(HDevWindowStack.Pop());
            }
            hv_WindowHandle.Dispose();
            dev_open_window_fit_image(ho_GrayImage, 0, 0, -1, -1, out hv_WindowHandle);
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_GrayImage, HDevWindowStack.GetActive());
            }
            hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
            HOperatorSet.DrawRectangle1(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2,
                out hv_Column2);
            ho_Rectangle.Dispose();
            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle, out ho_ImageReduced);
            hv_Area.Dispose(); hv_Row3.Dispose(); hv_Column3.Dispose();
            HOperatorSet.AreaCenter(ho_Rectangle, out hv_Area, out hv_Row3, out hv_Column3);

            hv_ModelID.Dispose();
            HOperatorSet.CreateShapeModel(ho_ImageReduced, "auto", -0.39, 0.79, "auto", "auto",
                "use_polarity", "auto", "auto", out hv_ModelID);

            //Image Acquisition 01: Code generated by Image Acquisition 01
            hv_ImageFiles.Dispose();
            HOperatorSet.ListFiles("C:/Program Files (x86)/MindVision/Image", (new HTuple("files")).TupleConcat(
                "follow_links"), out hv_ImageFiles);
            {
                HTuple ExpTmpOutVar_0;
                HOperatorSet.TupleRegexpSelect(hv_ImageFiles, (new HTuple("\\.(tif|tiff|gif|bmp|jpg|jpeg|jp2|png|pcx|pgm|ppm|pbm|xwd|ima|hobj)$")).TupleConcat(
                    "ignore_case"), out ExpTmpOutVar_0);
                hv_ImageFiles.Dispose();
                hv_ImageFiles = ExpTmpOutVar_0;
            }
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ImageFiles.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Image1.Dispose();
                    HOperatorSet.ReadImage(out ho_Image1, hv_ImageFiles.TupleSelect(hv_Index));
                }
                //Image Acquisition 01: Do something
                ho_GrayImage1.Dispose();
                HOperatorSet.Rgb1ToGray(ho_Image1, out ho_GrayImage1);
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Score.Dispose();
                HOperatorSet.FindShapeModel(ho_GrayImage1, hv_ModelID, -0.39, 0.79, 0.5, 1,
                    0.5, "least_squares", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle,
                    out hv_Score);
                dev_display_shape_matching_results(hv_ModelID, "red", hv_Row, hv_Column, hv_Angle,
                    1, 1, 0);
                hv_ROW_Trans.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ROW_Trans = hv_Row - hv_Row3;
                }
                hv_Column_trans.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Column_trans = hv_Column - hv_Column3;
                }
                // stop(...); only in hdevelop
            }

            ho_Image.Dispose();
            ho_GrayImage.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_Image1.Dispose();
            ho_GrayImage1.Dispose();

            hv_WindowHandle.Dispose();
            hv_Row1.Dispose();
            hv_Column1.Dispose();
            hv_Row2.Dispose();
            hv_Column2.Dispose();
            hv_Area.Dispose();
            hv_Row3.Dispose();
            hv_Column3.Dispose();
            hv_ModelID.Dispose();
            hv_ImageFiles.Dispose();
            hv_Index.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Angle.Dispose();
            hv_Score.Dispose();
            hv_ROW_Trans.Dispose();
            hv_Column_trans.Dispose();

        }

#endif


    }

}
