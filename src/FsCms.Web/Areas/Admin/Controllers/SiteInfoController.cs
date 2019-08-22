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
using FsCms.Entity.Enum;

namespace FsCms.Web.Areas.Admin.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = AuthorizeName.Items)]
    [Area(AreasName.Admin)]
    public class SiteInfoController : AdminBaseController
    {
        public SiteInfoDAL SiteInfoDAL { get; set; }

        public SysDictionaryDAL SysDictionaryDAL { get; set; }

        public async Task<IActionResult> Index()
        {
            var sitelist = await SiteInfoDAL.QueryAsync(s => s.Status == 1, new List<SortInfo<SiteInfo, object>> {
                   new SortInfo<SiteInfo, object>{  Orderby=o=>o.CreateDt, SortMethods = SortEnum.Desc }
            });
            var model = sitelist.list.FirstOrDefault();
            return View(model);
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SiteInfo query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SiteInfo>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SiteInfo, bool>> predicate = ExpressionBuilder.True<SiteInfo>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.SiteName.IndexOf(searchContent) != -1 || b.SiteName.IndexOf(searchContent) != -1);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SiteInfo> list, long count) datas = await SiteInfoDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SiteInfo>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SiteInfo>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CreateModule(string id)
        {
            SiteInfo model = new SiteInfo() { Status = 1 };
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            SiteInfo model = new SiteInfo() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SiteInfoDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SiteInfo model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
            {
                model.CreateBy = "admin";
                model.CreateDt = DateTime.Now;
                model.Status = 1;
                result.Data = await SiteInfoDAL.InsertAsync(model);
                if (result.Data == 0)
                {
                    throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                }
            }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SiteInfo model)
        {
            var resdata = await AutoException.Excute<SiteInfo>(async (result) =>
            {
                model.Status = 1;
                var res = await SiteInfoDAL.UpdateAsync(model);
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
                    var bl = await SiteInfoDAL.DeleteAsync(Convert.ToInt32(item));
                    if (!bl) throw new Exception("数据删除异常，ID:" + item);
                }
            }, true);
            return Json(resdata);
        }
    }
}
