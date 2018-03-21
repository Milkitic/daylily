using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DaylilyWeb.Models.CQRequest;
using DaylilyWeb.Models.CQResponse;
using DaylilyWeb.Functions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DaylilyWeb.Assist;

namespace DaylilyWeb.Controllers
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
                obj = JsonConvert.DeserializeObject(json);
                Log.SuccessLine(json,ToString(), "GetResponse()");
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
                        MsgHandler private_handler = new MsgHandler(parsed_obj);
                        //private_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        PrivateMsgResponse private_resp = new PrivateMsgResponse()
                        {
                            auto_escape = false,
                            reply = ex.Message
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
                        MsgHandler group_handler = new MsgHandler(parsed_obj);
                        //group_handler.HandleMessage();
                    }
                    catch (Exception ex)
                    {
                        GroupMsgResponse group_resp = new GroupMsgResponse()
                        {
                            reply = ex.Message,
                            auto_escape = false,
                            at_sender = true,
                            delete = false,
                            kick = false,
                            ban = false
                        };
                        return Json(group_resp);
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