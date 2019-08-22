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
    public class SysRoleMenuDAL : BaseDAL<SysRoleMenu>, Ioc.ISingletonDependency
    {
        public async Task<(List<SysRoleMenu> list, long count)> QueryUserRole(Expression<Func<SysRoleMenu, bool>> where, Expression<Func<SysRoleMenu, bool>> orderby = null, PageInfo pageInfo = null)
        {
            //设置查询条件
            var select = DbType.DB().Select<SysRoleMenu>();

            var list = select.LeftJoin<SysMenu>((a, b) => a.MenuId == b.Id)
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

        /// <summary>
        /// 获取用户菜单权限
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public async Task<(List<SysRoleMenu> list, long count)> QueryUserMenu(long userid)
        {
            //设置查询条件
            var select = DbType.DB()
                .Select<SysRoleMenu>();
            var list = select.LeftJoin<SysUserRole>((a, b) => a.RoleId == b.RoleId)
                .LeftJoin<SysMenu>((a, c) => a.MenuId == c.Id)
                .Where<SysUserRole>((rm, ur) => ur.UserId == userid);
            var count = list.Count();
            var resultList = await list.ToListAsync();
            //执行查询
            return (resultList, count);
        }
    }
}
