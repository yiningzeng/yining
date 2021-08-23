using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YiNing.Tools
{
    public class WaferMappingHelper
    {
        public static string[,] MappingAnalysis(string filePath)
        {
            string[,] data = null;
            if (File.Exists(filePath))
            {
                //string all = File.ReadAllText(filePath);
                //string txt = ;

                string[] waferLines = WaferLinesDataClean(File.ReadAllLines(filePath));

                //定义一个晶圆的二维数组
                //data = new int[waferLines.Length, waferLines[0].Length];
                for (int i =0;i< waferLines.Length; i++)
                {
                    AnalysisLine(ref data, i, waferLines[i], waferLines.Length);
                }
            }
            return data;
        }

        /// <summary>
        /// 晶圆行数据清洗
        /// </summary>
        /// <param name="contextLines">所有行数据</param>
        /// <param name="mixLength">每行清除空白字符后最小的长度限制</param>
        /// <returns></returns>
        private static string[] WaferLinesDataClean(string[] contextLines, int mixLength = 30)
        {
            List<string> res = new List<string>();
            foreach (var content in contextLines)
            {
                string temp = new Regex("\\s").Replace(content, ""); // 替换掉空白字符
                if (temp.Length > mixLength)
                {
                    res.Add(temp);
                }
            }
            return res.ToArray();
        }

        /// <summary>
        /// 处理单行数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lineIndex">行数</param>
        /// <param name="content">单行内容字符串</param>
        /// <param name="notChipStr">非芯片的标识</param>
        /// <param name="spilt">分隔符默认是没有分隔符</param>
        /// <param name="unitLength">表示一个芯片的字符长度默认是1</param>
        /// <param name="allWaferLine">表示总的晶圆行数,只有在data是空的时候才会有用</param>
        private static void AnalysisLine(ref string[,] data, int lineIndex, string content, int allWaferLine = 0, string notChipStr = "0", char spilt = '\0', int unitLength = 1)
        {
            // 如果是空的话，那么需要首先定义好
            if (data == null)
            {
                // 单行的芯片个数, 如果没有分隔符的话，那么就按照unitLength来截取, content.Length / unitLength 这个一定是可以整除的
                int rowChipNum = (spilt == '\0') ? content.Length / unitLength : content.Split(spilt).Length;
                data = new string[allWaferLine, rowChipNum];
            }
            if (spilt != '\0')
            {
                string[] cols = content.Split(spilt);
                for(int i=0; i< cols.Length;i++)
                {
                    data[lineIndex, i] = cols[i];
                }
            }
            else
            {
                int i = 0;
                while (content.Length >= unitLength)
                {
                    data[lineIndex, i] = content.Substring(0, unitLength);
                    content = content.Substring(unitLength);
                    i++;
                }
            }
        }
    }
}
