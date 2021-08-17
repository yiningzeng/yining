using YiNing.UI.Config;
using YiNing.UI.Controls;
using YiNing.UI.Docking;
using YiNing.UI.Forms;
using System.Drawing;
using System.Windows.Forms;
using System;
using YiNing.Tools;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

using MVSDK;//使用SDK接口
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;
using System.Runtime.InteropServices;

namespace WaferAoi
{
    public partial class DockCameraSetting : DarkDocument
    {
        #region 私有字段
        CommunicationManager serialPort = new CommunicationManager();

        protected CameraHandle m_hCamera = 0;             // 句柄
        protected IntPtr m_ImageBuffer;             // 预览通道RGB图像缓存
        protected IntPtr m_ImageBufferSnapshot;     // 抓拍通道RGB图像缓存
        protected tSdkCameraCapbility tCameraCapability;  // 相机特性描述
        protected int m_iDisplayedFrames = 0;    //已经显示的总帧数
        protected CAMERA_SNAP_PROC m_CaptureCallback;
        protected IntPtr m_iCaptureCallbackCtx;     //图像回调函数的上下文参数
        protected Thread m_tCaptureThread;          //图像抓取线程
        protected bool m_bExitCaptureThread = false;//采用线程采集时，让线程退出的标志
        protected IntPtr m_iSettingPageMsgCallbackCtx; //相机配置界面消息回调函数的上下文参数   
        protected tSdkFrameHead m_tFrameHead;
        protected bool m_bEraseBk = false;
        protected bool m_bSaveImage = false;
        #endregion

        #region Constructor Region
        public DockCameraSetting()
        {
            InitializeComponent();
        }

        public DockCameraSetting(string text, Image icon) : this()
        {
            DockText = text;
            Icon = icon;
        }

        #endregion

