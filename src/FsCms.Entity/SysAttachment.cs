using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsCms.Entity
{
    public class SysAttachment:BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "附件名称")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "附件大小")]
        public int? FileSize { get; set; }

        [MaxLength(10)]
        [Display(Name = "附件扩展名")]
        public string ExtName { get; set; }

        [MaxLength(50)]
        [Display(Name = "保存的文件名称")]
        public string LocalName { get; set; }

        [MaxLength(200)]
        [Display(Name = "附件路径")]
        public string FilePath { get; set; }

        [Display(Name = "所属对象编号")]
        public string objectid { get; set; }

        /// <summary>
        /// 
        /// 
        /// 20 保险单的图片
        /// 21 保险单的文件
        /// </summary>
        [Required]
        [Display(Name = "附件标识")]
        public int? objectflag { get; set; }

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


