using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FsCms.Entity;
using FsCms.Service.DAL;
using FsCms.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FsCms.Web.Controllers
{
    public class ExampleController : Controller
    {
        public ArticleTypeDAL ArticleTypeDAL { get; set; }

        public TemplateExampleDAL TemplateExampleDAL { get; set; }

        public IActionResult Index(int id = 1)
        {
            var typeList = ArticleTypeDAL.Query(d => d.Id != 0).list.OrderBy(s => s.SortNum).ToList();
            var contentlist = TemplateExampleDAL.Query(d => d.Status == 1, new List<SortInfo<TemplateExample, object>>
            {
                new SortInfo<TemplateExample, object>{ Orderby=s=>s.Id, SortMethods= Entity.Enum.SortEnum.Asc}
            }).list.OrderBy(s => s.Id).ToList();

            //适应两层结构即可
            var query = (from p in typeList
                         where p.UpID == null || p.UpID == 0
                         select new TreeData(p, typeList)).ToList();//.AddChildrens(GetContentTreeData(p.Id, contentlist), (tid) => GetContentTreeData(tid, contentlist))).ToList();

            ViewBag.DocumentList = query;
            ViewBag.DocID = contentlist.Exists(e => e.Id == id) ? id : contentlist.FirstOrDefault()?.Id;
            return View();
        }

        private List<TreeData> GetContentTreeData(long id, List<TemplateExample> contentlist)
        {
            return contentlist.Where(w => w.TypeID == id).Select(s => new TreeData
            {
                id = s.Id,
                text = s.TempateName,
                intextfield = 0,
                datatype = 1
            }).ToList();
        }

    }
}