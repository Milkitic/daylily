using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Abstract;
using Daylily.CoolQ.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Plugin;

namespace Daylily.Plugin.Basic
{
    [Name("发送自定义消息")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Stable)]
    [Help("支持发送任意格式的消息（包含cq码）。", Authority = Authority.Root)]
    [Command("send")]
    public class Send : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("09983821-0238-4b0d-b1c1-2921eb7e52d1");

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

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            string sessionId = null;
            var sessionType = MessageType.Private;
            if (routeMsg.CurrentAuthority != Authority.Root)
                return routeMsg.ToSource(DefaultReply.RootOnly);
            if (Message == null)
                return routeMsg.ToSource("你要说什么……");
            if (GroupId != null && DiscussId != null)
                return routeMsg.ToSource("不能同时选择群和讨论组……");

            string innerMessage = Decode(Message);
            if (UseAllGroup)
            {
                sessionType = MessageType.Group;
                List<GroupInfo> list = CoolQHttpApiClient.GetGroupList().Data;
                List<string> failedList = new List<string>();
                string ok = $"◈◈ {DateTime.Now:M月d日 H:mm}公告 ◈◈{Environment.NewLine}";
                string msg = ok + innerMessage;
                if (list?.Count > 0)
                    foreach (var groupInfo in list)
                    {
                        try
                        {
                            sessionId = groupInfo.GroupId.ToString();
                            SendMessage(new CoolQRouteMessage(msg, new CoolQIdentity(sessionId, sessionType)));
                            Thread.Sleep(3000);
                        }
                        catch
                        {
                            failedList.Add($"({groupInfo.GroupId}) {groupInfo.GroupName}");
                        }
                    }
                else
                    return routeMsg.ToSource("无有效群。");

                SaveLogs(msg, "announcement");
                if (failedList.Count == 0)
                    return routeMsg.ToSource("已成功发送至" + list.Count + "个群。");
                else
                    return routeMsg.ToSource(string.Format("有以下{0}个群未成功发送: {1}{2}", failedList.Count,
                        Environment.NewLine, string.Join(Environment.NewLine, failedList)));
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
                return routeMsg.ToSource(Decode(routeMsg.ArgString));

            SendMessage(new CoolQRouteMessage(innerMessage, new CoolQIdentity(sessionId, sessionType)));
            return null;
        }

        private static string Decode(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
