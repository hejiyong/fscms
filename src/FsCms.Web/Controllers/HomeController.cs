using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FsCms.Web.Models;
using FsCms.Service.DAL;

namespace FsCms.Web.Controllers
{
    public class HomeController : Controller
    {
        public ArticleContentDAL ArticleContentDAL { get; set; }
        public SysDictionaryDAL SysDictionaryDAL { get; set; }


        public IActionResult Index()
        {
            ViewBag.sketchlist = SysDictionaryDAL.Query(s => s.Parent.DictNo == "Index_Sketch", null, null);
            return View();
        }

        public IActionResult Header()
        {
            ViewBag.pathUrl = this.HttpContext.Request.Path.Value;
            ViewBag.ArticleList = ArticleContentDAL.Query(s => s.Status == 1, null, null);
            return PartialView();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
