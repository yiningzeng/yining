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
    public partial class DialogFlyPhoto : DarkDialog
    {
        #region 飞拍矫正
        int flyInterval = -1;
        ProgramConfig programConfig;
        HTuple DetectModelId;
        double halfRow, halfCol;
        #endregion
        private bool isFlyMode = false;
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
        public DialogFlyPhoto(ref MVCameraHelper mc)
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
            if (!isFlyMode)
            {
                ilMain.ShowImg(e.ImageHobject);
                return;
            }
            try
            {
                // 这里先保存上一步的模板信息
                tasklst.Add(fac.StartNew(obs =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    // 将object转成数组
                    ImageArgs imgArg = (ImageArgs)obs;

                    sw.Restart();
                    HOperatorSet.Rgb1ToGray(imgArg.ImageHobject, out HObject grayImage);
                    sw.Stop();
                    Debug.WriteLine("灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
                    sw.Restart();
                    HOperatorSet.FindShapeModel(grayImage, DetectModelId, -0.001, 0.002, 0.5, 1, 0.5, "least_squares", 5, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                    sw.Stop();
                    Debug.WriteLine("FindModel-分数:"+ Score1.D + "耗时(ms):" + sw.ElapsedMilliseconds);

                    //read_region(aa, 'D:/QTWaferProgram/test/chipModelRegion.hobj')
                    //smallest_rectangle1(aa, roww1, coll1, roww2, coll2)
                    //halfw:= (coll2 - coll1) / 2
                    //halfh:= (roww2 - roww1) / 2
                    //gen_rectangle1(rect, Row3 - halfh, Column3 - halfw, Row3 + halfh, Column3 + halfw)
                    //reduce_domain(Image2, rect, ImageReduced)

                    //find_ncc_model (GrayImage1, ModelID, -0.39, 0.79, 0.8, 1, 0.5, 'true', 4, Row, Column, Angle, Score)
                    //HOperatorSet.FindNccModel(grayImage, DetectModelId, -0.18, 0.18, 0.8, 1, 0.3, "true", 4, out HTuple Row, out HTuple Column, out HTuple Angle, out HTuple Score);
                    grayImage.Dispose();
                    if (Row.Length > 0 && Col.Length > 0)
                    {
                        
                        sw.Restart();
                        HOperatorSet.GenRectangle1(out HObject region, Row - halfRow, Col - halfCol, Row + halfRow, Col + halfCol);
                        sw.Stop();

                        Debug.WriteLine("GenRectangle1-耗时(ms):" + sw.ElapsedMilliseconds);
                        sw.Restart();
                        HOperatorSet.ReduceDomain(imgArg.ImageHobject, region, out HObject ress);
                        sw.Stop();
                        sw.Restart();
                        Debug.WriteLine("ReduceDomain-耗时(ms):" + sw.ElapsedMilliseconds);
                        HOperatorSet.WriteImage(ress, "jpg", 0, @"D:\芯片HOBJECT_modelres\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                        sw.Restart();
                        Debug.WriteLine("WriteImage-耗时(ms):" + sw.ElapsedMilliseconds);
                        ress.Dispose();
                        //HOperatorSet.ConcatObj(imgArg.ImageHobject, region, out HObject obj);
                        ilMain.ShowImg(imgArg.ImageHobject, region);
                        
                        //如果模板结果中心在图像左边就+，右边就减
                        double offsetX = Col.D - imgArg.Width / 2;
                        double offsetXPulse = Math.Abs(ProgramConfig.GetXPulseByPixel(Convert.ToInt32(offsetX), config.ActualPixelLenght));
                        int tempFlyInterval = -1;
                        if (offsetX > 0)
                        {
                            tempFlyInterval = Convert.ToInt32(flyInterval + offsetXPulse);
                        }
                        else {
                            tempFlyInterval = Convert.ToInt32(flyInterval - offsetXPulse);
                        }
                        MotorsControl.setCompareData_Pso(tempFlyInterval);
                        sw.Stop();
                        Debug.WriteLine(" 我是飞拍矫正----间距:" + tempFlyInterval + "-------已经矫正-耗时(ms):" + sw.ElapsedMilliseconds);
                        //   MotorsControl.setCompareData_Pso(int.Parse(dtebInterval.Text)); // 等差模式
                        //MotorsControl.setCompareData_Pso();
                    }
                    sw = null;
                    //if (imgArg.ImageType == ImageArgs.ImageTypeEn.HOBJECT)
                    //{
                    //    if (imgArg.ImageHobject != null && imgArg.ImageHobject.IsInitialized()) HOperatorSet.WriteImage(imgArg.ImageHobject, "jpg", 0, @"D:\芯片HOBJECT\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                    //}
                    //else
                    //{
                    //    if (imgArg.ImageBitmap != null) imgArg.ImageBitmap.Save(@"D:\芯片BITMAP\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp");
                    //}
                    imgArg.Dispose();
                    //{
                    //    ilStep5Model.ShowImg(val);
                    //}), e);
                    return false;
                }, e));

            }
            catch { }
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

        private void btnSetStart_Click(object sender, EventArgs e)
        {
            Axis ax = config.Axes.Find(v => v.Id == 2);
            int pos = MotorsControl.GetOneEncPos(ax.Id);
            tbStart.Text = pos.ToString();
        }

        private void DarkButtonClick(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            string type = btn.Tag.ToString();
            switch (type)
            {
                case "加载本地配置":
                    string filePath;
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog = new OpenFileDialog();
                    fileDialog.InitialDirectory = @"D:\QTWaferProgram\yining";
                    fileDialog.Title = "请选择配置文件";
                    fileDialog.Filter = "配置文件|*.zyn";
                    fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = Path.GetFullPath(fileDialog.FileName);
                        programConfig = JsonHelper.DeserializeByFile<ProgramConfig>(filePath);
                        fileDialog.Dispose();
                    }
                    else
                    {
                        fileDialog.Dispose();
                    }
                    break;
                case "加载本地模板":
                    fileDialog = new OpenFileDialog();
                    fileDialog.InitialDirectory = @"D:\QTWaferProgram\yining";
                    fileDialog.Title = "请选择模型";
                    fileDialog.Filter = "模型文件|fly.model";
                    fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = Path.GetFullPath(fileDialog.FileName);
                        HOperatorSet.ReadShapeModel(filePath, out DetectModelId);
                        HOperatorSet.ReadRegion(out HObject region, Path.Combine(Path.GetDirectoryName(filePath), "chipModelRegion.hobj"));
                        HOperatorSet.SmallestRectangle1(region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                        halfRow = (row2.D - row1.D) / 2;
                        halfCol = (col2.D - col1.D) / 2;
                        region.Dispose();
                        fileDialog.Dispose();
                    }
                    else
                    {
                        fileDialog.Dispose();
                    }
                    break;
                case "开始单行飞拍测试":
                    if (programConfig == null) {
                        DarkMessageBox.ShowError("请先选择文件加载");
                        return;
                    }
                    isFlyMode = true;
                    mVCameraHelper.ReSetNum();
                    mVCameraHelper.testBitmap = false;
                    //PointInfo pf = MotorsControl.GetXYZEncPos(ax.Id, ay.Id, az.Id);
                    _ = Step5SearchAsync(int.Parse(tbStart.Text) ,int.Parse(tbDistance.Text), int.Parse(tbVel.Text), Convert.ToInt32(programConfig.DieWidth), Convert.ToInt32(programConfig.DieHeight), Convert.ToInt32(programConfig.CutRoadWidth));
                    break;

            }
        }

        /// <summary>
        /// 飞拍
        /// </summary>
        /// <param name="markPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="dieWidth"></param>
        /// <param name="dieHeight"></param>
        /// <param name="cutRoadWidth"></param>
        /// <param name="fov"></param>
        /// <returns></returns>
        private async Task Step5SearchAsync(int startPos, int distance, int vel, int dieWidth, int dieHeight, int cutRoadWidth, int fov = 5000)
        {
            MotorsControl.stopCompare();
            await Task.Run(() =>
            {
                config = FsmHelper.GetConfig();
                flyInterval = dieWidth + cutRoadWidth / 2;
                Axis ax = config.Axes.Find(v => v.Id == 2);
                MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), 100, startPos, true);
                MotorsControl.setCompareMode(ax.Id, ax.Id, new short[] { 1, 1 }, new short[] { 1, 1 }, 2, 1);//(new short[] { 3, 4 }, new short[] { 1, 1 }, 2, 2, 1, 1, 2, 0, 100);
                MotorsControl.setCompareData_Pso(flyInterval); // 等差模式


                //MotorsControl.setCompareData_Pso(flyInterval); // 等差模式
                MotorsControl.startCompare();
                MotorsControl.MoveTrap(ax.Id, ax.TrapPrm.Get(), vel, startPos + distance, true);
                MotorsControl.stopCompare();
                //if (i < flyLines - 1)
                //{
                //    startY = startY - flyInterval;
                //    MotorsControl.MoveTrap(ay.Id, ay.TrapPrm.Get(), 50, startY, true);
                //    //Thread.Sleep(100);
                //}

            });
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
            isFlyMode = false;
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