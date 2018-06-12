using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models.CQRequest;
using Daylily.Common.Models.CQResponse;
using Daylily.Web.Function;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Daylily.Web.Controllers
{
    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> GetResponse()
        {
            dynamic obj;
            var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (ip != "127.0.0.1" && ip != "74.120.171.198" && ip != "123.207.137.177")
            {
                Logger.DangerLine("来自未知ip的请求：" + ip);
                Response.StatusCode = 403;
                return Json(new { });
            }

            using (var sr = new StreamReader(Request.Body))
            {
                string json = await sr.ReadToEndAsync();
                System.IO.File.WriteAllText("json.txt", json);
                //Logger.WriteLine(json);
                obj = JsonConvert.DeserializeObject(json);
            }

            // 判断post类别
            if (obj.post_type == "message")
            {
                // 私聊
                if (obj.message_type == "private")
                {
                    PrivateMsg parsedObj = JsonConvert.DeserializeObject<PrivateMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler privateHandler = new MessageHandler(parsedObj);
                        //private_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex);
                        //PrivateMsgResponse privateResp = new PrivateMsgResponse()
                        //{
                        //    AutoEscape = false,
                        //    Reply = ex.Message
                        //};
                        //return Json(privateResp);
                    }
                }

                //群聊
                else if (obj.message_type == "group")
                {
                    GroupMsg parsedObj = JsonConvert.DeserializeObject<GroupMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler groupHandler = new MessageHandler(parsedObj);
                        //group_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex);
                        //GroupMsgResponse group_resp = new GroupMsgResponse()
                        //{
                        //    reply = ex.Message,
                        //    auto_escape = false,
                        //    at_sender = true,
                        //    delete = false,
                        //    kick = false,
                        //    ban = false
                        //};
                        //return Json(group_resp);
                    }
                }

                //讨论组
                else if (obj.message_type == "discuss")
                {
                    DiscussMsg parsedObj = JsonConvert.DeserializeObject<DiscussMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler discussHandler = new MessageHandler(parsedObj);
                        //group_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex);
                        //GroupMsgResponse group_resp = new GroupMsgResponse()
                        //{
                        //    reply = ex.Message,
                        //    auto_escape = false,
                        //    at_sender = true,
                        //    delete = false,
                        //    kick = false,
                        //    ban = false
                        //};
                        //return Json(group_resp);
                    }
                }
            }
            else if (obj.post_type == "event")
            {
                // TODO
            }
            else if (obj.post_type == "request")
            {
                // TODO
            }

            return Json(new { });
        }
    }
}