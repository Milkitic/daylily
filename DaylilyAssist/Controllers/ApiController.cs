using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DaylilyAssist.Controllers
{
    public class ApiController : Controller
    {
        public static string CQRoot { get; set; } = "";

        //[HttpPost]
        public IActionResult ImgFile(string fileName)
        {
            try
            {
                string file = Path.Combine(CQRoot, "data", "image", fileName);
                if (System.IO.File.Exists(file))
                {
                    return Content(System.IO.File.ReadAllText(file));
                }
            }
            catch { }

            return NotFound();
        }
    }
}
