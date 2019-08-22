using FsCms.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsCms.Entity
{
    public class SysDictionary : BaseEntity
    {
        [Required]
        [MaxLength(300)]
        [Display(Name = "字典名称")]
        public string DictName { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "字典编号")]
        public string DictNo { get; set; }

        [Display(Name = "父节点ID")]
        public long? ParentID { get; set; }

        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [MaxLength(250)]
        [Display(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "操作人")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Display(Name = "操作时间")]
        public DateTime? UpdateDt { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual SysDictionary Parent { get; set; }
    }

    /// <summary>
    /// 类型树形结构
    /// </summary>
    public class SysDictionaryTreeNode : TreeNode
    {
        public SysDictionaryTreeNode()
        {

        }


        //构造函数自动转换层级
        public SysDictionaryTreeNode(List<SysDictionary> list, SysDictionary t)
        {
            this.id = t.Id.ToString();
            this.name = t.DictName;
            this.pid = t.ParentID.ToString();
            this.Description = t.Description;
            this.DictNo = t.DictNo;
            this.createdt = t.CreateDt;
            this.children = (from p in list
                             where p.ParentID == t.Id
                             select new SysDictionaryTreeNode(list, p) { }).ToList();
        }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string DictNo { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 子集节点
        /// </summary>
        public List<SysDictionaryTreeNode> children { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdt { get; set; }

    }

}


