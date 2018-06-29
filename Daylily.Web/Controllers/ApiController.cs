using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Function;
using Daylily.Common.Models.CQRequest;
using Daylily.Common.Models.CQResponse;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Daylily.Web.Controllers
{
    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> GetResponse()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (ip != "127.0.0.1" && ip != "74.120.171.198" && ip != "123.207.137.177")
            {
                Logger.DangerLine("来自未知ip的请求：" + ip);
                //Response.StatusCode = 403;
                return Json(new { });
            }

            string json;
            using (var sr = new StreamReader(Request.Body))
            {
                json = await sr.ReadToEndAsync();
            }

            object ret = JsonHandler.HandleReportJson(json);
            // 判断post类别

            return Json(ret ?? new { });
        }
    }
}