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
    public class TemplateExampleController : AdminBaseController
    {

        public TemplateExampleDAL TemplateExampleDAL { get; set; }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string searchContent, string seniorQueryJson, int page = 1, int limit = 10, string sidx = "CreateDt", string sord = "desc")
        {
            try
            {
                TemplateExample query = null;
                if (!string.IsNullOrEmpty(seniorQueryJson))
                {
                    query = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateExample>(seniorQueryJson);
                }
                System.Linq.Expressions.Expression<Func<TemplateExample, bool>> predicate = ExpressionBuilder.True<TemplateExample>();
                predicate = predicate.And(b => b.Id > 0);

                if (searchContent != null)
                {
                    predicate = predicate.And(b => b.TempateName.IndexOf(searchContent) != -1);
                }

                PageInfo pageinfo = new PageInfo { };
                (List<TemplateExample> list, long count) datas = await TemplateExampleDAL.QueryAsync(predicate, null, pageinfo);

                var lists = datas.list;
                return lists.GetJson<TemplateExample>(sidx, sord, page, limit, SysTool.GetPropertyNameArray<TemplateExample>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CreateModule(string id)
        {
            TemplateExample model = new TemplateExample() { };
            return View(model);
        }

        public async Task<ActionResult> UpdateModule(string id)
        {
            TemplateExample model = new TemplateExample() { };
            if (!string.IsNullOrEmpty(id) && id != "0")
            {
                int _id = Convert.ToInt32(id);
                model = await TemplateExampleDAL.GetByOneAsync(w => w.Id == _id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]TemplateExample model)
        {
            var resdata = await AutoException.Excute<long>(async (result) =>
               {
                   model.CreateBy = "admin";
                   model.CreateDt = DateTime.Now;
                   result.Data = await TemplateExampleDAL.InsertAsync(model);
                   if (result.Data == 0)
                   {
                       throw new Exception("数据新增异常，JSON:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                   }
               }, false);
            return Json(resdata);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody]TemplateExample model)
        {
            var resdata = await AutoException.Excute<TemplateExample>(async (result) =>
            {
                model.UpdateBy = "admin";
                model.UpdateDt = DateTime.Now;
                var res = await TemplateExampleDAL.UpdateAsync(model);
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
                     var bl = await TemplateExampleDAL.DeleteAsync(Convert.ToInt32(item));
                     if (!bl) throw new Exception("数据删除异常，ID:" + item);
                 }
             }, false);
            return Json(resdata);
        }

    }
}