        #region 用户事件
        private bool InitCamera()
        {
            CameraSdkStatus status;
            tSdkCameraDevInfo[] tCameraDevInfoList;
            IntPtr ptr;
            int i;
#if USE_CALL_BACK
            CAMERA_SNAP_PROC pCaptureCallOld = null;
#endif
            if (m_hCamera > 0)
            {
                //已经初始化过，直接返回 true

                return true;
            }

            status = MvApi.CameraEnumerateDevice(out tCameraDevInfoList);
            if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                if (tCameraDevInfoList != null)//此时iCameraCounts返回了实际连接的相机个数。如果大于1，则初始化第一个相机
                {
                    status = MvApi.CameraInit(ref tCameraDevInfoList[0], -1, -1, ref m_hCamera);
                    if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                    {
                        //获得相机特性描述
                        MvApi.CameraGetCapability(m_hCamera, out tCameraCapability);

                        m_ImageBuffer = Marshal.AllocHGlobal(tCameraCapability.sResolutionRange.iWidthMax * tCameraCapability.sResolutionRange.iHeightMax * 3 + 1024);
                        m_ImageBufferSnapshot = Marshal.AllocHGlobal(tCameraCapability.sResolutionRange.iWidthMax * tCameraCapability.sResolutionRange.iHeightMax * 3 + 1024);

                        if (tCameraCapability.sIspCapacity.bMonoSensor != 0)
                        {
                            // 黑白相机输出8位灰度数据
                            MvApi.CameraSetIspOutFormat(m_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                        }

                        //初始化显示模块，使用SDK内部封装好的显示接口
                        MvApi.CameraDisplayInit(m_hCamera, PreviewBox.Handle);
                        MvApi.CameraSetDisplaySize(m_hCamera, PreviewBox.Width, PreviewBox.Height);

                        //设置抓拍通道的分辨率。
                        tSdkImageResolution tResolution;
                        tResolution.uSkipMode = 0;
                        tResolution.uBinAverageMode = 0;
                        tResolution.uBinSumMode = 0;
                        tResolution.uResampleMask = 0;
                        tResolution.iVOffsetFOV = 0;
                        tResolution.iHOffsetFOV = 0;
                        tResolution.iWidthFOV = tCameraCapability.sResolutionRange.iWidthMax;
                        tResolution.iHeightFOV = tCameraCapability.sResolutionRange.iHeightMax;
                        tResolution.iWidth = tResolution.iWidthFOV;
                        tResolution.iHeight = tResolution.iHeightFOV;
                        //tResolution.iIndex = 0xff;表示自定义分辨率,如果tResolution.iWidth和tResolution.iHeight
                        //定义为0，则表示跟随预览通道的分辨率进行抓拍。抓拍通道的分辨率可以动态更改。
                        //本例中将抓拍分辨率固定为最大分辨率。
                        tResolution.iIndex = 0xff;
                        tResolution.acDescription = new byte[32];//描述信息可以不设置
                        tResolution.iWidthZoomHd = 0;
                        tResolution.iHeightZoomHd = 0;
                        tResolution.iWidthZoomSw = 0;
                        tResolution.iHeightZoomSw = 0;

                        MvApi.CameraSetResolutionForSnap(m_hCamera, ref tResolution);

                        //让SDK来根据相机的型号动态创建该相机的配置窗口。
                        MvApi.CameraCreateSettingPage(m_hCamera, this.Handle, tCameraDevInfoList[0].acFriendlyName,/*SettingPageMsgCalBack*/null,/*m_iSettingPageMsgCallbackCtx*/(IntPtr)null, 0);

                        //两种方式来获得预览图像，设置回调函数或者使用定时器或者独立线程的方式，
                        //主动调用CameraGetImageBuffer接口来抓图。
                        //本例中仅演示了两种的方式,注意，两种方式也可以同时使用，但是在回调函数中，
                        //不要使用CameraGetImageBuffer，否则会造成死锁现象。
#if USE_CALL_BACK
                        m_CaptureCallback = new CAMERA_SNAP_PROC(ImageCaptureCallback);
                        MvApi.CameraSetCallbackFunction(m_hCamera, m_CaptureCallback, m_iCaptureCallbackCtx, ref pCaptureCallOld);
#else //如果需要采用多线程，使用下面的方式
                        m_bExitCaptureThread = false;
                        m_tCaptureThread = new Thread(new ThreadStart(CaptureThreadProc));
                        m_tCaptureThread.Start();

#endif
                        //MvApi.CameraReadSN 和 MvApi.CameraWriteSN 用于从相机中读写用户自定义的序列号或者其他数据，32个字节
                        //MvApi.CameraSaveUserData 和 MvApi.CameraLoadUserData用于从相机中读取自定义数据，512个字节
                        return true;

                    }
                    else
                    {
                        m_hCamera = 0;
                        //StateLabel.Text = "Camera init error";
                        String errstr = string.Format("相机初始化错误，错误码{0},错误原因是", status);
                        String errstring = MvApi.CameraGetErrorString(status);
                        // string str1 
                        MessageBox.Show(errstr + errstring, "ERROR");
                        return false;
                    }


                }
            }
            else
            {
                MessageBox.Show("没有找到相机，如果已经接上相机，可能是权限不够，请尝试使用管理员权限运行程序。");
            }

            return false;

        }

#if USE_CALL_BACK
        public void ImageCaptureCallback(CameraHandle hCamera, IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead, IntPtr pContext)
        {
            //图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
            MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
            //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
            MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);
            //调用SDK封装好的接口，显示预览图像
            MvApi.CameraDisplayRGB24(hCamera, m_ImageBuffer, ref pFrameHead);
            m_iDisplayedFrames++;

