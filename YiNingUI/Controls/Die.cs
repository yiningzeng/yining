using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YiNing.UI.Controls
{
    public class Die
    {
        /// <summary>
        /// 是否有用的Die
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// 颜色Index
        /// </summary>
        public int ColorIndex { get; set; }

        /// <summary>
        /// X序号
        /// </summary>
        public int XIndex { get; set; }
        /// <summary>
        /// Y序号
        /// </summary>
        public int YIndex { get; set; }
        /// <summary>
        /// X点的脉冲位置
        /// </summary>
        public int XPluse { get; set; }
        /// <summary>
        /// Y点的脉冲位置
        /// </summary>
        public int YPluse { get; set; }

        /// <summary>
        /// 检测结果图和缺陷
        /// </summary>
        public string Res { get; set; }
        /// <summary>
        /// 当鼠标点击事件触发时，鼠标的参数
        /// </summary>
        public MouseEventArgs MouseArgs { get; set; }

        public string GetImagePath()
        {
            return Res + ".jpg";
        }
        public string GetHobjPath()
        {
            return Res + ".hobj";
        }
    }
}
