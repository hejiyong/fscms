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
    public class SysUpdateLogController : AdminBaseController
    {
        public SysUpdateLogDAL SysUpdateLogDAL { get; set; }

        public async Task<IActionResult> Index()
        {
            var select = await SysUpdateLogDAL.QueryAsync(s => s.Status == 1, new List<SortInfo<SysUpdateLog, object>> {
                   new SortInfo<SysUpdateLog, object>{  Orderby=o=>o.CreateDt, SortMethods = SortEnum.Desc }
            });
            var model = select.list.FirstOrDefault();
            return View(model);
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                SysUpdateLog query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUpdateLog>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<SysUpdateLog, bool>> predicate = ExpressionBuilder.True<SysUpdateLog>();

                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.VersionNum.IndexOf(searchContent) != -1 || b.VersionNum.IndexOf(searchContent) != -1);
                }
                PageInfo pageinfo = new PageInfo { };
                (List<SysUpdateLog> list, long count) datas = await SysUpdateLogDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<SysUpdateLog>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<SysUpdateLog>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CreateModule(string id)
        {
            SysUpdateLog model = new SysUpdateLog() { Status = 1 };
            PageInfo pageinfo = new PageInfo
            {
                IsPaging = true,
                PageIndex = 1,
                PageSize = 1
            };
            //获取最近一次版本号
            var updatelog = SysUpdateLogDAL.Query(w => w.Status == 1, new List<SortInfo<SysUpdateLog, object>>
            {
                new SortInfo<SysUpdateLog, object>{  Orderby=o=>o.CreateDt, SortMethods= SortEnum.Desc}
            }, pageinfo);


            ViewBag.OldVersionName = updatelog.count == 0 ? "" : updatelog.list.FirstOrDefault().VersionNum;

            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            SysUpdateLog model = new SysUpdateLog() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await SysUpdateLogDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]SysUpdateLog model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
            {
                model.CreateBy = "admin";
                model.CreateDt = DateTime.Now;
                model.Status = 1;
                result.Data = await SysUpdateLogDAL.InsertAsync(model);
                if (result.Data == 0)
                {
                    throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                }
            }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]SysUpdateLog model)
        {
            var resdata = await AutoException.Excute<SysUpdateLog>(async (result) =>
            {
                model.Status = 1;
                var res = await SysUpdateLogDAL.UpdateAsync(model);
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
                    var bl = await SysUpdateLogDAL.DeleteAsync(Convert.ToInt32(item));
                    if (!bl) throw new Exception("数据删除异常，ID:" + item);
                }
            }, true);
            return Json(resdata);
        }
    }
}
