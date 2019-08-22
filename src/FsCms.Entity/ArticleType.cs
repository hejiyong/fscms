//using FreeSql.DataAnnotations;
using FreeSql.DataAnnotations;
using FsCms.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FsCms.Entity
{
    public class ArticleType : BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [MaxLength(100)]
        public string TypeName { get; set; }

        /// <summary>
        /// 上级类型名称
        /// </summary>
        public long? UpID { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [MaxLength(200)]
        public string Tag { get; set; }

        public DateTime? UpdateDt { get; set; }

        public string UpdateBy { get; set; }
    }

    /// <summary>
    /// 类型树形结构
    /// </summary>
    public class ArticleTypeTreeNode : TreeNode
    {
        public ArticleTypeTreeNode()
        {

        }


        //构造函数自动转换层级
        public ArticleTypeTreeNode(List<ArticleType> list, ArticleType t)
        {
            this.id = t.Id.ToString();
            this.name = t.TypeName;
            this.pid = t.UpID.ToString();
            this.tag = t.Tag;
            this.createdt = t.CreateDt;
            this.children = (from p in list
                             where p.UpID == t.Id
                             select new ArticleTypeTreeNode(list, p) { }).ToList();
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string tag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdt { get; set; }

        /// <summary>
        /// 子集节点
        /// </summary>
        public List<ArticleTypeTreeNode> children { get; set; }
    }
}
