using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FsCms.Entity
{
    public class SysRole : BaseEntity
    {
        [Display(Name = "角色编码")]
        [MaxLength(50)] 
        public string RoleCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }

        [MaxLength(500)]
        [Display(Name = "描述")]
        public string Description { get; set; }

        [MaxLength(50)]
        [Display(Name = "修改人")]
        public string UpdateBy { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? UpdateDt { get; set; }

        [Display(Name = "来源类型 =1 商家添加 否则为商家添加")]
        public int OriginType { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual List<SysRoleMenu> SysRoleMenu { get; set; }

    }

    public class SysRoleView : SysRole
    {
        /// <summary>
        /// 授权菜单权限id
        /// </summary>
        public string authids { get; set; }

        /// <summary>
        /// 授权菜单按钮权限id
        /// </summary>
        public string btnids { get; set; }
    }
}


