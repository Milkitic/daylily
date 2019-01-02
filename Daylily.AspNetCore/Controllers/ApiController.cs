using System.IO;
using System.Threading.Tasks;
using Daylily.Bot.Models;
using Microsoft.AspNetCore.Mvc;

namespace Daylily.AspNetCore.Controllers
{
    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> GetResponse()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if (ip == "74.120.171.198")
            {
                //Logger.Warn("来自白菜的请求：" + ip);
                return Json(new { });
            }

            using (var sr = new StreamReader(Request.Body))
            {
                string json = await sr.ReadToEndAsync();
                //Core.ReceiveJson(json);
            }

            return Json(new { });
        }
    }
}