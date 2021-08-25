using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using MVSDK;//使用SDK接口
using YiNing.Tools;
using static WaferAoi.Tools.Utils;
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;

namespace WaferAoi.Tools
{
    public class ImageArgs : EventArgs
    {
        /// <summary>
        /// 图像的宽高
        /// </summary>
        int _w, _h;
        HObject _hObject;
        //事件参数重载
        public ImageArgs(int w, int h, HObject hObject)//当输入内容为字符
        {
            this._w = w;
            this._h = h;
            this._hObject = hObject;
        }
        //事件属性
        public int Width
        {
            get { return _w; }
        }
        public int Height
        {
            get { return _h; }
        }
        public HObject ImageHobject
        {
            get { return _hObject; }
        }
    }
    public class MVCameraHelper
    {
        protected IntPtr[] m_Grabber = new IntPtr[4];
        protected CameraHandle[] m_hCamera = new CameraHandle[4];
        protected tSdkCameraDevInfo[] m_DevInfo;
        protected pfnCameraGrabberFrameCallback m_FrameCallback;
        private int TriggerMode;
        public event EventHandler<ImageArgs> CameraImageCallBack; //定义一个委托类型的事件  
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));

        /// <summary>
        /// 一般情况，0表示连续采集模式；1表示软件触发模式；2表示硬件触发模式。
        /// </summary>
        /// <param name="TriggerMode"></param>
        public MVCameraHelper(int TriggerMode = 2)
        {
            InitCamera(TriggerMode);
        }
        /// <summary>
        ///初始化相机
        /// </summary>
        /// <param name="iModeSel">一般情况，0表示连续采集模式；1表示软件触发模式；2表示硬件触发模式。</param>
        private void InitCamera(int iModeSel)
        {
            this.TriggerMode = iModeSel;
            m_FrameCallback = new pfnCameraGrabberFrameCallback(CameraGrabberFrameCallback1);
            OpenCameras();
            //m_SaveImageComplete = new pfnCameraGrabberSaveImageComplete(CameraGrabberSaveImageComplete);


            //for (int i = 0; i < NumDev; ++i)
            //{
            //    if (m_Grabber[i] != IntPtr.Zero)
            //        MvApi.CameraGrabber_StartLive(m_Grabber[i]);
            //}
        }

        private void CameraGrabberFrameCallback1(IntPtr Grabber, IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead, IntPtr Context)
        {
            // 数据处理回调

            // 由于黑白相机在相机打开后设置了ISP输出灰度图像
            // 因此此处pFrameBuffer=8位灰度数据
            // 否则会和彩色相机一样输出BGR24数据

            // 彩色相机ISP默认会输出BGR24图像
            // pFrameBuffer=BGR24数据
            //int w = ;
            //int h = ;
            try
            {
                object[] objectArray = new object[4];//这里的2就是改成你要传递几个参数
                objectArray[0] = pFrameBuffer;
                objectArray[1] = pFrameHead.iWidth;
                objectArray[2] = pFrameHead.iHeight;
                objectArray[3] = pFrameHead.uiMediaType;
                object param = (object)objectArray;

                tasklst.Add(fac.StartNew(obs => {
                    // 将object转成数组
                    object[] objArr = (object[])obs;
                    var pFB = (IntPtr)objArr[0];
                    var w = (CameraHandle)objArr[1];
                    var h = (CameraHandle)objArr[2];
                    var uiMediaType = (uint)objArr[3];
                    HObject Image;
                    HOperatorSet.GenEmptyObj(out Image);
                    try
                    {
                        if (uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8)
                        {
                            HOperatorSet.GenImage1(out Image, "byte", w, h, pFrameBuffer);
                        }
                        else if (uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_BGR8)
                        {
                            HOperatorSet.GenImageInterleaved(out Image,
                                pFrameBuffer,
                                "bgr",
                                w, h,
                                -1, "byte",
                                w, h,
                                0, 0, -1, 0);
                        }
                        else
                        {
                            //throw new HalconException("Image format is not supported!!");
                        }

                        HObject ImageRaw = Image;
                        HOperatorSet.MirrorImage(ImageRaw, out Image, "row");
                        ImageRaw.Dispose();
                        //HOperatorSet.WriteImage(Image, "jpg", 0, "aaaaa.jpg");
                        if (CameraImageCallBack != null) CameraImageCallBack(this, new ImageArgs(w, h, Image));
                    }
                    catch (Exception er)
                    {
                        LogHelper.WriteLog("相机回调图片转换出错", er);
                    }
                    return false;
                }, param));
            }
            catch(Exception er) {
                LogHelper.WriteLog("相机回调添加到线程池时数据处理出错", er);
            }
        }


        public void OpenCameras()
        {
            try
            {
                MvApi.CameraEnumerateDevice(out m_DevInfo);
                int NumDev = (m_DevInfo != null ? Math.Min(m_DevInfo.Length, 4) : 0);
                for (int i = 0; i < NumDev; ++i)
                {
                    if (MvApi.CameraGrabber_Create(out m_Grabber[i], ref m_DevInfo[i]) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                    {
                        MvApi.CameraGrabber_GetCameraHandle(m_Grabber[i], out m_hCamera[i]);
                        MvApi.CameraCreateSettingPage(m_hCamera[i], IntPtr.Zero, m_DevInfo[i].acFriendlyName, null, (IntPtr)0, 0);

                        //if (Encoding.UTF8.GetString(m_DevInfo[i].acSn).TrimEnd('\0').StartsWith(RAW_MATERIAL_CAMERA))
                        //{
                        //    MvApi.CameraGrabber_SetRGBCallback(m_Grabber[i], m_FrameCallback1, IntPtr.Zero);
                        //}
                        //else if (Encoding.UTF8.GetString(m_DevInfo[i].acSn).TrimEnd('\0').StartsWith(BAD_MATERIAL_CAMERA))
                        //{
                        //    MvApi.CameraGrabber_SetRGBCallback(m_Grabber[i], m_FrameCallback2, IntPtr.Zero);
                        //}

                        MvApi.CameraGrabber_SetRGBCallback(m_Grabber[i], m_FrameCallback, IntPtr.Zero);

                        // 黑白相机设置ISP输出灰度图像
                        // 彩色相机ISP默认会输出BGR24图像
                        tSdkCameraCapbility cap;
                        MvApi.CameraGetCapability(m_hCamera[i], out cap);
                        if (cap.sIspCapacity.bMonoSensor != 0)
                        {
                            MvApi.CameraSetIspOutFormat(m_hCamera[i], (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                        }

                        // 设置触发模式
                        MvApi.CameraSetTriggerMode(m_hCamera[i], TriggerMode);
                        MvApi.CameraSetTriggerCount(m_hCamera[i], 1);

                        MvApi.CameraGrabber_StartLive(m_Grabber[i]);
                    }
                }
            }
            catch { }
        }

        public void CloseCameras()
        {
            try
            {
                for (int i = 0; i < m_Grabber.Length; i++)
                {
                    if (m_Grabber[i] != IntPtr.Zero)
                    {
                        MvApi.CameraGrabber_Destroy(m_Grabber[i]);
                    }
                }
            }
            catch (Exception er)
            {
                LogHelper.WriteLog("关闭相机出错", er);
            }
        }

        public void CameraSetExposureTime(double exposureTime)
        {
            if (m_Grabber[0] != IntPtr.Zero)
            {
                //MvApi.CameraShowSettingPage(m_hCamera[i], 1);//1 show ; 0 hide
                MvApi.CameraSetAeState(m_hCamera[0], 0);//设置为手动曝光模式
                MvApi.CameraSetExposureTime(m_hCamera[0], exposureTime);
            }
        }
    }
}
