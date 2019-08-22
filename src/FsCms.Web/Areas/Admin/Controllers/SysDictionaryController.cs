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
    public class SysDictionaryController : AdminBaseController
    {
        public SysDictionaryDAL SysDictionaryDAL { get; set; }


        public async Task<IActionResult> Index()
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<SysDictionary> list, long count) datas = await new SysDictionaryDAL().QueryAsync(q => q.Status == 1, null, pageinfo);

            ViewBag.SysDictionaryTreeDatas = (from p in datas.list where (p.ParentID == 0) select new SysDictionaryTreeNode(datas.list, p) { }).ToList();
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SysDictionary query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SysDictionary>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SysDictionary, bool>> predicate = ExpressionBuilder.True<SysDictionary>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.DictNo.IndexOf(searchContent) != -1 || b.DictName.IndexOf(searchContent) != -1);
                }
                if (query.ParentID != null)
                {
                    predicate = predicate.And(b => b.ParentID == query.ParentID);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SysDictionary> list, long count) datas = await SysDictionaryDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SysDictionary>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SysDictionary>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateModule(string id, string parentid = "")
        {
            (List<SysDictionary> list, long count) dictionary = await new SysDictionaryDAL().QueryAsync(w => w.Status == 1);
            ViewBag.SysDictionaryList = dictionary.list.Select(s => new SelectListItem { Text = s.DictName, Value = s.Id.ToString() }).ToList();

            SysDictionary model = new SysDictionary() { Status = 1 };
            if (!string.IsNullOrEmpty(parentid))
            {
                model.ParentID = Convert.ToInt32(parentid);
            }
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            (List<SysDictionary> list, long count) dictionary = await new SysDictionaryDAL().QueryAsync(w => w.Status == 1);
            ViewBag.SysDictionaryList = dictionary.list.Select(s => new SelectListItem { Text = s.DictName, Value = s.Id.ToString() }).ToList();

            SysDictionary model = new SysDictionary() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SysDictionaryDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SysDictionary model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   model.Status = 1;
                   model.ParentID = model.ParentID == null ? 0 : model.ParentID;
                   result.Data = await SysDictionaryDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SysDictionary model)
        {
            var resdata = await AutoException.Excute<SysDictionary>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                model.Status = 1;
                var res = await SysDictionaryDAL.UpdateAsync(model);
                result.Data = model;
                if (!res)
                {
                    throw new Exception("数据修改异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
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
                     var bl = await SysDictionaryDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, true);
            return Json(resdata);
        }

        [HttpGet]
        public async Task<ActionResult> GetTree(long id, int type = 1)
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<SysDictionary> list, long count) datas = await new SysDictionaryDAL().QueryAsync(q => q.Status == 1, null, pageinfo);

            var SysDictionaryTreeDatas = (from p in datas.list where (p.ParentID == 0) select new SysDictionaryTreeNode(datas.list, p) { }).ToList();

            return Json(SysDictionaryTreeDatas);
        }
    }
}
