using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FsCms.Entity;
using FsCms.Entity.Enum;
using FsCms.Service.DAL;
using Microsoft.AspNetCore.Mvc;


namespace FsCms.Web.Areas.Admin.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AuthorizeName.Items)]
    [Area(AreasName.Admin)]
    public class HomeController : AdminBaseController
    {
        public SysRoleMenuDAL SysRoleMenuDAL { get; set; }
        public SysMenuDAL SysMenuDAL { get; set; }

        public async Task<IActionResult> Index(int type)
        {
            var sid = this.UserID;
            var usertype = this.UserType;

            List<SysMenu> list = new List<SysMenu> { };
            if (usertype == UserType.SuperUser)
            {
                var menus = await SysMenuDAL.QueryAsync(w => w.Status == 1, null, null);
                list = menus.list;
            }
            else
            {
                var userMenus = await SysRoleMenuDAL.QueryUserMenu(Convert.ToInt64(sid));
                userMenus.list.ForEach(m =>
                {
                    list.Add(m.Menu);
                });
            }

            ViewBag.menuList = (from p in list
                                where (p.ParentID ?? 0) == 0
                                select new SysMenuTreeNode(list, p)).ToList();
            return View();
        }
    }
}
