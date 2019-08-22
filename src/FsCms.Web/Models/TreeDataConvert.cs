using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FsCms.Entity;

namespace FsCms.Web.Models
{
    public class TreeData
    {
        public TreeData() { }

        public TreeData(ArticleType type)
        {
            this.id = type.Id;
            this.text = type.TypeName;
        }

        public TreeData(ArticleType type, List<ArticleType> list)
        {
            this.id = type.Id;
            this.text = type.TypeName;
            this.children = (from l in list where l.UpID == type.Id select new TreeData(l, list)).ToList();
        }

        /// <summary>
        /// 唯一编号
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 类型 =0 表示类型 =1 表示内容
        /// </summary>
        public int datatype { get; set; } = 0;

        /// <summary>
        /// 扩展父节id
        /// </summary>
        public int intextfield { get; set; }

        public List<TreeData> children { get; set; }

        public TreeData AddChildrens(List<TreeData> list, Func<long, List<TreeData>> bind = null)
        {
            if (this.children != null && bind != null)
            {
                this.children.ForEach(f =>
                {
                    f.children = bind(f.id);
                });
            }
            this.children?.AddRange(list);
            return this;
        }
    }
}
