using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FsCms.Entity
{
    public class SysUserRole : BaseEntity
    {
        [Display(Name = "角色ID")]
        public long RoleId { get; set; }

        [Display(Name = "用户ID")]
        public long UserId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("UserId")]
        public virtual SysUser User { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("RoleId")]
        public virtual SysRole Role { get; set; }
    }

    public class SysUserRoleView : SysUserRole
    {
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string RoleName { get; set; }

        public string RoleCode { get; set; }

    }
}


