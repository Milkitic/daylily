using System;
using System.Collections.Generic;
using System.Threading;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse.Api;

namespace Daylily.Plugin.Core.Command
{
    [Name("发送自定义消息")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("支持发送任意格式的消息（包含cq码）。", HelpType = PermissionLevel.Root)]
    [Command("send")]
    public class Send : CommandPlugin
    {
        [Arg("g")]
        [Help("要发送的群号。")]
        public string GroupId { get; set; }
        [Arg("d")]
        [Help("要发送的讨论组号。")]
        public string DiscussId { get; set; }
        [Arg("u")]
        [Help("要发送的用户QQ号。")]
        public string UserId { get; set; }
        [Arg("all-group", IsSwitch = true)]
        [Help("给所有群发送（公告）。")]
        public bool UseAllGroup { get; set; }
        [FreeArg]
        [Help("要发送的信息。")]
        public string Message { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            string sessionId = null;
            var sessionType = MessageType.Private;
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj);
            if (Message == null)
                return new CommonMessageResponse("你要说什么……", messageObj);
            if (GroupId != null && DiscussId != null)
                return new CommonMessageResponse("不能同时选择群和讨论组……", messageObj);

            string innerMessage = Decode(Message);
            if (UseAllGroup)
            {
                sessionType = MessageType.Group;
                List<GroupInfo> list = CqApi.GetGroupList().Data;
                List<string> failedList = new List<string>();
                string ok = $"◈◈ {DateTime.Now:M月d日 H:mm}公告 ◈◈{Environment.NewLine}";
                string msg = ok + innerMessage;
                if (list?.Count > 0)
                    foreach (var groupInfo in list)
                    {
                        try
                        {
                            sessionId = groupInfo.GroupId.ToString();
                            SendMessage(new CommonMessageResponse(msg, new Identity(sessionId, sessionType)));
                            Thread.Sleep(3000);
                        }
                        catch
                        {
                            failedList.Add($"({groupInfo.GroupId}) {groupInfo.GroupName}");
                        }
                    }
                else
                    return new CommonMessageResponse("无有效群。", messageObj);

                SaveLogs(msg, "announcement");
                if (failedList.Count == 0)
                    return new CommonMessageResponse("已成功发送至" + list.Count + "个群。", messageObj);
                else
                    return new CommonMessageResponse(string.Format("有以下{0}个群未成功发送: {1}{2}", failedList.Count,
                        Environment.NewLine, string.Join(Environment.NewLine, failedList)), messageObj);
            }
            if (DiscussId != null)
            {
                sessionId = DiscussId;
                sessionType = MessageType.Discuss;
            }
            else if (GroupId != null)
            {
                sessionId = GroupId;
                sessionType = MessageType.Group;
            }
            if (UserId != null)
                sessionId = UserId;

            if (DiscussId == null && GroupId == null && UserId == null)
                return new CommonMessageResponse(Decode(messageObj.ArgString), messageObj);

            SendMessage(new CommonMessageResponse(innerMessage, new Identity(sessionId, sessionType)));
            return null;
        }

        private static string Decode(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
