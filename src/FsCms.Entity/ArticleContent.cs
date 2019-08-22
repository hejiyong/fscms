//using FreeSql.DataAnnotations;
using FreeSql.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace FsCms.Entity
{
    /// <summary>
    /// 数据库实体
    /// </summary>
    public class ArticleContent : BaseEntity
    {
        /// <summary>
        /// 类型编号
        /// </summary>
        public long? TypeID { get; set; }

        /// <summary>
        /// 父节点文章
        /// </summary>
        public long? ParentArticleID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [MaxLength(500)]
        public string Abstract { get; set; }

        /// <summary>
        /// 内容来源类型（0 当前记录 1=Url地址
        /// </summary>
        public int OriginType { get; set; }

        /// <summary>
        /// 来源地址
        /// </summary>
        [MaxLength(200)]
        public string OriginUrl { get; set; }

        /// <summary>
        /// 编辑器模式 （=0 Markdown =1 HTML编辑器 ）
        /// </summary>
        public int EditorMode { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        [Column(DbType = "text")]
        public string DocContent { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int WatchCount { get; set; }

        /// <summary>
        /// Star统计
        /// </summary>
        public int StarCount { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDt { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 层级编码
        /// </summary>
        public int LevelNum { get; set; } = 0;
    }

    /// <summary>
    /// 返回实体内容
    /// </summary>
    public class DocumentContentView : ArticleContent
    {

    }
}
