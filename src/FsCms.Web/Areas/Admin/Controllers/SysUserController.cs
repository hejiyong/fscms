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
    public class SysUserController : AdminBaseController
    {
        public SysUserDAL SysUserDAL { get; set; }

        public SysRoleDAL SysRoleDAL { get; set; }

        public SysUserRoleDAL SysUserRoleDAL { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SysUser query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUser>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SysUser, bool>> predicate = ExpressionBuilder.True<SysUser>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.UserName.IndexOf(searchContent) != -1);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SysUser> list, long count) datas = await SysUserDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SysUser>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SysUser>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateModule(string id)
        {
            (List<SysRole> list, long count) role = await SysRoleDAL.QueryAsync(w => w.Status == 1);
            ViewBag.RoleList = role.list.Select(s => new SelectListItem { Text = s.RoleName, Value = s.Id.ToString() }).ToList();

            SysUser model = new SysUser() { };
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            var userroles = await SysUserRoleDAL.QueryUserRole(u => u.UserId == Convert.ToInt32(id), null, null);

            (List<SysRole> list, long count) role = await SysRoleDAL.QueryAsync(w => w.Status == 1);
            ViewBag.RoleList = role.list.Select(s => new SelectListItem { Selected = userroles.list.Exists(ss => ss.RoleId == s.Id), Text = s.RoleName, Value = s.Id.ToString() }).ToList();

            SysUser model = new SysUser() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SysUserDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SysUserView model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   model.Status = 1;
                   result.Data = await SysUserDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
                   else
                   {
                       if (!string.IsNullOrEmpty(model.userrole))
                       {
                           string[] idstr = model.userrole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                           List<SysUserRole> rolelist = (from p in idstr
                                                         select new SysUserRole
                                                         {
                                                             RoleId = Convert.ToInt32(p),
                                                             UserId = model.Id,
                                                             Status = 1,
                                                             CreateDt = DateTime.Now,
                                                             CreateBy = model.UpdateBy
                                                         }).ToList();
                           await SysUserRoleDAL.BatchInsertAsync(rolelist);
                       }
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SysUserView model)
        {
            var resdata = await AutoException.Excute<SysUser>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                var res = await SysUserDAL.UpdateAsync(model);
                result.Data = model;
                if (!res)
                {
                    throw new Exception("数据修改异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.userrole))
                    {
                        string[] idstr = model.userrole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<SysUserRole> rolelist = (from p in idstr
                                                      select new SysUserRole
                                                      {
                                                          RoleId = Convert.ToInt32(p),
                                                          UserId = model.Id,
                                                          Status = 1,
                                                          CreateDt = DateTime.Now,
                                                          CreateBy = model.UpdateBy
                                                      }).ToList();
                        await SysUserRoleDAL.BatchInsertAsync(rolelist);
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
                     var bl = await SysUserDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, true);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePwd(string id, string pwd)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
             {
                 string[] idstr = id.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                 foreach (var item in idstr)
                 {
                     var model = await SysUserDAL.GetByOneAsync(w => w.Id == Convert.ToInt32(item));
                     model.Password = pwd;//MD5编码
                     var bl = await SysUserDAL.UpdateAsync(model);
                     if (!bl) throw new Exception("密码修改异常，ID:" + item);
                 }
             }, false);
            return Json(resdata);
        }
    }
}
