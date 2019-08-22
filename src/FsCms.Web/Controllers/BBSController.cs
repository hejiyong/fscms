using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FsCms.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FsCms.Web.Controllers
{
    public class BBSController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Ask() {
            return View();
        }

    }
}