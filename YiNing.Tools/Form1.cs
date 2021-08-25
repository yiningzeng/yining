using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YiNing.WafermapDisplay.WafermapControl;

namespace YiNing.Tools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            #region tt
            ////测试一段带注释的字符串

            //String multiLineComments = "\np** \n" +

            //"* this is \n" +

            //"* //multi line comment \n" +

            //"*/ \n 这几个字将会在控制台输出 /** \n" +

            //"* this is \n" +

            //"* multi line comment \n" +

            //"*/";

            ////指定正则表达式的规则，如果你怕打错，

            ////就请复制这一段@"(?<!/)/\*([^*/]|\*(?!/)|/(?<!\*))*((?=\*/))(\*/)"

            //Regex r = new Regex(@"(?<!/)p((?=\*/))(\*/)");

            ////这里我们使用正则表达是的替换函数,

            ////我们将正则表达式匹配出来的字符串替换为空白字符,

            ////如果想要替换成其它字符串，可以自己手动设下。

            ////这里不做太多解释。

            //multiLineComments = r.Replace(multiLineComments, "");

            ////以上工作做完后直接向控制台打印输出就能看见你想要的东西咯。

            //Console.WriteLine(multiLineComments);

            #endregion

            // Create sample dataset
            Die[,] data = new Die[41, 40];
            Random binGenerator = new Random(1);
            for (int x = 0; x < 41; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    data[x, y] = new Die() { ColorIndex = binGenerator.Next(8), XIndex = x, YIndex = y, XPluse = 0, YPluse = 0 };// 
                }
            }
            int a = data.Length;
            waferMap.Dataset = data;
            waferMap.Notchlocation = 0;
            //wmap.MinimumSize = new Size(500, 500);
            waferMap.Interactive = true;
            waferMap.SelectX = 21;
            waferMap.SelectY = 14;
            waferMap.SelectOneDie = data[21, 14];

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //LogHelper.WriteLog("asdsadasdsa");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WaferMappingHelper.MappingAnalysis(@"D:\WaferDataIn\mapping\LP1Wafer01.txt");
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            //waferMap.Test();
        }
    }
}
