using DaylilyWeb.Assist;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CQResponse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application.Command
{
    public class Send : Application
    {
        HttpApi CQApi = new HttpApi();

        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            if (currentLevel != PermissionLevel.Root)
                return null;

            string[] split = @params.Split("|");
            if (split.Length == 1)
                return Transform(@params);
            else
            {
                string inner_user = null, inner_group = null, inner_discuss = null, inner_message = null;

                for (int i = 0; i < split.Length; i += 2)
                {
                    if (i + 1 == split.Length)
                        return split[i] + "缺少参数...";
                    string cmd = split[i], par = split[i + 1];
                    if (cmd == "-u")
                        inner_user = par;
                    else if (cmd == "-g")
                    {
                        if (inner_discuss != null)
                            return "不能同时选择群和讨论组...";
                        inner_group = par;
                    }
                    else if (cmd == "-d")
                    {
                        if (inner_group != null)
                            return "不能同时选择群和讨论组...";
                        inner_discuss = par;
                    }
                    else if (cmd == "-m")
                    {
                        inner_message = Transform(par);
                    }
                    else
                        return "未知的参数: " + cmd + "...";
                }
                if (inner_message == null)
                    return "你还没有填写消息...";

                if (inner_group != null)
                {
                    SendGroupMsgResponse msg = CQApi.SendGroupMessageAsync(inner_group, inner_message).Result;
                    Logger.InfoLine($"我: {CQCode.Decode(inner_message)} {{status: {msg.Status}}})");
                }
                else if (inner_discuss != null)
                {
                    SendDiscussMsgResponse msg = CQApi.SendDiscussMessageAsync(inner_discuss, inner_message).Result;
                    Logger.InfoLine($"我: {CQCode.Decode(inner_message)} {{status: {msg.Status}}})");
                }
                else if (inner_user != null)
                {
                    SendPrivateMsgResponse msg = CQApi.SendPrivateMessageAsync(inner_user, inner_message).Result;
                    Logger.InfoLine($"我: {CQCode.Decode(inner_message)} {{status: {msg.Status}}})");
                }
            }

            return null;
        }

        private string Transform(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
            .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
            Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");
    }
}
