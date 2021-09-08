using HalconDotNet;
using System;
using System.Collections.Generic;
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
    public partial class DialogTestModel : DarkDialog
    {
        private ProgramConfig programConfig;
        HObject objMain;
        InterLayerDraw ilMain;
        public DialogTestModel()
        {
            InitializeComponent();
            Ini();
        }

        public DialogTestModel(ProgramConfig pc) : this()
        {
            programConfig = pc;
        }

        public void Ini()
        {
            ilMain = new InterLayerDraw(hswcMain);
            this.btnClose.Text = "关闭";
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
            //ilMain.ShowImg(objMain);
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
            //new HDevelopExport();

            string filePath;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择图片";
            fileDialog.Filter = "BMP图像|*.bmp|PNG图像|*.png|JPG图像|*.jpg|所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = Path.GetFullPath(fileDialog.FileName);
                HOperatorSet.ReadImage(out objMain, filePath);

                DeTest(objMain);
                fileDialog.Dispose();
            }
            else
            {
                fileDialog.Dispose();
            }
        }

        private void DeTest(HObject img)
        {
            HOperatorSet.Rgb1ToGray(img, out HObject grayImage);
            HOperatorSet.ReadShapeModel(programConfig.GetChipModelFileName(), out HTuple modelId);
            HOperatorSet.ReadRegion(out HObject region, programConfig.GetChipModelRegionFileName());
            HOperatorSet.AreaCenter(region, out HTuple area, out HTuple regionCenterRow, out HTuple regionCenterCol);

            //find_shape_model(GrayImage1, ModelID1, -0.39, 0.79, 0.5, 1, 0.5, 'least_squares', 4, 0.9, Row3, Column3, Angle1, Score1)
            HOperatorSet.FindShapeModel(grayImage, modelId, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 4, 0.9, out HTuple Row3, out HTuple Column3, out HTuple Angle1, out HTuple Score1);
            if (HalconAPI.isWindows)
                HOperatorSet.SetSystem("use_window_thread", "true");
            dev_display_shape_matching_results(modelId, "red", Row3, Column3, Angle1, 1, 1, 0);

            //ImageTrans(grayImage, out HObject final, Row3, Column3, Angle1, regionCenterRow, regionCenterCol, 0, out HTuple hhasdhas);

            //this.BeginInvoke(new Action<HObject>((ho) =>
            //{
            //    ilMain.ShowImg(ho);
            //}), final);
        }

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
