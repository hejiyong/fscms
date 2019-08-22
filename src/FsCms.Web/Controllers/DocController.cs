using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FsCms.Web.Models;
using FsCms.Service.DAL;
using FsCms.Entity;
using Microsoft.AspNetCore.Http;

namespace FsCms.Web.Controllers
{
    public class DocController : Controller
    {
        public ArticleTypeDAL ArticleTypeDAL { get; set; }

        public ArticleContentDAL ArticleContentDAL { get; set; }

        /// <summary>
        /// Doc导航页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Index(int id = 1)
        {
            var typeList = ArticleTypeDAL.Query(d => d.Id != 0).list.OrderBy(s => s.SortNum).ToList();
            var contentlist = ArticleContentDAL.Query(d => d.Status == 1, new List<SortInfo<ArticleContent, object>>
            {
                new SortInfo<ArticleContent, object>{ Orderby=s=>s.SortNum, SortMethods= Entity.Enum.SortEnum.Asc}
            }).list.OrderBy(s => s.SortNum).ToList();

            //适应两层结构即可
            var query = (from p in typeList
                         where p.UpID == null || p.UpID == 0
                         select new TreeData(p, typeList).AddChildrens(GetContentTreeData(p.Id, contentlist), (tid) => GetContentTreeData(tid, contentlist))).ToList();

            ViewBag.DocumentList = query;
            ViewBag.DocID = contentlist.Exists(e => e.Id == id) ? id : contentlist.FirstOrDefault()?.Id;
            return View();
        }

        private List<TreeData> GetContentTreeData(long id, List<ArticleContent> contentlist)
        {
            return contentlist.Where(w => w.TypeID == id).Select(s => new TreeData
            {
                id = s.Id,
                text =  s.Title,
                intextfield = s.LevelNum,
                datatype = 1
            }).ToList();
        }

        // GET: Doc/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.DocumentID = id;
            var doc = this.ArticleContentDAL.GetByOne(w => w.Id == id);
            ViewBag.DocumentInfo = doc;
            return this.PartialView();
        }

        // GET: Doc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Doc/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Doc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Doc/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Doc/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}