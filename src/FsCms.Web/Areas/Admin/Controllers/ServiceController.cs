
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FsCms.Entity;
using FsCms.Entity.Enum;
using FsCms.Service.DAL;
using Microsoft.AspNetCore.Mvc;


namespace FsCms.Web.Areas.Admin.Controllers
{
    [Area(AreasName.Admin)]
    public class ServiceController : AdminBaseController
    {
        public IActionResult Error(string msg = "")
        {
            ViewBag.Msg = msg;
            return View();
        }
    }
}
