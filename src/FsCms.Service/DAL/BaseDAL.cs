using FreeSql;
using FsCms.Entity;
using FsCms.Entity.Common;
using FsCms.Entity.Enum;
using FsCms.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FsCms.Service.DAL
{
    public class BaseDAL<T> where T : BaseEntity
    {
        public static DataType DbType = EnumHelper.StringConvertToEnum<DataType>(AppSettingsManager.Get("DbContexts:DbType"));

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async virtual Task<long> CountAsync()
        {
            var runsql = DbType.DB().Select<T>();
            return await runsql.CountAsync();
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async virtual Task<long> InsertAsync(T model)
        {
            var runsql = DbType.DB().Insert<T>(model);
            return await runsql.ExecuteIdentityAsync();
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async virtual Task<int> BatchInsertAsync(List<T> models)
        {
            var runsql = DbType.DB().Insert<T>().AppendData(models);
            return await runsql.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async virtual Task<bool> UpdateAsync(T model)
        {
            var runsql = DbType.DB().Update<T>().SetSource(model);
            var rows = await runsql.ExecuteAffrowsAsync();
            return rows > 0;
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async virtual Task<bool> DeleteAsync(long id)
        {
            var result = await DbType.DB().Delete<T>(id).ExecuteAffrowsAsync();
            return result > 0;
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> where)
        {
            var result = DbType.DB().Delete<T>().Where(where).ExecuteAffrows();
            return result > 0;
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async virtual Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
        {
            var result = await DbType.DB().Delete<T>().Where(where).ExecuteAffrowsAsync();
            return result > 0;
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async virtual Task<T> GetByOneAsync(Expression<Func<T, bool>> where)
        {
            return await DbType.DB().Select<T>()
                .Where(where).ToOneAsync();
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public async virtual Task<(List<T> list, long count)> QueryAsync(Expression<Func<T, bool>> where,
             List<SortInfo<T, object>> orderbys = null, PageInfo pageInfo = null)
        {
            //设置查询条件
            var list = DbType.DB().Select<T>()
                .Where(where);
            
            var count = list.Count();

            BaseEntity baseEntity = new BaseEntity();
            //设置排序
            if (orderbys == null) list = list.OrderBy(s => s.CreateDt);
            else
            {
                foreach (var item in orderbys)
                {
                    list = item.SortMethods == SortEnum.Asc ? list.OrderBy(item.Orderby) : list.OrderByDescending(item.Orderby);
                }
            }
            
            //设置分页操作
            if (pageInfo != null && pageInfo.IsPaging)
                list.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Limit(pageInfo.PageSize);
            var resultList = await list.ToListAsync();
            //执行查询
            return (resultList, count);
        }

        #region no Async

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual long Count()
        {
            var runsql = DbType.DB().Select<T>();
            return runsql.Count();
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual long Insert(T model)
        {
            var runsql = DbType.DB().Insert<T>(model);
            return runsql.ExecuteIdentity();
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int BatchInsert(List<T> models)
        {
            var runsql = DbType.DB().Insert<T>().AppendData(models);
            return runsql.ExecuteAffrows();
        }

        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update(T model)
        {
            var runsql = DbType.DB().Update<T>().SetSource(model);
            var rows = runsql.ExecuteAffrows();
            return rows > 0;
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(long id)
        {
            var result = DbType.DB().Delete<T>(id).ExecuteAffrows();
            return result > 0;
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetByOne(Expression<Func<T, bool>> where)
        {
            return DbType.DB().Select<T>()
                .Where(where).ToOne();
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public virtual (List<T> list, long count) Query(Expression<Func<T, bool>> where,
            List<SortInfo<T, object>> orderbys = null, PageInfo pageInfo = null)
        {
            //设置查询条件
            var list = DbType.DB().Select<T>()
                .Where(where);

			
            var count = list.Count();

            BaseEntity baseEntity = new BaseEntity();
            //设置排序
            if (orderbys == null) list = list.OrderBy(s => s.CreateDt);
            else
            {
                foreach (var item in orderbys)
                {
                    list = item.SortMethods == SortEnum.Asc ? list.OrderBy(item.Orderby) : list.OrderByDescending(item.Orderby);
                }
            }

            //设置分页操作
            if (pageInfo != null && pageInfo.IsPaging)
                list.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Limit(pageInfo.PageSize);
            var resultList = list.ToList();
            //执行查询
            return (resultList, count);
        }
        #endregion
    }
}
