using System.Threading.Tasks;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Common.Function.Application.Command
{
    [Name("发送自定义消息")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("支持发送任意格式的消息（包含cq码），支持群聊私聊")]
    [Command("send")]
    public class Send : CommandApp
    {
        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.PermissionLevel != PermissionLevel.Root)
                return null;

            string[] split = messageObj.Parameter.Split("|");
            if (split.Length == 1)
                return new CommonMessageResponse(Transform(messageObj.Parameter), messageObj);
            string innerUser = null, innerGroup = null, innerDiscuss = null, innerMessage = null;
            MessageType innerType = MessageType.Private;
            for (int i = 0; i < split.Length; i += 2)
            {
                if (i + 1 == split.Length)
                    return new CommonMessageResponse(split[i] + "缺少参数...", messageObj);
                string cmd = split[i], par = split[i + 1];
                switch (cmd)
                {
                    case "-u":
                        innerUser = par;
                        break;
                    case "-g" when innerDiscuss != null:
                        return new CommonMessageResponse("不能同时选择群和讨论组...", messageObj);
                    case "-g":
                        innerGroup = par;
                        innerType = MessageType.Group;
                        break;
                    case "-d" when innerGroup != null:
                        return new CommonMessageResponse("不能同时选择群和讨论组...", messageObj);
                    case "-d":
                        innerDiscuss = par;
                        innerType = MessageType.Discuss;
                        break;
                    case "-m":
                        innerMessage = Transform(par);
                        break;
                    default:
                        return new CommonMessageResponse("未知的参数: " + cmd + "...", messageObj);
                }
            }

            if (innerMessage == null)
                return new CommonMessageResponse("你还没有填写消息...", messageObj);

            SendMessage(new CommonMessageResponse(innerMessage, innerUser), innerGroup, innerDiscuss, innerType);

            return null;
        }

        private static string Transform(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
