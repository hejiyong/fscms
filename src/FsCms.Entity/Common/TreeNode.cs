using System;
using System.Collections.Generic;
using System.Text;

namespace FsCms.Entity.Common
{
    public class TreeNode
    {
        public string id { get; set; }

        public string pid { get; set; }

        public string name { get; set; }
        public string title { get; set; }

        public bool disabled { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool ischecked { get; set; } = false;
    }
}
