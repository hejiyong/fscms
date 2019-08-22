using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FsCms.Entity
{
    /// <summary>
    /// 系统更新日志
    /// </summary>
    public class SysUpdateLog : BaseEntity
    {
        /// <summary>
        /// 版本号
        /// </summary>
        [MaxLength(20)]
        public string VersionNum { get; set; }

        /// <summary>
        /// 版本说明
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

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
