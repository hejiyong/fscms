//using FreeSql.DataAnnotations;
using FreeSql.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace FsCms.Entity
{
    public class ArticleComment:BaseEntity
    {
        /// <summary>
        /// 功能类型（文章、模板、示例等）
        /// </summary>
        public int FunctionType { get; set; }

        /// <summary>
        /// 功能ID  文章、模板、示例等
        /// </summary>
        public int FunctionID { get; set; }

        /// <summary>
        /// 是否匿名访问
        /// </summary>
        public int IsAnonymous { get; set; }

        /// <summary>
        /// 评论人
        /// </summary>
        [MaxLength(100)]
        public string Commentator { get; set; }

        /// <summary>
        /// 评论者IP
        /// </summary>
        [MaxLength(100)]
        public string CommentatorIp { get; set; }

        /// <summary>
        /// 回复评论编号
        /// </summary>
        public int ReplyID { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Column(DbType = "text")]
        public string CommentContent { get; set; }
    }
}
