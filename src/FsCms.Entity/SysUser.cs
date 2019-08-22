using FreeSql.DataAnnotations;
using FsCms.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FsCms.Entity
{
    public class SysUser : BaseEntity
    {
        [MaxLength(50)]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(100)]
        [Display(Name = "姓名")]
        public string RealName { get; set; }

        [MaxLength(500)]
        [Display(Name = "头像")]
        public string Img { get; set; }

        [Display(Name = "用户类别")]
        [Column(MapType = typeof(int))]
        public UserType UserType { get;  set; }

        [MaxLength(50)]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [MaxLength(15)]
        [Display(Name = "联系电话")]
        public string MobilePhone { get; set; }

        [MaxLength(36)]
        [Display(Name = "所属组织")]
        public string OrgID { get; set; }

        [MaxLength(50)]
        [Display(Name = "最后访问IP")]
        public string LastViewIP { get; set; }

        [Display(Name = "最后访问时间")]
        public DateTime? LastViewDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "修改人")]
        public string UpdateBy { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? UpdateDt { get; set; }

        [Display(Name = "微信OpenId")]
        [MaxLength(50)]
        public string OpenId { get; set; }

        [Display(Name = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

    }


    public class SysUserView : SysUser
    {
        /// <summary>
        /// 用户角色
        /// </summary>
        public string userrole { get; set; }
    }
}


