using FreeSql.DataAnnotations;
using FsCms.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FsCms.Entity
{
    /// <summary>
    /// 模板示例
    /// </summary>
    public class TemplateExample : BaseEntity
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        [Display(Name = "数据类型")]
        [Column(MapType = typeof(int))]
        public ExampleOrTemplate? DataType { get; set; }

        /// <summary>
        /// 模板图片
        /// </summary>
        [MaxLength(2000)]
        public string TemplateImg { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [Display(Name = "模板名称")]
        [MaxLength(100)]
        public string TempateName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [Column(DbType = "text")]
        public string Describe { get; set; }

        /// <summary>
        /// 模板路径  例如Github地址
        /// </summary>
        [Display(Name = "模板路径")]
        [MaxLength(300)]

        public string TemplatePath { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        [Display(Name = "查看次数")]

        public int WatchCount { get; set; }

        /// <summary>
        /// 下载统计
        /// </summary>
        [Display(Name = "下载统计")]

        public int DownloadCount { get; set; }

        /// <summary>
        /// Star统计
        /// </summary>
        [Display(Name = "Star统计")]

        public int StarCount { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]

        public DateTime? UpdateDt { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [Display(Name = "修改人")]
        [MaxLength(50)]
        public string UpdateBy { get; set; }
    }
}
