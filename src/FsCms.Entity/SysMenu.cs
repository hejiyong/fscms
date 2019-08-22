using FsCms.Entity.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FsCms.Entity
{
    [Serializable]
    public class SysMenu : BaseEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required]
        [Display(Name = "菜单名称")]
        [MaxLength(100)]
        public string MenuName { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        [Display(Name = "父菜单ID")]
        public long? ParentID { get; set; }

        /// <summary>
        /// 菜单描述
        /// </summary>
        [MaxLength(250)]
        [Display(Name = "菜单描述")]
        public string Description { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [MaxLength(50)]
        [RegularExpression(@"^\/[A-Za-z]+\/[A-Za-z]+\/[A-Za-z]+$",
 ErrorMessage = "请输入正确的地址格式格式\n示例：/Sys/Dictionary/Index")]
        [Display(Name = "菜单地址")]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单图标地址
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "菜单图标地址")]
        public string IconUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }

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

        /// <summary>
        /// 导航属性 -- 角色菜单
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<SysRoleMenu> SysRoleMenus { get; set; }

        /// <summary>
        /// 导航属性 -- 菜单按钮
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<SysMenuButton> SysMenuButtons { get; set; }
    }

    public class SysMenuView : SysMenu
    {
        public string menubuttons { get; set; }
    }

    /// <summary>
    /// 类型树形结构
    /// </summary>
    public class SysMenuTreeNode : TreeNode
    {
        public SysMenuTreeNode()
        {

        }

        //构造函数自动转换层级
        public SysMenuTreeNode(List<SysMenu> list, SysMenu t)
        {
            this.id = t.Id.ToString();
            this.name = t.MenuName;
            this.pid = t.ParentID.ToString();
            this.Description = t.Description;
            this.MenuUrl = t.MenuUrl;
            this.IconUrl = t.IconUrl;
            this.createdt = t.CreateDt;
            this.children = (from p in list
                             where p.ParentID == t.Id
                             select new SysMenuTreeNode(list, p) { }).ToList();
        }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 子集节点
        /// </summary>
        public List<SysMenuTreeNode> children { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdt { get; set; }

        /// <summary>
        /// 页面按钮
        /// </summary>
        public List<SysMenuButton> buttons { get; set; }

        /// <summary>
        /// 页面按钮json字符串格式
        /// </summary>
        public string jsonButtons { get; set; }

    }
}


