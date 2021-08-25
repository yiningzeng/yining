using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaferAoi.Tools
{
    public class InterLayerDraw
    {
        //获取的图像变量
        public string color = "red";
        public HObject hImage = null;
        public HObject fullImage = null;
        public List<HDrawingObject> drawingObjects = new List<HDrawingObject>();
        public List<HDrawingObject> drawingObjects2 = new List<HDrawingObject>();

        private HObject drawingCross;
        public HDrawingObject selected_drawing_object = new HDrawingObject();
        public HSmartWindowControl hsmartwindows;
        public double X = 0, Y = 0;
        public InterLayerDraw(HSmartWindowControl hs)
        {
            hsmartwindows = hs;
            HOperatorSet.GenEmptyObj(out hImage);
            HOperatorSet.GenEmptyObj(out fullImage);
        }
        public InterLayerDraw()
        {
            HOperatorSet.GenEmptyObj(out hImage);
            HOperatorSet.GenEmptyObj(out fullImage);
        }
        /// <summary>
        /// 加载外部图像
        /// </summary>
        /// <param name="hSmartWindow"></param>
        /// <returns></returns>

        public void SaveImg(string format, string path)
        {
            HOperatorSet.WriteImage(hImage, format, 0, path);
        }
        public void Dispose()
        {
            try
            {
                foreach (var obj in drawingObjects) obj.Dispose();
                drawingObjects.Clear();
                if (hImage != null) hImage.Dispose();
                if (drawingCross != null) drawingCross.Dispose();
                if (hsmartwindows != null) HOperatorSet.ClearWindow(hsmartwindows.HalconWindow);
            }
            catch { }
        }
        public string DrawRectange1(bool isSec = false)
        {
            try
            {
                HDrawingObject rect1 = HDrawingObject.CreateDrawingObject(
                     HDrawingObject.HDrawingObjectType.RECTANGLE1, Y - 100, X - 100, Y + 100, X + 100);
                rect1.SetDrawingObjectParams("color", color);
                selected_drawing_object = rect1;
                if (!isSec)
                    drawingObjects.Add(rect1);
                else
                    drawingObjects2.Add(rect1);
                hsmartwindows.HalconWindow.AttachDrawingObjectToWindow(rect1);
                return "";
            }
            catch (Exception er)
            {
                return "err";
            }
        }

        public string DrawCross()
        {
            try
            {
                if (hImage != null && hImage.IsInitialized() == true)
                {
                    HOperatorSet.SetColor(hsmartwindows.HalconWindow, color);
                    HOperatorSet.GenCrossContourXld(out HObject hObjectCross, Y, X, 200, 0);
                    HOperatorSet.DispObj(hImage, hsmartwindows.HalconWindow);

                    if (drawingCross != null)
                    {
                        HOperatorSet.DispObj(drawingCross, hsmartwindows.HalconWindow);
                    }

                    HOperatorSet.DispObj(hObjectCross, hsmartwindows.HalconWindow);

                    hObjectCross.Dispose();
                    return "已显示十字架";
                }
                else
                {
                    return "请查看图像是否存在";
                }
            }
            catch (Exception er)
            {
                return "err";
            }
        }

        public bool SaveCross()
        {
            try
            {
                HOperatorSet.GenCrossContourXld(out HObject hObjectCross, Y, X, 200, 0);
                if (drawingCross == null || !drawingCross.IsInitialized())
                {
                    drawingCross = hObjectCross;
                }
                else
                {
                    drawingCross = drawingCross.ConcatObj(hObjectCross);
                }
                HOperatorSet.DispObj(hImage, hsmartwindows.HalconWindow);
                HOperatorSet.DispObj(drawingCross, hsmartwindows.HalconWindow);
                //hObjectCross.Dispose();
                return true;
            }
            catch { return false; }
        }

        public string DrawRectange2()
        {
            try
            {
                if (hImage != null && hImage.IsInitialized() == true)
                {
                    HDrawingObject rect2 = HDrawingObject.CreateDrawingObject(
                    HDrawingObject.HDrawingObjectType.RECTANGLE2, Y, X, 0, 100, 100);
                    rect2.SetDrawingObjectParams("color", color);
                    selected_drawing_object = rect2;
                    drawingObjects.Add(rect2);
                    hsmartwindows.HalconWindow.AttachDrawingObjectToWindow(rect2);
                    return "已添加矩形2工具";
                }
                else
                {
                    return "请查看图像是否存在";
                }
            }
            catch (Exception er)
            {
                return "err";
            }
        }
        public string DrawCircle()
        {
            try
            {
                if (hImage != null && hImage.IsInitialized() == true)
                {
                    HDrawingObject circle = HDrawingObject.CreateDrawingObject(
                    HDrawingObject.HDrawingObjectType.CIRCLE, 200, 200, 70);
                    circle.SetDrawingObjectParams("color", color);
                    selected_drawing_object = circle;
                    drawingObjects.Add(circle);
                    hsmartwindows.HalconWindow.AttachDrawingObjectToWindow(circle);
                    return "已添加圆形工具";
                }
                else
                {
                    return "请查看图像是否存在";
                }
            }
            catch (Exception er)
            {
                return "err";
            }
        }
        public string SaveROI(HDrawingObject selected_drawing_object, string path)
        {
            if (selected_drawing_object.IsInitialized() == true)
            {
                HObject GetDrawRoI = selected_drawing_object.GetDrawingObjectIconic();
                HOperatorSet.WriteRegion(GetDrawRoI, path);
                return string.Format("保存完成 :路径 ={0}", path);
            }
            return "ROI不存在，请先绘制";
        }
        public string SaveROIImage(HDrawingObject hDrawingObject, string path)
        {
            if (hDrawingObject.IsInitialized() == true)
            {
                double Row1 = hDrawingObject.GetDrawingObjectParams("row1");
                double Row2 = hDrawingObject.GetDrawingObjectParams("row2");
                double Col1 = hDrawingObject.GetDrawingObjectParams("column1");
                double Col2 = hDrawingObject.GetDrawingObjectParams("column2");
                HOperatorSet.CropRectangle1(hImage, out HObject cropImg, Row1 - 50, Col1 - 50, Row2 + 50, Col2 + 50);
                HOperatorSet.WriteImage(cropImg, "bmp", 0, path);
                return string.Format("保存完成 :路径 ={0}", path);
            }
            return "ROI不存在，请先绘制";
        }
        public void SetRedColor()
        {
            color = "red";
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams("color", color);
        }
        public void SetYellowColor()
        {
            color = "yellow";
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams("color", color);
        }
        public void SetGreenColor()
        {
            color = "green";
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams("color", color);
        }
        public void SetBlueColor()
        {
            color = "blue";
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams("color", color);
        }
        public void Setlinedashed()
        {
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams(new HTuple("line_style"), new HTuple(20, 5));

        }
        public void Setlinecontinuous()
        {
            if (selected_drawing_object.IsInitialized())
                selected_drawing_object.SetDrawingObjectParams(new HTuple("line_style"), new HTuple());

        }
        public void ClearallTool()
        {
            foreach (HDrawingObject dobj in drawingObjects)
                dobj.Dispose();
            drawingObjects.Clear();
            selected_drawing_object = new HDrawingObject();
        }
        public void ReDrawAllHDrawingObject()
        {
            try
            {
                selected_drawing_object = new HDrawingObject();
                foreach (HDrawingObject obj in drawingObjects)
                {
                    if (obj.IsInitialized())
                        hsmartwindows.HalconWindow.AttachDrawingObjectToWindow(obj);
                }

                foreach (HDrawingObject obj in drawingObjects2)
                {
                    if (obj.IsInitialized())
                        hsmartwindows.HalconWindow.AttachDrawingObjectToWindow(obj);
                }
            }
            catch (HalconException er) { }
        }
        public HObject GetReduceImage()
        {
            if (selected_drawing_object != null || selected_drawing_object.IsInitialized())
            {
                HObject GetDrawRoI = selected_drawing_object.GetDrawingObjectIconic();
                HOperatorSet.ReduceDomain(hImage, GetDrawRoI, out HObject imageaReduce);
                return imageaReduce;
            }
            else
            {
                return null;
            }
        }

        public bool SaveTemplet(string filePath)
        {
            if (selected_drawing_object != null || selected_drawing_object.IsInitialized())
            {
                HObject GetDrawRoI = selected_drawing_object.GetDrawingObjectIconic();
                HOperatorSet.ReduceDomain(hImage, GetDrawRoI, out HObject imageaReduce);

                HOperatorSet.CreateShapeModel(imageaReduce, "auto", (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(),

                  (new HTuple(0.1)).TupleRad(), "auto", "use_polarity", "auto", "auto", out HTuple modelID);
                HOperatorSet.WriteShapeModel(modelID, filePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        public HObject Createmode(HObject himage, HTuple hSmartWindow, HTuple mb_Row, HTuple mb_column, HTuple Rectanglewidth, HTuple Rectanglehigth, HTuple Recinter, int Forrow, int ForColumn)
        {
            HTuple distlen;
            //  HOperatorSet.GetMbutton(hSmartWindow,out HTuple mb_Row,out  HTuple mb_column,out HTuple button);
            HTuple r = mb_Row.D.ToString("0.00");
            HTuple c = mb_column.D.ToString("0.00");
            HOperatorSet.GenCrossContourXld(out HObject hObjectCross, r, c.D, 30, 0);
            HOperatorSet.GenRectangle1(out HObject hObjectRectangle, mb_Row, mb_column, Rectanglewidth, Rectanglehigth);
            HOperatorSet.GenEmptyRegion(out HObject hObjectRectangle1);
            HOperatorSet.GenEmptyRegion(out HObject hObjectRectanglelist);
            distlen = Rectanglewidth + Recinter;
            for (int i = 0; i < Forrow; i++)
            {
                HOperatorSet.MoveRegion(hObjectRectangle, out HObject hObjectmoveRectangle1, 0, distlen);
                HOperatorSet.ConcatObj(hObjectRectangle1, hObjectRectangle, out hObjectRectangle1);
                HOperatorSet.ConcatObj(hObjectRectangle1, hObjectmoveRectangle1, out hObjectRectangle1);
                distlen += Rectanglewidth + Recinter;
            }
            return hObjectRectangle1;
        }
        public void showCross(HObject himage, HTuple hSmartWindow, double x, double y)
        {
            HOperatorSet.ClearWindow(hSmartWindow);
            HOperatorSet.SetColor(hSmartWindow, new HTuple("red"));
            HOperatorSet.GenCrossContourXld(out HObject hObjectCross, x, y, 50, 0);
            HOperatorSet.DispImage(himage, hSmartWindow);
            HOperatorSet.DispObj(hObjectCross, hSmartWindow);
            hObjectCross.Dispose();
        }
    }

}
