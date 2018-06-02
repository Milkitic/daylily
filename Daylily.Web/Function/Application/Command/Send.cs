using Daylily.Common.Models;

namespace Daylily.Web.Function.Application.Command
{
    public class Send : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.PermissionLevel != PermissionLevel.Root)
                return null;

            string[] split = message.Parameter.Split("|");
            if (split.Length == 1)
                return new CommonMessageResponse(Transform(message.Message), message);
            string innerUser = null, innerGroup = null, innerDiscuss = null, innerMessage = null;
            MessageType innerType = MessageType.Private;
            for (int i = 0; i < split.Length; i += 2)
            {
                if (i + 1 == split.Length)
                    return new CommonMessageResponse(split[i] + "缺少参数...", message);
                string cmd = split[i], par = split[i + 1];
                switch (cmd)
                {
                    case "-u":
                        innerUser = par;
                        break;
                    case "-g" when innerDiscuss != null:
                        return new CommonMessageResponse("不能同时选择群和讨论组...", message);
                    case "-g":
                        innerGroup = par;
                        innerType = MessageType.Group;
                        break;
                    case "-d" when innerGroup != null:
                        return new CommonMessageResponse("不能同时选择群和讨论组...", message);
                    case "-d":
                        innerDiscuss = par;
                        innerType = MessageType.Discuss;
                        break;
                    case "-m":
                        innerMessage = Transform(par);
                        break;
                    default:
                        return new CommonMessageResponse("未知的参数: " + cmd + "...", message);
                }
            }

            if (innerMessage == null)
                return new CommonMessageResponse("你还没有填写消息...", message);

            SendMessage(new CommonMessageResponse(innerMessage, innerUser, true), innerGroup, innerDiscuss, innerType);

            return null;
        }

        private static string Transform(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
