//using FsCms.Entity;
using FreeSql;
using FsCms.Entity;
using FsCms.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FsCms.Service.DAL
{
    public class SysUserRoleDAL : BaseDAL<SysUserRole>, Ioc.ISingletonDependency
    {
        public async Task<(List<SysUserRole> list, long count)> QueryUserRole(Expression<Func<SysUserRole, bool>> where, Expression<Func<SysUserRole, bool>> orderby = null, PageInfo pageInfo = null)
        {
            //设置查询条件
            var select = DbType.DB().Select<SysUserRole>();

            var list = select.LeftJoin<SysUser>((a, b) => a.UserId == b.Id)
                .LeftJoin<SysRole>((a, c) => a.RoleId == c.Id)
                .Where(where);

            BaseEntity baseEntity = new BaseEntity();
            //设置排序
            if (orderby != null) list = list.OrderBy(nameof(baseEntity.CreateDt) + " desc ");

            var count = list.Count();
            //设置分页操作
            if (pageInfo != null && pageInfo.IsPaging)
                list.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Limit(pageInfo.PageSize);

            var resultList = await list.ToListAsync();

            //执行查询
            return (resultList, count);
        }
    }
}
