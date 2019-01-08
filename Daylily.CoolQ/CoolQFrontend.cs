using Daylily.Bot;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Frontend;
using Daylily.Common;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Newtonsoft.Json;
using System;

namespace Daylily.CoolQ
{
    public class CoolQFrontend : IFrontend
    {
        public MiddlewareConfig MiddlewareConfig { get; set; } = new MiddlewareConfig();

        public event MessageEventHandler MessageReceived;
        public event MessageEventHandler PrivateMessageReceived;
        public event MessageEventHandler GroupMessageReceived;
        public event MessageEventHandler DiscussMessageReceived;

        public event NoticeEventHandler Noticed;
        public event NoticeEventHandler GroupFileUploaded;
        public event NoticeEventHandler GroupAdminChanged;
        public event NoticeEventHandler GroupMemberChanged;
        public event NoticeEventHandler FriendAdded;

        public event RequestEventHandler Requested;
        public event RequestEventHandler FriendRequested;
        public event RequestEventHandler GroupRequested;

        public event EventEventHandler EventReceived;
        public event ExceptionEventHandler ErrorOccured;

        public bool RawObject_Received(object rawObject)
        {
            var handled = false;
            if (!(rawObject is string rawJson))
                return handled;

            dynamic obj = JsonConvert.DeserializeObject(rawJson);
            try
            {
                if (obj.post_type == "message")
                {
                    if (obj.message_type == "private") // 私聊
                    {
                        CoolQPrivateMessageApi parsedObj = JsonConvert.DeserializeObject<CoolQPrivateMessageApi>(rawJson);
                        var arg = new MessageEventArgs(parsedObj);
                        PrivateMessageReceived?.Invoke(this, arg);
                        MessageReceived?.Invoke(this, arg);
                    }
                    else if (obj.message_type == "group") // 群聊
                    {
                        CoolQGroupMessageApi parsedObj = JsonConvert.DeserializeObject<CoolQGroupMessageApi>(rawJson);
                        var arg = new MessageEventArgs(parsedObj);
                        DiscussMessageReceived?.Invoke(this, arg);
                        MessageReceived?.Invoke(this, arg);
                    }
                    else if (obj.message_type == "discuss") // 讨论组
                    {
                        CoolQDiscussMessageApi parsedObj = JsonConvert.DeserializeObject<CoolQDiscussMessageApi>(rawJson);
                        var arg = new MessageEventArgs(parsedObj);
                        GroupMessageReceived?.Invoke(this, arg);
                        MessageReceived?.Invoke(this, arg);
                    }
                }
                else
                {
                    EventReceived?.Invoke(this, new EventEventArgs(rawJson));
                    if (obj.post_type == "notice")
                    {
                        if (obj.notice_type == "group_upload") // 群文件上传
                        {
                            GroupFileUpload parsedObj = JsonConvert.DeserializeObject<GroupFileUpload>(rawJson);
                            var arg = new NoticeEventArgs(parsedObj);
                            GroupFileUploaded?.Invoke(this, arg);
                            Noticed?.Invoke(this, arg);
                        }
                        else if (obj.notice_type == "group_admin") // 群管理员变动
                        {
                            GroupAdminChange parsedObj = JsonConvert.DeserializeObject<GroupAdminChange>(rawJson);
                            var arg = new NoticeEventArgs(parsedObj);
                            GroupAdminChanged?.Invoke(this, arg);
                            Noticed?.Invoke(this, arg);
                        }
                        else if (obj.notice_type == "group_decrease" || obj.notice_type == "group_increase") // 群成员增加/减少
                        {
                            GroupMemberChange parsedObj = JsonConvert.DeserializeObject<GroupMemberChange>(rawJson);
                            var arg = new NoticeEventArgs(parsedObj);
                            Noticed?.Invoke(this, arg);
                            GroupMemberChanged?.Invoke(this, arg);
                        }
                        else if (obj.notice_type == "friend_add") // 好友添加
                        {
                            FriendAdd parsedObj = JsonConvert.DeserializeObject<FriendAdd>(rawJson);
                            var arg = new NoticeEventArgs(parsedObj);
                            FriendAdded?.Invoke(this, arg);
                            Noticed?.Invoke(this, arg);
                        }
                    }
                    else if (obj.post_type == "request")
                    {
                        if (obj.request_type == "friend") // 加好友请求
                        {
                            FriendRequest parsedObj = JsonConvert.DeserializeObject<FriendRequest>(rawJson);
                            var arg = new RequestEventArgs(parsedObj);
                            FriendRequested?.Invoke(this, arg);
                            Requested?.Invoke(this, arg);

                            //// TODO，临时
                            //CqApi.SendPrivateMessage("2241521134",
                            //    string.Format("{0} ({1})邀请加我为好友",
                            //        CqApi.GetStrangerInfo(parsedObj.UserId.ToString()).Data?.Nickname, parsedObj.UserId));
                        }
                        else if (obj.request_type == "group") // 加群请求／邀请
                        {
                            GroupInvite parsedObj = JsonConvert.DeserializeObject<GroupInvite>(rawJson);
                            var arg = new RequestEventArgs(parsedObj);
                            GroupRequested?.Invoke(this, arg);
                            Requested?.Invoke(this, arg);

                            //// TODO，临时
                            //if (parsedObj.SubType == "invite")
                            //{
                            //    CqApi.SendPrivateMessage("2241521134",
                            //        string.Format("{0} ({1})邀请我加入群{2}",
                            //            CqApi.GetStrangerInfo(parsedObj.UserId.ToString()).Data?.Nickname, parsedObj.UserId,
                            //            parsedObj.GroupId));
                            //}
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, new ExceptionEventArgs(ex));
            }

            return handled;
        }
    }
}
