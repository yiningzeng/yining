using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public enum ImageTypeEn
        {
            HOBJECT,
            BITMAP,
        }
        /// <summary>
        /// 这个是晶圆图谱上的点位信息
        /// </summary>
        int _xId, _yId;
        /// <summary>
        /// 图像的宽高
        /// </summary>
        int _w, _h;
        ImageTypeEn _imageType;
        HObject _hObject;
        Bitmap _bitmap;
        int _xPulse, _yPulse, _zPulse; //x, y 对应该图片的脉冲位置
        //事件参数重载
        public ImageArgs(int w, int h, int xPulse, int yPulse, int zPulse, HObject hObject)//当输入内容为字符
        {
            this._w = w;
            this._h = h;
            _xPulse = xPulse;
            _yPulse = yPulse;
            _zPulse = zPulse;
            this._hObject = hObject;
            _imageType = ImageTypeEn.HOBJECT;
        }
        /// <summary>
        /// 用于检测用的
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="xPulse"></param>
        /// <param name="yPulse"></param>
        /// <param name="zPulse"></param>
        /// <param name="hObject"></param>
        public ImageArgs(int w, int h, int xPulse, int yPulse, int zPulse, HObject hObject, int xId, int yId)//当输入内容为字符
        {
            this._w = w;
            this._h = h;
            _xPulse = xPulse;
            _yPulse = yPulse;
            _zPulse = zPulse;
            this._hObject = hObject;
            _imageType = ImageTypeEn.HOBJECT;
            _xId = xId;
            _yId = yId;
        }
        public ImageArgs(int w, int h, int xPulse, int yPulse, int zPulse, Bitmap bitmap)//当输入内容为字符
        {
            this._w = w;
            this._h = h;
            _xPulse = xPulse;
            _yPulse = yPulse;
            _zPulse = zPulse;
            this._bitmap = bitmap;
            _imageType = ImageTypeEn.BITMAP;
        }
        public int XId
        {
            get { return _xId; }
            set { _xId = value; }
        }
        public int YId
        {
            get { return _yId; }
            set { _yId = value; }
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
        /// <summary>
        /// 获取当前图像对应的X坐标
        /// </summary>
        public int XPulse
        {
            get { return _xPulse; }
        }
        /// <summary>
        /// 获取当前图像对应的Y坐标
        /// </summary>
        public int YPulse
        {
            get { return _yPulse; }
        }
        /// <summary>
        /// 获取当前图像对应的z坐标
        /// </summary>
        public int ZPulse
        {
            get { return _zPulse; }
        }
        public ImageTypeEn ImageType
        {
            get { return _imageType; }
        }
        public HObject ImageHobject 
        {
            get { return _hObject; }
            set { _hObject = value; }
        }

        public Bitmap ImageBitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; }
        }

        public void Dispose()
        {
            if (_hObject != null && _hObject.IsInitialized()) _hObject.Dispose();
            if (_bitmap != null) _bitmap.Dispose();
        }
    }
    public class MVCameraHelper
    {

        public int num = 0;
        public bool ThreadPoolEnable = false;
        public bool testBitmap = false;
        private int axisXId = 2, axisYId = 1; // X和Y轴的Id
        protected IntPtr[] m_Grabber = new IntPtr[4];
        protected CameraHandle[] m_hCamera = new CameraHandle[4];
        protected tSdkCameraDevInfo[] m_DevInfo;
        protected pfnCameraGrabberFrameCallback m_FrameCallback;
        private int TriggerMode;
        public event EventHandler<ImageArgs> CameraImageCallBack; //定义一个委托类型_hObject的事件  
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        /// <summary>
        /// 一般情况，0表示连续采集模式；1表示软件触发模式；2表示硬件触发模式。
        /// </summary>
        /// <param name="TriggerMode"></param>
        public MVCameraHelper(int TriggerMode = 2)
        {
            InitCamera(TriggerMode);
        }

        /// <summary>
        /// 移除所有注册事件
        /// </summary>
        public void RemoveAllEvent(string eventName)
        {
            var newType = this.GetType();
            foreach (var item in newType.GetEvents())
            {
                FieldInfo _Field = newType.GetField(item.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (_Field != null && item.Name.Equals(eventName))
                {
                    object _FieldValue = _Field.GetValue(this);
                    if (_FieldValue != null && _FieldValue is Delegate)
                    {
                        Delegate _ObjectDelegate = (Delegate)_FieldValue;
                        Delegate[] invokeList = _ObjectDelegate.GetInvocationList();
                        if (invokeList != null)
                        {
                            foreach (Delegate del in invokeList)
                            {
                                item.RemoveEventHandler(this, del);
                            }
                        }
                    }
                }
            }
        }

        public void ReSetNum()
        {
            num = 0;
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
        }

        private void CameraGrabberFrameCallbackBitmap(
           IntPtr Grabber,
           IntPtr pFrameBuffer,
           ref tSdkFrameHead pFrameHead,
           IntPtr Context)
        {
            // 数据处理回调

            // 由于黑白相机在相机打开后设置了ISP输出灰度图像
            // 因此此处pFrameBuffer=8位灰度数据
            // 否则会和彩色相机一样输出BGR24数据

            // 彩色相机ISP默认会输出BGR24图像
            // pFrameBuffer=BGR24数据

            // 执行一次GC，释放出内存
            GC.Collect();

            // 由于SDK输出的数据默认是从底到顶的，转换为Bitmap需要做一下垂直镜像
            MvApi.CameraFlipFrameBuffer(pFrameBuffer, ref pFrameHead, 1);

            int w = pFrameHead.iWidth;
            int h = pFrameHead.iHeight;
            Boolean gray = (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
            Bitmap Image = new Bitmap(w, h,
                gray ? w : w * 3,
                gray ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb,
                pFrameBuffer);

            // 如果是灰度图要设置调色板
            if (gray)
            {
                //Image.Palette = m_GrayPal;
            }
        }

        private void CameraGrabberFrameCallback1(IntPtr Grabber, IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead, IntPtr Context)
        {
            num++;
            var xyzTemp = MotorsControl.GetXYZEncPos(2, 1, 4);
            if (ThreadPoolEnable)
            {
                try
                {
                    object[] objectArray = new object[6];//这里的2就是改成你要传递几个参数
                    objectArray[0] = pFrameBuffer;
                    objectArray[1] = pFrameHead.iWidth;
                    objectArray[2] = pFrameHead.iHeight;
                    objectArray[3] = pFrameHead.uiMediaType;
                    objectArray[4] = xyzTemp;
                    objectArray[5] = pFrameHead.uBytes;
                    object param = (object)objectArray;

                    tasklst.Add(fac.StartNew(obs =>
                    {
                        // 将object转成数组
                        object[] objArr = (object[])obs;
                        var ptr = (IntPtr)objArr[0];
                        var width = (CameraHandle)objArr[1];
                        var height = (CameraHandle)objArr[2];
                        var uiMediaType = (uint)objArr[3];
                        var xyz = (PointInfo)objArr[4];
                        var size = (uint)objArr[5];
                        HObject Image = null;

                        //IntPtr ptr = Marshal.AllocHGlobal((int)size);
                        //byte[] data = new byte[size];
                        //Marshal.Copy(ptrr, data, 0, data.Length);
                        //Marshal.Copy(data, 0, ptr, data.Length);
                        try
                        {
                            if (uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8)
                            {
                                HOperatorSet.GenImage1(out Image, "byte", width, height, ptr);
                            }
                            else if (uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_BGR8)
                            {
                                HOperatorSet.GenImageInterleaved(out Image,
                                    ptr,
                                    "bgr",
                                    width, height,
                                    -1, "byte",
                                    width, height,
                                    0, 0, -1, 0);
                            }
                            HObject ImageRaw = Image;
                            HOperatorSet.MirrorImage(ImageRaw, out Image, "row");
                            ImageRaw.Dispose();
                            if (CameraImageCallBack != null) CameraImageCallBack(this, new ImageArgs(width, height, xyz.X, xyz.Y, xyz.Z, Image));
                        }
                        catch (Exception er)
                        {
                            LogHelper.WriteLog("相机回调图片转换出错", er);
                        }
                        finally
                        {
                            if (m_Grabber[0] != IntPtr.Zero)
                            {
                                MvApi.CameraClearBuffer(m_hCamera[0]);
                            }
                            //Marshal.FreeHGlobal(ptr);
                            GC.Collect();
                        }
                        return true;
                    }, param));
                }
                catch (Exception er)
                {
                    LogHelper.WriteLog("相机回调添加到线程池时数据处理出错", er);
                }
            }
            else
            {
                int w = pFrameHead.iWidth;
                int h = pFrameHead.iHeight;
                HObject Image = null;
                try
                {
                    if (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8)
                    {
                        HOperatorSet.GenImage1(out Image, "byte", w, h, pFrameBuffer);
                    }
                    else if (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_BGR8)
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
                    if (CameraImageCallBack != null) CameraImageCallBack(this, new ImageArgs(w, h, xyzTemp.X, xyzTemp.Y, xyzTemp.Z, Image));
                }
                catch (HalconException Exc)
                {
                }
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

        public void ShowSettingPage(uint show = 1)
        {
            if (m_Grabber[0] != IntPtr.Zero)
            {
                MvApi.CameraShowSettingPage(m_hCamera[0], show);//1 show ; 0 hide
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
