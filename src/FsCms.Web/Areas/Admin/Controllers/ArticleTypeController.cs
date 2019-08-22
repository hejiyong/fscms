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
    public class ArticleTypeController : AdminBaseController
    {
        public ArticleTypeDAL ArticleTypeDAL { get; set; }

        public async Task<IActionResult> Index()
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<ArticleType> list, long count) datas = await ArticleTypeDAL.QueryAsync(q => q.Status == 1, null, pageinfo);

            ViewBag.ArticleTreeDatas = (from p in datas.list where (p.UpID == 0 || p.UpID == null) select new ArticleTypeTreeNode(datas.list, p) { }).ToList();
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                ArticleType query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<ArticleType>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<ArticleType, bool>> predicate = ExpressionBuilder.True<ArticleType>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.TypeName.IndexOf(searchContent) != -1);
                }
                if (query != null)
                {
                    if (query.UpID != null)
                    {
                        predicate = predicate.And(b => b.UpID == query.UpID);
                    }
                }
                PageInfo pageinfo = new PageInfo { };
                (List<ArticleType> list, long count) datas = await ArticleTypeDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<ArticleType>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<ArticleType>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateModule(string id)
        {
            (List<ArticleType> list, long count) articles = await ArticleTypeDAL.QueryAsync(w => w.Status == 1);
            ViewBag.ArticleTypeList = articles.list.Select(s => new SelectListItem { Text = s.TypeName, Value = s.Id.ToString() }).ToList();
            ArticleType model = new ArticleType() { Status = 1 };
            if (!string.IsNullOrEmpty(id))
            {
                model.UpID = Convert.ToInt32(id);
            }
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            (List<ArticleType> list, long count) articles = await ArticleTypeDAL.QueryAsync(w => w.Status == 1);
            ViewBag.ArticleTypeList = articles.list.Select(s => new SelectListItem { Text = s.TypeName, Value = s.Id.ToString() }).ToList();

            ArticleType model = new ArticleType() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await ArticleTypeDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]ArticleType model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   result.Data = await ArticleTypeDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]ArticleType model)
        {
            var resdata = await AutoException.Excute<ArticleType>(async (result) =>
        {
            model.UpdateBy = "admin";
            model.UpdateDt = DateTime.Now;
            var res = await ArticleTypeDAL.UpdateAsync(model);
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
                     var bl = await ArticleTypeDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, false);
            return Json(resdata);
        }

        [HttpGet]
        public async Task<ActionResult> GetTree()
        {
            PageInfo pageinfo = new PageInfo { IsPaging = false };
            (List<ArticleType> list, long count) datas = await ArticleTypeDAL.QueryAsync(q => q.Status == 1, null, pageinfo);
            var ArticleTreeDatas = (from p in datas.list where (p.UpID == 0 || p.UpID == null) select new ArticleTypeTreeNode(datas.list, p) { }).ToList();
            return Json(ArticleTreeDatas);
        }
    }
}
