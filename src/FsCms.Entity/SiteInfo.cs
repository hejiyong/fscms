using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FsCms.Entity
{
    public class SiteInfo : BaseEntity
    {
        /// <summary>
        /// 站点信息
        /// </summary>
        [MaxLength(200)]
        public string SiteName { get; set; }

        /// <summary>
        /// 标题语
        /// </summary>
        [MaxLength(500)]
        public string Headline { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [MaxLength(500)]
        public string Abstract { get; set; }


    }
}
