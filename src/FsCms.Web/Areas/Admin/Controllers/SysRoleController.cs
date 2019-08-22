using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FsCms.Entity;
using FsCms.Service;
using FsCms.Service.DAL;
using FsCms.Entity.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FsCms.Web.Areas.Admin.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AuthorizeName.Items)]
    [Area(AreasName.Admin)]
    public class SysRoleController : AdminBaseController
    {
        public SysRoleDAL SysRoleDAL { get; set; }
        public SysMenuDAL SysMenuDAL { get; set; }
        public SysRoleMenuDAL SysRoleMenuDAL { get; set; }
        public SysRoleButtonDAL SysRoleButtonDAL { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SysRole query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SysRole>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SysRole, bool>> predicate = ExpressionBuilder.True<SysRole>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.RoleName.IndexOf(searchContent) != -1);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SysRole> list, long count) datas = await SysRoleDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SysRole>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SysRole>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateModule(string id)
        {
            (List<ArticleType> list, long count) articles = await new ArticleTypeDAL().QueryAsync(w => w.Status == 1);
            ViewBag.ArticleTypeList = articles.list.Select(s => new SelectListItem { Text = s.TypeName, Value = s.Id.ToString() }).ToList();
            SysRole model = new SysRole() { };
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            var roleMenus = await SysRoleMenuDAL.QueryUserRole(u => u.RoleId == Convert.ToInt32(id), null, null);

            (List<SysMenu> list, long count) menus = await SysMenuDAL.QueryAsync(w => w.Status == 1);
            ViewBag.Menus = menus.list.Select(s => new SelectListItem { Selected = roleMenus.list.Exists(ss => ss.RoleId == s.Id), Text = s.MenuName, Value = s.Id.ToString() }).ToList();

            SysRole model = new SysRole() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SysRoleDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SysRoleView model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   result.Data = await SysRoleDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
                   else
                   {
                       if (!string.IsNullOrEmpty(model.authids))
                       {
                           string[] idstr = model.authids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                           List<SysRoleMenu> rolelist = (from p in idstr
                                                         select new SysRoleMenu
                                                         {
                                                             RoleId = Convert.ToInt32(result.Data),
                                                             MenuId = Convert.ToInt32(p),
                                                             Status = 1,
                                                             CreateDt = DateTime.Now,
                                                             CreateBy = model.UpdateBy
                                                         }).ToList();
                           await SysRoleMenuDAL.BatchInsertAsync(rolelist);
                       }
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SysRoleView model)
        {
            var resdata = await AutoException.Excute<SysRole>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                var res = await SysRoleDAL.UpdateAsync(model);
                result.Data = model;
                if (!res)
                {
                    throw new Exception("数据修改异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.authids))
                    {
                        string[] idstr = model.authids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<SysRoleMenu> rolelist = (from p in idstr
                                                      select new SysRoleMenu
                                                      {
                                                          RoleId = model.Id,
                                                          MenuId = Convert.ToInt32(p),
                                                          Status = 1,
                                                          CreateDt = DateTime.Now,
                                                          CreateBy = model.UpdateBy
                                                      }).ToList();
                        await SysRoleMenuDAL.BatchInsertAsync(rolelist);
                    }
                }
            }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
             {
                 string[] idstr = id.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                 foreach (var item in idstr)
                 {
                     var bl = await SysRoleDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, true);
            return Json(resdata);
        }

        #region 授权

        public async Task<ActionResult> AuthModule(string id)
        {
            //var roleMenus = await SysRoleMenuDAL.QueryUserRole(u => u.RoleId == Convert.ToInt32(id), null, null);

            //(List<SysMenu> list, long count) menus = await SysMenuDAL.QueryAsync(w => w.Status == 1);
            //ViewBag.Menus = menus.list.Select(s => new SelectListItem { Selected = roleMenus.list.Exists(ss => ss.RoleId == s.Id), Text = s.MenuName, Value = s.Id.ToString() }).ToList();

            SysRole model = new SysRole() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SysRoleDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Auth([FromBody]SysRoleView model)
        {
            var resdata = await AutoException.Excute<SysRole>(async (result) =>
            {
                //删除
                SysRoleMenuDAL.Delete(del => del.RoleId == model.Id);
                SysRoleButtonDAL.Delete(del => del.RoleId == model.Id);

                if (!string.IsNullOrEmpty(model.authids))
                {
                    string[] idstr = model.authids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<SysRoleMenu> rolelist = (from p in idstr
                                                  select new SysRoleMenu
                                                  {
                                                      RoleId = model.Id,
                                                      MenuId = Convert.ToInt32(p),
                                                      Status = 1,
                                                      CreateDt = DateTime.Now,
                                                      CreateBy = model.UpdateBy
                                                  }).ToList();
                    await SysRoleMenuDAL.BatchInsertAsync(rolelist);

                    string[] btnids = model.btnids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<SysRoleButton> btnlist = (from p in btnids
                                                   select new SysRoleButton
                                                  {
                                                      RoleId = model.Id,
                                                      ButtonId = Convert.ToInt32(p), 
                                                      Status = 1,
                                                      CreateDt = DateTime.Now,
                                                      CreateBy = model.UpdateBy
                                                  }).ToList();
                    await SysRoleButtonDAL.BatchInsertAsync(btnlist);
                }
            }, false);
            return Json(resdata);
        }
        #endregion

    }
}
