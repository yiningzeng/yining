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
    public partial class DialogTest : DarkDialog
    {
        #region 飞拍矫正
        int flyInterval = -1;
        HTuple DetectModelId;
        double halfRow, halfCol;
        #endregion
        HTuple trainModelId;
        HObject detectRect;

        private Point leftTopPointPixel, rightBottomPointPixel;
        private ProgramConfig programConfig;
        List<Task<bool>> tasklst = new List<Task<bool>>();
        TaskFactory fac = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(3));
        HDevProgramHelper hDevProgramHelper;
        InterLayerDraw ilMain;
        public DialogTest()
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
        public DialogTest(ProgramConfig pc) : this()
        {
            programConfig = pc;
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

        /// <summary>
        /// 加载本地模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = @"D:\QTWaferProgram";
            fileDialog.Title = "请选择模型";
            fileDialog.Filter = "模型文件|fly.model";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = Path.GetFullPath(fileDialog.FileName);
                HOperatorSet.ReadShapeModel(filePath, out DetectModelId);
                HOperatorSet.ReadRegion(out HObject region, Path.Combine(Path.GetDirectoryName(filePath), "chipModelRegion.hobj"));
                HOperatorSet.SmallestRectangle1(region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                halfRow = (row2.D - row1.D) / 2;
                halfCol = (col2.D - col1.D) / 2;
                region.Dispose();
                fileDialog.Dispose();
                if (programConfig==null) programConfig = JsonHelper.DeserializeByFile<ProgramConfig>(Path.Combine(Path.GetDirectoryName(filePath), "config.zyn"));
                propertyGrid1.SelectedObject = programConfig.ChipModelPar;
                HOperatorSet.ReadVariationModel(programConfig.GetChipVariationModelFileName(), out trainModelId);
                HOperatorSet.GenRectangle1(out detectRect, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
            }
            else
            {
                fileDialog.Dispose();
            }
        }

        /// <summary>
        /// 准备模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            HOperatorSet.WriteVariationModel(trainModelId, programConfig.GetChipVariationModelFileName());
            HOperatorSet.PrepareVariationModel(trainModelId, 10, 2);
        }
        /// <summary>
        /// 测试一张图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string filePath;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择图片";
            fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = Path.GetFullPath(fileDialog.FileName);

                HOperatorSet.ReadImage(out HObject img, filePath);

                Stopwatch sw = new Stopwatch();
                sw.Restart();
                HOperatorSet.Rgb1ToGray(img, out HObject grayImage);
                Debug.WriteLine("灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
                sw.Restart();
                HOperatorSet.FindShapeModel(grayImage, DetectModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent, programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                Debug.WriteLine("FindShapeModel-耗时(ms):" + sw.ElapsedMilliseconds);
                if (Score1.Length > 0)
                {
                    darkListView1.Items.Add(new DarkListItem("耗时(ms):" + sw.ElapsedMilliseconds + " 分数:" + Score1.D));
                    sw.Restart();
                    ImageTrans(grayImage, out HObject fu, Row, Col, Angle1, programConfig.ChipModelPar.ShapeModelCenterRow, programConfig.ChipModelPar.ShapeModelCenterCol, 0, out HTuple aaa);
                    Debug.WriteLine("ImageTrans-耗时(ms):" + sw.ElapsedMilliseconds);
                    HOperatorSet.CropRectangle1(fu, out HObject trainImg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);


                    //HOperatorSet.WriteImage(trainImg, "jpg", 0, "D:/fu.jpg");
                    try
                    {//compare_variation_model
                        sw.Restart();
                        HOperatorSet.CompareVariationModel(trainImg,out HObject region, trainModelId);
                        Debug.WriteLine("CompareVariationModel-耗时(ms):" + sw.ElapsedMilliseconds);
                        sw.Restart();
                        HOperatorSet.AffineTransImage(img, out HObject finalimg, aaa, "constant", "false");
                        Debug.WriteLine("AffineTransImage-耗时(ms):" + sw.ElapsedMilliseconds);
                        sw.Restart();
                        HOperatorSet.CropRectangle1(finalimg, out HObject finalCropimg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);
                        Debug.WriteLine("CropRectangle1-耗时(ms):" + sw.ElapsedMilliseconds);
                        ilMain.ShowImg(finalCropimg, region);
                    }
                    catch (HalconException er) { ilMain.ShowImg(trainImg); }


                    fu.Dispose();
                    trainImg.Dispose();

                    grayImage.Dispose();
                    img.Dispose();
                    fileDialog.Dispose();
                }
            }
            else
            {
                fileDialog.Dispose();
            }
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            //DialogTestModel dialogTestModel = new DialogTestModel(programConfig);
            //dialogTestModel.ShowDialog();
            if (!File.Exists(programConfig.GetChipModelRegionFileName()) || !File.Exists(programConfig.GetChipModelFileName()))
            {
                DarkMessageBox.ShowError("模型相关文件不存在，请重新保存");
                return;
            }
            string filePath;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择图片";
            fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach(var f in fileDialog.FileNames)
                {
                    filePath = Path.GetFullPath(f);

                    HOperatorSet.ReadImage(out HObject img, filePath);

                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    HOperatorSet.Rgb1ToGray(img, out HObject grayImage);
                    Debug.WriteLine("灰度图-耗时(ms):" + sw.ElapsedMilliseconds);
                    sw.Restart();
                    HOperatorSet.FindShapeModel(grayImage, DetectModelId, programConfig.ChipModelPar.AngleStart, programConfig.ChipModelPar.AngleExtent, programConfig.ChipModelPar.MinScore, programConfig.ChipModelPar.NumMatches, 0.5, "least_squares", programConfig.ChipModelPar.NumLevels, 0.9, out HTuple Row, out HTuple Col, out HTuple Angle1, out HTuple Score1);
                    if (Score1.Length > 0)
                    {
                        darkListView1.Items.Add(new DarkListItem("耗时(ms):" + sw.ElapsedMilliseconds + " 分数:" + Score1.D));

                        ImageTrans(grayImage, out HObject fu, Row, Col, Angle1, programConfig.ChipModelPar.ShapeModelCenterRow, programConfig.ChipModelPar.ShapeModelCenterCol, 0, out HTuple aaa);

                        HOperatorSet.CropRectangle1(fu, out HObject trainImg, programConfig.ChipModelPar.DetectRow1, programConfig.ChipModelPar.DetectCol1, programConfig.ChipModelPar.DetectRow2, programConfig.ChipModelPar.DetectCol2);


                        HOperatorSet.WriteImage(trainImg, "jpg", 0, "D:/fu.jpg");
                        try
                        {
                            HOperatorSet.TrainVariationModel(trainImg, trainModelId);
                        }
                        catch (HalconException er) { DarkMessageBox.ShowError("训练失败"); }


                        ilMain.ShowImg(trainImg);


                    }


                    //read_region(aa, 'D:/QTWaferProgram/test/chipModelRegion.hobj')
                    //smallest_rectangle1(aa, roww1, coll1, roww2, coll2)
                    //halfw:= (coll2 - coll1) / 2
                    //halfh:= (roww2 - roww1) / 2
                    //gen_rectangle1(rect, Row3 - halfh, Column3 - halfw, Row3 + halfh, Column3 + halfw)
                    //reduce_domain(Image2, rect, ImageReduced)

                    //find_ncc_model (GrayImage1, ModelID, -0.39, 0.79, 0.8, 1, 0.5, 'true', 4, Row, Column, Angle, Score)
                    //HOperatorSet.FindNccModel(grayImage, DetectModelId, -0.18, 0.18, 0.8, 1, 0.3, "true", 4, out HTuple Row, out HTuple Column, out HTuple Angle, out HTuple Score);
                    grayImage.Dispose();
                    img.Dispose();
                    fileDialog.Dispose();
                }
 
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

}