using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else
            {
                string inner_user = null, inner_group = null, inner_discuss = null, inner_message = null;

                for (int i = 0; i < split.Length; i += 2)
                {
                    if (i + 1 == split.Length)
                        return new CommonMessageResponse(split[i] + "缺少参数...", message);
                    string cmd = split[i], par = split[i + 1];
                    if (cmd == "-u")
                        inner_user = par;
                    else if (cmd == "-g")
                    {
                        if (inner_discuss != null)
                            return new CommonMessageResponse("不能同时选择群和讨论组...", message);
                        inner_group = par;
                    }
                    else if (cmd == "-d")
                    {
                        if (inner_group != null)
                            return new CommonMessageResponse("不能同时选择群和讨论组...", message);
                        inner_discuss = par;
                    }
                    else if (cmd == "-m")
                    {
                        inner_message = Transform(par);
                    }
                    else
                        return new CommonMessageResponse("未知的参数: " + cmd + "...", message);
                }
                if (inner_message == null)
                    return new CommonMessageResponse("你还没有填写消息...", message);

                SendMessage(new CommonMessageResponse(inner_message, message.UserId, true), message.GroupId, message.DiscussId, message.MessageType);
            }

            return null;
        }

        private string Transform(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");

    }
}
