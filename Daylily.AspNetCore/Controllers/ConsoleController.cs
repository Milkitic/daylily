using Microsoft.AspNetCore.Mvc;

namespace Daylily.AspNetCore.Controllers
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