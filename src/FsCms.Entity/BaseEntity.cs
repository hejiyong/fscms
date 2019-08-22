//using FreeSql.DataAnnotations;
using FreeSql.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace FsCms.Entity
{
    public class BaseEntity
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; } = 1;

        public DateTime? CreateDt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string CreateBy { get; set; } = "admin";

    }
}
