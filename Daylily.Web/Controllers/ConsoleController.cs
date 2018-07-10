using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Daylily.Web.Controllers
{
    public class ConsoleController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Daylily Console";
            return View();
        }
    }
}