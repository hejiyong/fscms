using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsCms.Entity
{
    [Serializable]
    public class SysMenuButton : BaseEntity
    {
        [MaxLength(20)]
        [Display(Name = "编按钮码")]
        public string ButtonCode { get; set; }

        [MaxLength(50)]
        [Display(Name = "按钮名称")]
        public string ButtonName { get; set; }

        [Display(Name = "所属模块ID")]
        public long MenuID { get; set; }

        [Display(Name = "分类")]
        public int Category { get; set; }

        [Display(Name = "序号")]
        public int Sort { get; set; }

        [MaxLength(500)]
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [ForeignKey("MenuId")]
        public virtual SysMenu Menu { get; set; }
    }
}


