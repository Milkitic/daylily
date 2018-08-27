using System;
using System.Collections.Generic;
using Daylily.Bot.Interface;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using Newtonsoft.Json;

namespace Daylily.Bot
{
    public class CoolQJsonDeserializer : IJsonDeserializer
    {
        public CoolQJsonDeserializer()
        {
            Core.JsonReceived += Json_Received;
        }

        public void Json_Received(object sender, JsonReceivedEventArgs args)
        {
            string json = args.JsonString;

            dynamic obj = JsonConvert.DeserializeObject(json);
            // 判断post类别
            try
            {
                if (obj.post_type == "message")
                {
                    // 私聊
                    if (obj.message_type == "private")
                    {
                        PrivateMsg parsedObj = JsonConvert.DeserializeObject<PrivateMsg>(json);
                        Core.ReceiveMessage(parsedObj);
                    }

                    // 群聊
                    else if (obj.message_type == "group")
                    {
                        GroupMsg parsedObj = JsonConvert.DeserializeObject<GroupMsg>(json);
                        Core.ReceiveMessage(parsedObj);
                    }

                    // 讨论组
                    else if (obj.message_type == "discuss")
                    {
                        DiscussMsg parsedObj = JsonConvert.DeserializeObject<DiscussMsg>(json);
                        Core.ReceiveMessage(parsedObj);
                    }
                }
                else if (obj.post_type == "notice")
                {
                    // 群文件上传
                    if (obj.notice_type == "group_upload")
                    {
                        GroupFileUpload parsedObj = JsonConvert.DeserializeObject<GroupFileUpload>(json);
                        // TODO
                    }
                    // 群管理员变动
                    else if (obj.notice_type == "group_admin")
                    {
                        GroupAdminChange parsedObj = JsonConvert.DeserializeObject<GroupAdminChange>(json);
                        // TODO
                    }
                    // 群成员增加/减少
                    else if (obj.notice_type == "group_decrease" || obj.notice_type == "group_increase")
                    {
                        GroupMemberChange parsedObj = JsonConvert.DeserializeObject<GroupMemberChange>(json);
                        // TODO
                    }
                    // 好友添加
                    else if (obj.notice_type == "friend_add")
                    {
                        FriendAdd parsedObj = JsonConvert.DeserializeObject<FriendAdd>(json);
                        // TODO
                    }
                }
                else if (obj.post_type == "request")
                {
                    // 加好友请求
                    if (obj.request_type == "friend")
                    {
                        FriendRequest parsedObj = JsonConvert.DeserializeObject<FriendRequest>(json);
                        // TODO，临时
                        CqApi.SendPrivateMessage("2241521134",
                            string.Format("{0} ({1})邀请加我为好友",
                                CqApi.GetStrangerInfo(parsedObj.UserId.ToString()).Data?.Nickname, parsedObj.UserId));
                    }
                    // 加群请求／邀请
                    else if (obj.request_type == "group")
                    {
                        GroupInvite parsedObj = JsonConvert.DeserializeObject<GroupInvite>(json);
                        // TODO，临时
                        if (parsedObj.SubType == "invite")
                        {
                            CqApi.SendPrivateMessage("2241521134",
                                string.Format("{0} ({1})邀请我加入群{2}",
                                    CqApi.GetStrangerInfo(parsedObj.UserId.ToString()).Data?.Nickname, parsedObj.UserId,
                                    parsedObj.GroupId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}
