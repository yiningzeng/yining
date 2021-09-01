using HalconDotNet;
using System;
using System.Collections.Generic;
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
        public void FindEdge(HObject hObject, out HObject objectSelected, out double pointXPulse, out double pointYPulse, int xPulse, int yPulse, double Pixel_Length = 3.2, double S_scope = 2, int width = 4096, int height = 3072)
        {
            HOperatorSet.Rgb1ToGray(hObject, out hObject);
            HDevProcedureCall call = new HDevProcedureCall(new HDevProcedure(m_Program, "Edge_point"));
            call.SetInputIconicParamObject("Rectangle", hObject);
            call.SetInputIconicParamObject("GrayImage", hObject);
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
