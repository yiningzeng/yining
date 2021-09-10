using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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
    public partial class DialogCalPixel : DarkDialog
    {
        private MVCameraHelper mVCameraHelper;
        Config config = FsmHelper.GetConfig();
        private Axis ax, ay, az, ar;// = FsmHelper.GetConfig().Axes.Find(v => v.Remarks == "载盘X轴");

        #region 用于计算像元， 模型的属性
        HObject baseImage;
        HObject region;
        int baseX, baseY;
        #endregion

        private List<PointF> pointFs = new List<PointF>();

        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(3));
        HDevProgramHelper hDevProgramHelper;
        HObject hObject;
        InterLayerDraw ilMain;
        public DialogCalPixel(ref MVCameraHelper mc)
        {
            InitializeComponent();
            Ini();
            mVCameraHelper = mc;
            mVCameraHelper.ThreadPoolEnable = false;
            mVCameraHelper.CameraImageCallBack += MVCameraHelper_CameraImageCallBack;
            hDevProgramHelper = new HDevProgramHelper("求像元大小v2.1.hdev");
        }

        private void MVCameraHelper_CameraImageCallBack(object sender, ImageArgs e)
        {
            try
            {
                ilMain.ShowImg(e);
            }
            catch (Exception er)
            {

            }
        }

        public void Ini()
        {
            UpdateGetAxes();
            HOperatorSet.GenEmptyObj(out baseImage);
            HOperatorSet.GenEmptyObj(out region);
            ilMain = new InterLayerDraw(hswcMain);
            this.btnOk.Text = "保存";
            this.btnCancel.Text = "取消";
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.Load += DialogCreateModel_Load;
            this.FormClosed += DialogCreateModel_FormClosed;
            hswcMain.MouseWheel += hswcMain.HSmartWindowControl_MouseWheel;
            hswcMain.HMouseMove += HswcMain_HMouseMove;
        }

        private void DialogCreateModel_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ilMain.Dispose();
                mVCameraHelper.CameraImageCallBack -= MVCameraHelper_CameraImageCallBack;
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
      
        private void DialogCreateModel_Load(object sender, EventArgs e)
        {
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

        /// <summary>
        /// 保存模板操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            //if (save)
            //{
            //    HOperatorSet.WriteRegion(region, );
            //    HOperatorSet.WriteShapeModel(ModelId, programConfig.GetChipModelFileName());
            //}
            //grayImage.Dispose();
            //imageReduced.Dispose();
            //region.Dispose();
            //ilMain.GetRoiRegion(ilMain.drawingObjects2[0], out double Row1, out double Col1, out double Row2, out double Col2);
            //HOperatorSet.GenEmptyObj(out HObject cropImg);
            //HOperatorSet.CropRectangle1(ilMain.hImage, out cropImg, Row1, Col1, Row2, Col2);
            //programConfig.DetectRegion = new Region(Row1, Col1, Row2, Col2);
            //JsonHelper.Serialize(programConfig, programConfig.GetThisFileName());
            //HDevProgramHelper.CreatVariationModele(cropImg, out HTuple vMId);
            //HDevProgramHelper.SaveVariationModel(programConfig.GetChipVariationModelFileName(), vMId);
            //ilMain.ClearallTool();
            //ilMain.ClearallTool2();
            //ilMain.ShowImg(cropImg);
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
                      string filePath;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择图片";
            fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = Path.GetFullPath(fileDialog.FileName);
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

        private void 画一个矩形区域ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ilMain.ClearallTool();
            ilMain.DrawRectange1();

        }

        private void 保存模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ilMain.hImage == null || !ilMain.hImage.IsInitialized()) { DarkMessageBox.ShowError("请重新移动再制作模板"); return; }
            region.Dispose();
            baseImage.Dispose();

            PointInfo pointInfo = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
            baseX = pointInfo.X;
            baseY = pointInfo.Y;
            region = ilMain.selected_drawing_object.GetDrawingObjectIconic().Clone();
            baseImage = ilMain.hImage.Clone();
            ilMain.ClearallTool();
        }

        private void DarkButtonClick(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "计算一组":
                    PointInfo nowx = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                    int dX = Math.Abs(nowx.X - baseX);
                    int dY = Math.Abs(nowx.Y - baseY);
                    hDevProgramHelper.CalPixel(baseImage, region, ilMain.hImage, dX, dY, out HTuple pixelX, out HTuple pixelY);
                    if (pixelX.Length != 0 && pixelY.Length != 0)
                    {
                        
                        pointFs.Add(new PointF(Convert.ToSingle(pixelX.D), Convert.ToSingle(pixelY.D)));
                        darkListView1.Items.Add(new DarkListItem("X:" + pixelX.D + " Y:" + pixelY.D));
                        tbFinalX.Text = pixelX.D.ToString();
                        tbFinalY.Text = pixelY.D.ToString();
                    }
       
                    break;
                case "去掉最低最高求平均":
                    float sumX = 0, sumY = 0;
                    pointFs.Sort((x, y) => y.X.CompareTo(x.X));
                    for (int i = 3 ; i < pointFs.Count - 3; i++)
                    {
                        sumX += Math.Abs(pointFs[i].X);
                    }
                    pointFs.Sort((x, y) => y.Y.CompareTo(x.Y));
                    for (int i = 3; i < pointFs.Count - 3; i++)
                    {
                        sumY += Math.Abs(pointFs[i].Y);
                    }
                    float finalX = sumX / (pointFs.Count - 6);
                    float finalY = sumY / (pointFs.Count - 6);
                    tbFinalX.Text = finalX.ToString();
                    tbFinalY.Text = finalY.ToString();
                    tbAvg.Text = ((finalX + finalY) / 2).ToString();
                    break;
                case "清空":
                    pointFs.Clear();
                    darkListView1.Items.Clear();
                    break;
                case "保存最终结果":
                    config.PixelLenght = float.Parse(tbAvg.Text);
                    JsonHelper.Serialize(config, config.GetThisFileName());
                    break;
            }
        }


        #region 运动控制

        private void UpdateGetAxes()
        {
            config = FsmHelper.GetConfig();
            ax = config.Axes.Find(v => v.Remarks == "载盘X轴");
            ay = config.Axes.Find(v => v.Remarks == "载盘Y轴");
            az = config.Axes.Find(v => v.Remarks == "载盘Z轴");
            ar = config.Axes.Find(v => v.Remarks == "载盘旋转轴");
        }

        /// <summary>
        /// 运动控制停止运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            MotorsControl.stopCompare();
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1; break;//晶圆载具Y轴状态
                case "向右": axisId = 2; direction = -1; break;//晶圆载具Y轴状态
                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
                MotorsControl.StopAxis(axis.Id, 0);

            //最后再拍一张防止后面又有一点小走动
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            MotorsControl.startCompare();
            MotorsControl.stopCompare();
        }
        /// <summary>
        /// 运动控制，开始运动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            MotorsControl.setCompareMode(ax.Id, ay.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 2, 20);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
            MotorsControl.setCompareData_Pso(20); // 等差模式 
            MotorsControl.startCompare();
            Debug.WriteLine("开始运动");
            Button btn = sender as Button;
            Axis axis;
            int axisId = 0;
            int direction = 1;
            int runDirection = -1; // 表示相机方向
            switch (btn.Tag.ToString())
            {
                case "向上": axisId = 1; direction = -1 * runDirection; break; // 晶圆载具X轴状态
                case "向下": axisId = 1; direction = 1 * runDirection; break;// 晶圆载具X轴状态
                case "向左": axisId = 2; direction = 1 * runDirection; break;//"晶圆载具Y轴状态";
                case "向右": axisId = 2; direction = -1 * runDirection; break;//晶圆载具Y轴状态";

                case "顺时针转": axisId = 3; direction = 1; break;//晶圆载具旋转轴状态
                case "逆时针转": axisId = 3; direction = -1; break;//晶圆载具旋转轴状态               
                case "相机高度向上": axisId = 4; direction = -1; break;//相机拍照Z轴状态               
                case "相机高度向下": axisId = 4; direction = 1; break;//相机拍照Z轴状态
            }
            axis = config.Axes.Find(a => a.Id == axisId);
            if (axis != null)
                MotorsControl.MoveJog(axis.Id, axis.JogPrm.Get(), Convert.ToDouble(dbupVel.Value), direction);
        }
        #endregion


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
}