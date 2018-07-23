using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application.Command
{
    [Name("发送自定义消息")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("支持发送任意格式的消息（包含cq码），支持群聊私聊")]
    [Command("send")]
    public class Send : CommandApp
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
        [FreeArg]
        [Help("要发送的信息。")]
        public string Message { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            string innerUser = null, innerGroup = null, innerDiscuss = null;
            var innerType = MessageType.Private;
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return new CommonMessageResponse(LoliReply.RootOnly, messageObj);
            if (Message == null)
                return new CommonMessageResponse("你要说什么……", messageObj);
            if (GroupId != null && DiscussId != null)
                return new CommonMessageResponse("不能同时选择群和讨论组……", messageObj);

            string innerMessage = Transform(Message);
            if (DiscussId != null)
            {
                innerDiscuss = DiscussId;
                innerType = MessageType.Discuss;
            }
            else if (GroupId != null)
            {
                innerGroup = GroupId;
                innerType = MessageType.Group;
            }
            if (UserId != null)
                innerUser = UserId;

            if (DiscussId == null && GroupId == null && UserId == null)
                return new CommonMessageResponse(Transform(messageObj.ArgString), messageObj);

            SendMessage(new CommonMessageResponse(innerMessage, innerUser), innerGroup, innerDiscuss, innerType);
            return null;
        }

        private static string Transform(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
