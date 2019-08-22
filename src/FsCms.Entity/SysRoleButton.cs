using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsCms.Entity
{
    public  class SysRoleButton : BaseEntity
    {
        [Required]
        [Display(Name = "角色ID")]
        public long RoleId { get; set; }
        
        [Required]
        [Display(Name = "按钮ID")]
        public long ButtonId{ get; set; }

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


    }
}


