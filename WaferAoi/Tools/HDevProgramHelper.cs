using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferAoi.Tools
{
    public class HDevProgramHelper: IDisposable
    {
        HDevProgram m_Program;
        private HDevEngine MyEngine = new HDevEngine();
        public HDevProgramHelper(string ProgramPathString)
        {
            //MyEngine.SetProcedurePath(System.Windows.Forms.Application.StartupPath);
            //ProgramPathString = @"C:\Users\Administrator\source\repos\WindowsFormsApp2\WindowsFormsApp2\bin\x64\Debug\ol\chipInsp_0508.hdev";
            m_Program = new HDevProgram(ProgramPathString);
        }

        ~HDevProgramHelper()
        {
            if (m_Program!=null) m_Program.Dispose();
            if (MyEngine != null) MyEngine.Dispose();
        }

        void IDisposable.Dispose()
        {
            if (m_Program != null) m_Program.Dispose();
            if (MyEngine != null) MyEngine.Dispose();
        }


        /// <summary>
        ///         /**chip_deg 这里用halcon自带排版算子计算旋转角度，经测试效果还行
        /// </summary>
        /// <param name="hObject">为输入图像</param>
        /// <param name="ImageRotate1">为角度校正后的图像</param>
        /// <param name="OrientationAngle">OrientationAngle</param>
        /// <param name="Degree">为旋转角度</param>
        public void ChipDeg(HObject hObject, out HObject ImageRotate1, out HTuple OrientationAngle, out HTuple Degree)
        {
            HOperatorSet.GenEmptyObj(out ImageRotate1);
            OrientationAngle = new HTuple();
            Degree = new HTuple();
            try
            {
                using(HDevProcedureCall call = new HDevProcedureCall(new HDevProcedure(m_Program, "chip_deg")))
                {
                    call.SetInputIconicParamObject("Image3", hObject);

                    call.Execute();
                    ImageRotate1 = call.GetOutputIconicParamObject("ImageRotate1");
                    OrientationAngle = call.GetOutputCtrlParamTuple("OrientationAngle");
                    Degree = call.GetOutputCtrlParamTuple("Degree");
                }
            }
            catch (Exception er) { }
        }

        /// <summary>
        /// 用于边沿搜索的时候查找边沿的占比，默认应该设为40%-70%算找到边沿
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ratio"></param>
        /// <param name="darkthresh"></param>
        public bool CheckEdgeRatio(HObject image, out HTuple ratio, double darkthresh = 20, double min = 40, double max = 70)
        {
            ratio = new HTuple();
            bool ret = false;
            try
            {
                using(HDevProcedureCall call = new HDevProcedureCall(new HDevProcedure(m_Program, "border_check")))
                {
                    call.SetInputIconicParamObject("Image", image);
                    call.SetInputCtrlParamTuple("darkthresh", darkthresh);
                    call.Execute();
                    ratio = call.GetOutputCtrlParamTuple("ratio");
                    //return true;
#if DEBUG
                    Debug.WriteLine(ratio.D.ToString());
#endif
                    if (ratio.D >= min && ratio.D <= max)
                    {
                        ret = true;
                    }
                }
            }
            catch (HalconException er) { }
            return ret;
        }
        /// <summary>
        /// 获取边沿脉冲坐标用于计算圆心
        /// </summary>
        /// <param name="hObject">输入原始图像，函数带转灰度图</param>
        /// <param name="objectSelected">识别到的结果</param>
        /// <param name="pointXPulse">计算出的X脉冲，用于后面的计算圆心</param>
        /// <param name="pointYPulse">计算出的Y脉冲，用于后面的计算圆心<</param>
        /// <param name="xPulse">图像中心对应的x脉冲</param>
        /// <param name="yPulse">图像中心对应的y脉冲</param>
        /// <param name="Pixel_Length"></param>
        /// <param name="S_scope"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void FindEdge(string funcName, HObject hObject, out HObject objectSelected, out HTuple pointXPulse, out HTuple pointYPulse, int xPulse, int yPulse, int width = 4096, int height = 3072, double S_scope = 2, double Pixel_Length = 3.2, double mult_V = 0.5, double thresh = 30)
        {
            HOperatorSet.GenEmptyObj(out objectSelected);
            pointXPulse = new HTuple();
            pointYPulse = new HTuple();
            try
            {
                using(HDevProcedureCall call = new HDevProcedureCall(new HDevProcedure(m_Program, funcName)))
                {
                    HOperatorSet.Rgb1ToGray(hObject, out hObject);

                    call.SetInputIconicParamObject("Rectangle", hObject);
                    call.SetInputIconicParamObject("GrayImage", hObject);
                    call.SetInputCtrlParamTuple("mult_V", mult_V);
                    call.SetInputCtrlParamTuple("thresh", thresh);
                    call.SetInputCtrlParamTuple("Pixel_Length", Pixel_Length);
                    call.SetInputCtrlParamTuple("S_scope", S_scope);
                    call.SetInputCtrlParamTuple("XX", xPulse);
                    call.SetInputCtrlParamTuple("width", width);

                    call.SetInputCtrlParamTuple("YY", yPulse);
                    call.SetInputCtrlParamTuple("height", height);

                    call.Execute();
                    objectSelected = call.GetOutputIconicParamObject("ObjectSelected");
                    pointXPulse = call.GetOutputCtrlParamTuple("Point_X");
                    pointYPulse = call.GetOutputCtrlParamTuple("Point_Y");
                }
               
            }
            catch (Exception er) { }
        }


        /// <summary>
        /// 通过边沿计算圆心和半径
        /// </summary>
        /// <param name="xTop"></param>
        /// <param name="yTop"></param>
        /// <param name="xBottom"></param>
        /// <param name="yBottom"></param>
        /// <param name="xRight"></param>
        /// <param name="yRight"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        public void FindCenter(HTuple xTop, HTuple yTop, HTuple xBottom, HTuple yBottom, HTuple xRight, HTuple yRight, out HTuple centerX, out HTuple centerY, out HTuple radius)
        {
            radius = new HTuple();
            centerX = new HTuple();
            centerY = new HTuple();
            try
            {
                using (HDevProcedureCall call = new HDevProcedureCall(new HDevProcedure(m_Program, "find_center")))
                {
                    call.SetInputCtrlParamTuple("Point_X_top", xTop);
                    call.SetInputCtrlParamTuple("Point_Y_top", yTop);
                    call.SetInputCtrlParamTuple("Point_X_down", xBottom);
                    call.SetInputCtrlParamTuple("Point_Y_down", yBottom);
                    call.SetInputCtrlParamTuple("Point_X_right", xRight);
                    call.SetInputCtrlParamTuple("Point_Y_right", yRight);

                    call.Execute();
                    centerX = call.GetOutputCtrlParamTuple("Cx");
                    centerY = call.GetOutputCtrlParamTuple("Cy");
                    radius = call.GetOutputCtrlParamTuple("R");
                }
            }
            catch (Exception er) { }
        }
        /// <summary>
        /// 提取晶圆
        /// </summary>
        //public void GetImageRoi(HObject ImageIn, out HObject ImageStd, HTuple RotateFlg, HTuple ModelFlg, bool disposeImageIn = true)
        //{
        //HOperatorSet.GenEmptyObj(out ImageStd);
        //try
        //{
        //    HDevProcedureCall ipp_chip_preprocess_call = new HDevProcedureCall(new HDevProcedure(m_Program, "ipp_chip_preprocess"));
        //    #region 检测流程：前预处理
        //    //HOperatorSet.ReadImage(out HObject img, imgFilePath);
        //    ipp_chip_preprocess_call.SetInputIconicParamObject("ImageIn", ImageIn);
        //    ipp_chip_preprocess_call.SetInputIconicParamObject("ImageGs", ho_ImageGSStd);
        //    ipp_chip_preprocess_call.SetInputCtrlParamTuple("NccModelID", NccModelID);
        //    ipp_chip_preprocess_call.SetInputCtrlParamTuple("RotateFlg", RotateFlg);
        //    ipp_chip_preprocess_call.SetInputCtrlParamTuple("ModelFlg", ModelFlg);
        //    ipp_chip_preprocess_call.Execute();
        //    ImageStd = ipp_chip_preprocess_call.GetOutputIconicParamObject("ImageOut");
        //    #endregion
        //    ipp_chip_preprocess_call.Dispose();
        //}
        //catch (Exception er) { }
        //finally
        //{
        //    if (disposeImageIn)
        //        ImageIn.Dispose();
        //}
        //}
    }
}
