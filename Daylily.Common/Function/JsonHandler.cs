using System;
using System.Collections.Generic;
using Daylily.Common.Assist;
using Daylily.Common.Function.Dispatch;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.MessageList;
using Newtonsoft.Json;

namespace Daylily.Common.Function
{
    public static class JsonHandler
    {
        public static object HandleReportJson(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            // 判断post类别
            try
            {
                if (obj.post_type == "message")
                {
                    PrivateMsg parsed = null;
                    // 私聊
                    if (obj.message_type == "private")
                    {
                        PrivateMsg parsedObj = JsonConvert.DeserializeObject<PrivateMsg>(json);
                        parsed = JsonConvert.DeserializeObject<PrivateMsg>(json);
                        MessageHandler privateHandler = new MessageHandler(parsedObj);
                    }

                    // 群聊
                    else if (obj.message_type == "group")
                    {
                        GroupMsg parsedObj = JsonConvert.DeserializeObject<GroupMsg>(json);
                        parsed = JsonConvert.DeserializeObject<GroupMsg>(json);
                        MessageHandler groupHandler = new MessageHandler(parsedObj);
                    }

                    // 讨论组
                    else if (obj.message_type == "discuss")
                    {
                        DiscussMsg parsedObj = JsonConvert.DeserializeObject<DiscussMsg>(json);
                        parsed = JsonConvert.DeserializeObject<DiscussMsg>(json);
                        MessageHandler discussHandler = new MessageHandler(parsedObj);
                    }

                    Dispatcher dispatcher = new Dispatcher(new List<IMessageList>
                    {
                        new GroupList(),
                        new PrivateList(),
                        new DiscussList()

                    });
                    dispatcher.SendToBack(parsed);
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
                        // TODO
                    }
                    // 加群请求／邀请
                    else if (obj.request_type == "group")
                    {
                        GroupInvite parsedObj = JsonConvert.DeserializeObject<GroupInvite>(json);
                        // TODO
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            return null;
        }
    }
}