            if (pFrameHead.iWidth != m_tFrameHead.iWidth || pFrameHead.iHeight != m_tFrameHead.iHeight)
            {
                m_bEraseBk = true;
                m_tFrameHead = pFrameHead;
            }
            
        }
#else
        public void CaptureThreadProc()
        {
            CameraSdkStatus eStatus;
            tSdkFrameHead FrameHead;
            IntPtr uRawBuffer;//rawbuffer由SDK内部申请。应用层不要调用delete之类的释放函数

            while (m_bExitCaptureThread == false)
            {
                //500毫秒超时,图像没捕获到前，线程会被挂起,释放CPU，所以该线程中无需调用sleep
                eStatus = MvApi.CameraGetImageBuffer(m_hCamera, out FrameHead, out uRawBuffer, 500);

                if (eStatus == CameraSdkStatus.CAMERA_STATUS_SUCCESS)//如果是触发模式，则有可能超时
                {
                    //图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
                    MvApi.CameraImageProcess(m_hCamera, uRawBuffer, m_ImageBuffer, ref FrameHead);
                    //叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。    
                    MvApi.CameraImageOverlay(m_hCamera, m_ImageBuffer, ref FrameHead);
                    //调用SDK封装好的接口，显示预览图像
                    MvApi.CameraDisplayRGB24(m_hCamera, m_ImageBuffer, ref FrameHead);
                    //成功调用CameraGetImageBuffer后必须释放，下次才能继续调用CameraGetImageBuffer捕获图像。
                    MvApi.CameraReleaseImageBuffer(m_hCamera, uRawBuffer);

                    if (FrameHead.iWidth != m_tFrameHead.iWidth || FrameHead.iHeight != m_tFrameHead.iHeight)
                    {
                        m_bEraseBk = true;
                        m_tFrameHead = FrameHead;
                    }

                    m_iDisplayedFrames++;

                    if (m_bSaveImage)
                    {
                        MvApi.CameraSaveImage(m_hCamera, "c:\\test.bmp", m_ImageBuffer, ref FrameHead, emSdkFileType.FILE_BMP, 100);
                        m_bSaveImage = false;
                    }
                }

            }

        }
#endif


        #endregion
        #region Event Handler Region

        public override void Close()
        {
            if (m_hCamera > 0)
            {
#if !USE_CALL_BACK //使用回调函数的方式则不需要停止线程
                m_bExitCaptureThread = true;
                while (m_tCaptureThread.IsAlive)
                {
                    Thread.Sleep(10);
                }
#endif
                MvApi.CameraUnInit(m_hCamera);
                Marshal.FreeHGlobal(m_ImageBuffer);
                Marshal.FreeHGlobal(m_ImageBufferSnapshot);
                m_hCamera = 0;
            }

            if (serialPort.isPortOpen) serialPort.ClosePort();
            base.Close();
        }
        #endregion

        private void DockCameraSetting_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (var i in ports)
            {
                DarkListItem darkListItem = new DarkListItem(i);
                dlvw.Items.Add(darkListItem);
            }
            if (ports.Length > 0)
                dlvw.SelectItem(0);
        }

        private void dlvw_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (serialPort.isPortOpen) serialPort.ClosePort();
            serialPort.PortName = dlvw.Items[dlvw.SelectedIndices[0]].Text;
            serialPort.Parity = "None";
            serialPort.StopBits = "One";
            serialPort.DataBits = "8";
            serialPort.BaudRate = "115200";
            serialPort.OpenPort();
            serialPort.CurrentTransmissionType = CommunicationManager.TransmissionType.Hex;
            serialPort.WriteData("5A A5 06 83 10 05 01 00 04 A3");
        }

        private void darkRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.WriteData("5A A5 06 83 10 03 01 00 01 9E");
        }

        private void darkRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.WriteData("5A A5 06 83 10 03 01 00 02 9F");
        }

        private void darkRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.WriteData("5A A5 06 83 10 03 01 00 03 A0");
        }

        private void darkRadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.WriteData("5A A5 06 83 10 03 01 00 04 A1");
        }

        private void darkRadioButton5_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.WriteData("5A A5 06 83 10 03 01 00 05 A2");
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            if (InitCamera() == true)
            {
                MvApi.CameraPlay(m_hCamera);
            }
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            if (m_hCamera > 0)
            {
                MvApi.CameraShowSettingPage(m_hCamera, 1);//1 show ; 0 hide
            }
        }
    }
}
