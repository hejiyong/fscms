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
    public class ArticleController : AdminBaseController
    {
        public ArticleTypeDAL ArticleTypeDAL { get; set; }

        public ArticleContentDAL ArticleContentDAL { get; set; }

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
                ArticleContent query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<ArticleContent>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<ArticleContent, bool>> predicate = ExpressionBuilder.True<ArticleContent>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.Title.IndexOf(searchContent) != -1);
                }
                if (query != null)
                {
                    if (query.TypeID != null)
                    {
                        predicate = predicate.And(b => b.TypeID == query.TypeID);
                    }
                }

                PageInfo pageinfo = new PageInfo { };
                (List<ArticleContent> list, long count) datas = await ArticleContentDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<ArticleContent>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<ArticleContent>());
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
            ArticleContent model = new ArticleContent() { OriginType = 0 };
            if (!string.IsNullOrEmpty(id))
            {
                model.TypeID = Convert.ToInt32(id);
            }
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            (List<ArticleType> list, long count) articles = await ArticleTypeDAL.QueryAsync(w => w.Status == 1);
            ViewBag.ArticleTypeList = articles.list.Select(s => new SelectListItem { Text = s.TypeName, Value = s.Id.ToString() }).ToList();

            ArticleContent model = new ArticleContent() { OriginType = 0 };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await ArticleContentDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]ArticleContent model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   result.Data = await ArticleContentDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]ArticleContent model)
        {
            var resdata = await AutoException.Excute<ArticleContent>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                var res = await ArticleContentDAL.UpdateAsync(model);
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
                     var bl = await ArticleContentDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, false);
            return Json(resdata);
        }

        [HttpPost]
        public ActionResult SyncData()
        {
            var resdata = AutoException.Execute((result) =>
            {
                new Common.Help.ReadWikiHepler().WikiToArticle();
                result.Status = 1;
            }, false);
            return Json(resdata);
        }
    }
}
