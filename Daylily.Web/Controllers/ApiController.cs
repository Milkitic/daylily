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
            using (var sr = new StreamReader(Request.Body))
            {
                string json = await sr.ReadToEndAsync();
                //Logger.WriteLine(json);
                obj = JsonConvert.DeserializeObject(json);
            }
            // 判断post类别
            if (obj.post_type == "message")
            {
                // 私聊
                if (obj.message_type == "private")
                {
                    PrivateMsg parsed_obj = JsonConvert.DeserializeObject<PrivateMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler private_handler = new MessageHandler(parsed_obj);
                        //private_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        PrivateMsgResponse private_resp = new PrivateMsgResponse()
                        {
                            AutoEscape = false,
                            Reply = ex.Message
                        };
                        return Json(private_resp);
                    }
                }

                //群聊
                else if (obj.message_type == "group")
                {
                    GroupMsg parsed_obj = JsonConvert.DeserializeObject<GroupMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler group_handler = new MessageHandler(parsed_obj);
                        //group_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            Logger.DangerLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace.Replace("\r", "").Split('\n')[0]);
                        else
                            Logger.DangerLine(ex.Message + Environment.NewLine + ex.StackTrace.Replace("\r", "").Split('\n')[0]);
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
                    DiscussMsg parsed_obj = JsonConvert.DeserializeObject<DiscussMsg>(JsonConvert.SerializeObject(obj));
                    try
                    {
                        MessageHandler group_handler = new MessageHandler(parsed_obj);
                        //group_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            Logger.DangerLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                        else
                            Logger.DangerLine(ex.Message + Environment.NewLine + ex.StackTrace);
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
                // todo
            }
            else if (obj.post_type == "request")
            {
                // todo
            }
            return Json(new { });
        }
    }
}