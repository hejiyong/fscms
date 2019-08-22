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
using AutoMapper;

namespace FsCms.Web.Areas.Admin.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AuthorizeName.Items)]
    [Area(AreasName.Admin)]
    public class SysMenuController : AdminBaseController
    {
        public SysMenuDAL SysMenuDAL { get; set; }

        public SysRoleMenuDAL SysRoleMenuDAL { get; set; }

        public SysMenuButtonDAL SysMenuButtonDAL { get; set; }

        public SysRoleButtonDAL SysRoleButtonDAL { get; set; }

        public ArticleTypeDAL ArticleTypeDAL { get; set; }

        public SysDictionaryDAL SysDictionaryDAL { get; set; }

        private readonly IMapper _mapper;

        public SysMenuController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<SysMenu> list, long count) datas = await new SysMenuDAL().QueryAsync(q => q.Status == 1, null, pageinfo);

            ViewBag.SysMenuTreeDatas = (from p in datas.list where ((p.ParentID ?? 0) == 0) select new SysMenuTreeNode(datas.list, p) { }).ToList();
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SysMenu query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SysMenu>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SysMenu, bool>> predicate = ExpressionBuilder.True<SysMenu>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.MenuName.IndexOf(searchContent) != -1);
                }
                if (query.ParentID != null)
                {
                    predicate = predicate.And(b => b.ParentID == query.ParentID);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SysMenu> list, long count) datas = await new SysMenuDAL().QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SysMenu>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SysMenu>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateModule(string id, string parentid = "")
        {
            (List<SysDictionary> list, long count) buttons = await SysDictionaryDAL.QueryAsync(w => w.Status == 1 && w.Parent.DictNo == AuthorizeName.PermissionButton);
            ViewBag.ButtonList = buttons.list.Select(s => new SelectListItem { Text = s.DictName, Value = s.DictNo.ToString() }).ToList();

            (List<SysMenu> list, long count) menus = await SysMenuDAL.QueryAsync(w => w.Status == 1);
            //var lists = menus.list.Select(s => new SysMenuTreeNode
            //{
            //    name = s.MenuName,
            //    id = s.Id.ToString()
            //}).ToList();

            ViewBag.SysMenuList = menus.list.Select(s => new SelectListItem { Text = s.MenuName, Value = s.Id.ToString() }).ToList();

            SysMenuView model = new SysMenuView() { Status = 1 };
            if (!string.IsNullOrEmpty(parentid))
            {
                model.ParentID = Convert.ToInt32(parentid);
            }
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            //获取菜单已经绑定的按钮
            var menubuttons = await SysMenuButtonDAL.QueryAsync(u => u.MenuID == Convert.ToInt32(id), null, null);

            //回去权限按钮字典
            (List<SysDictionary> list, long count) buttons = await SysDictionaryDAL.QueryAsync(w => w.Status == 1 && w.Parent.DictNo == AuthorizeName.PermissionButton);
            ViewBag.ButtonList = buttons.list.Select(s => new SelectListItem
            {
                Text = s.DictName,
                Value = s.DictNo.ToString(),
                Selected = menubuttons.list.Exists(e => e.ButtonCode == s.DictNo)
            }).ToList();

            (List<SysMenu> list, long count) menus = await SysMenuDAL.QueryAsync(w => w.Status == 1);
            ViewBag.SysMenuList = menus.list.Select(s => new SelectListItem
            {
                Text = s.MenuName,
                Value = s.Id.ToString()
            }).ToList();

            SysMenuView model = new SysMenuView() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                var tempModel = await new SysMenuDAL().GetByOneAsync(w => w.Id == _id);
                model = _mapper.Map<SysMenuView>(tempModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SysMenuView model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   model.ParentID = model.ParentID == null ? 0 : model.ParentID;
                   result.Data = await new SysMenuDAL().InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
                   else
                   {
                       if (!string.IsNullOrEmpty(model.menubuttons))
                       {
                           var idstr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(model.menubuttons);
                           List<SysMenuButton> rolelist = (from p in idstr
                                                           select new SysMenuButton
                                                           {
                                                               MenuID = result.Data,
                                                               ButtonCode = p.id,
                                                               ButtonName = p.title,
                                                               Status = 1,
                                                               CreateDt = DateTime.Now,
                                                               CreateBy = model.UpdateBy
                                                           }).ToList();
                           await SysMenuButtonDAL.BatchInsertAsync(rolelist);
                       }
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SysMenuView model)
        {
            var resdata = await AutoException.Excute<SysMenu>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                var res = await new SysMenuDAL().UpdateAsync(model);
                result.Data = model;
                if (!res)
                {
                    throw new Exception("数据修改异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.menubuttons))
                    {
                        //删除原来的按钮
                        await SysMenuButtonDAL.DeleteAsync(d => d.MenuID == model.Id);
                        var idstr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(model.menubuttons);
                        List<SysMenuButton> rolelist = (from p in idstr
                                                        select new SysMenuButton
                                                        {
                                                            MenuID = model.Id,
                                                            ButtonCode = p.id,
                                                            ButtonName = p.title,
                                                            Status = 1,
                                                            CreateDt = DateTime.Now,
                                                            CreateBy = model.UpdateBy
                                                        }).ToList();
                        await SysMenuButtonDAL.BatchInsertAsync(rolelist);
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
                     var bl = await new SysMenuDAL().DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, false);
            return Json(resdata);
        }

        /// <summary>
        /// id 为对象的编号 角色 还是 用户
        /// type =1 角色  =2 用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"> =1 表示角色 =2 用户</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetTree(long id, int type = 1)
        {
            //已授权的角色菜单
            var roleMenus = await SysRoleMenuDAL.QueryUserRole(u => u.RoleId == Convert.ToInt32(id), null, null);
            var roleButtons = await SysRoleButtonDAL.QueryAsync(u => u.RoleId == Convert.ToInt32(id), null, null);
            //所有状态为有效的菜单
            (List<SysMenu> list, long count) menus = await SysMenuDAL.QueryAsync(w => w.Status == 1);
            var lists = menus.list.Select(s =>
            {
                var btns = s.SysMenuButtons ?? new List<SysMenuButton>();
                var newBtns = btns.Select(b => new TreeNode
                {
                    id = b.Id.ToString(),
                    name = b.ButtonCode,
                    title = b.ButtonName,
                    ischecked = roleButtons.list.Exists(e => e.ButtonId == b.Id)
                }).ToList();

                var strbtns = Newtonsoft.Json.JsonConvert.SerializeObject(newBtns);
                return new SysMenuTreeNode
                {
                    ischecked = roleMenus.list.Exists(ss => ss.MenuId == s.Id),
                    name = s.MenuName,
                    title = s.MenuName,
                    pid = (s.ParentID ?? 0).ToString(),
                    id = s.Id.ToString(),
                    //buttons = newBtns,
                    jsonButtons = strbtns
                };
            }).ToList();

            return Json(lists);
        }

        [HttpGet]
        public async Task<ActionResult> GetTreeData(long id, int type = 1)
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<SysMenu> list, long count) datas = await new SysMenuDAL().QueryAsync(q => q.Status == 1, null, pageinfo);

            var SysMenuTreeDatas = (from p in datas.list where ((p.ParentID ?? 0) == 0) select new SysMenuTreeNode(datas.list, p) { }).ToList();

            return Json(SysMenuTreeDatas);
        }
    }
}
