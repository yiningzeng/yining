using YiNing.UI.Collections;
using System;
using System.Drawing;

namespace YiNing.UI.Controls
{
    public class DarkWaferListItemInfo
    {
        /// <summary>
        /// 图谱文件的路径
        /// </summary>
        public string FullFileName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// 文件名前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 文件名后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

    }
}
