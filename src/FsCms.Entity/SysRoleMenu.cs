using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FsCms.Entity
{
    public class SysRoleMenu : BaseEntity
    {
        [Display(Name = "角色ID")]
        public long RoleId { get; set; }

        [Display(Name = "菜单ID")]
        public long MenuId { get; set; }

        [Display(Name = "授权按钮")]
        [MaxLength(100)]
        public string ButtonCodes { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("MenuId")]
        public virtual SysMenu Menu { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("RoleId")]
        public virtual SysRole Role { get; set; }
    }
}


