using FsCms.Entity.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FsCms.Entity
{
    /// <summary>
    /// 列表数据返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortInfo<T, T2>
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public Expression<Func<T, T2>> Orderby { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public SortEnum SortMethods { get; set; }

    }
}
