using System;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Database.BLL;
using Daylily.Common.Database.Model;
using Daylily.Common.Interface;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("绑定id")]
    [Author("yf_extension")]
    [Version(0, 1, 1, PluginVersion.Stable)]
    [Help("绑定osu id至发送者QQ。")]
    [Command("setid")]
    public class SetId : CommandApp
    {
        [FreeArg]
        [Help("绑定指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            string osuId = Decode(OsuId);
            if (string.IsNullOrEmpty(osuId))
                return new CommonMessageResponse(LoliReply.ParamMissing, messageObj);

            BllUserRole bllUserRole = new BllUserRole();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(osuId);

            if (userList.Length == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, messageObj);

            OsuUser userObj = userList[0];
            var role = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
            if (role.Count != 0)
            {
                if (role[0].CurrentUname == userObj.username)
                    return new CommonMessageResponse("我早就认识你啦.", messageObj, true);
                string msg = role[0].CurrentUname + "，我早就认识你啦. 有什么问题请找Mother Ship（扔锅）";
                return new CommonMessageResponse(msg, messageObj, true);
            }

            var newRole = new TblUserRole
            {
                UserId = long.Parse(userObj.user_id),
                Role = "creep",
                QQ = long.Parse(messageObj.UserId),
                LegacyUname = "[]",
                CurrentUname = userObj.username,
                IsBanned = false,
                RepeatCount = 0,
                SpeakingCount = 0,
                Mode = 0,
            };
            int c = bllUserRole.InsertUserRole(newRole);
            return c < 1
                ? new CommonMessageResponse("由于各种强大的原因，绑定失败..", messageObj)
                : new CommonMessageResponse("明白了，" + userObj.username + "，多好的名字呢.", messageObj);
        }

        private static string Decode(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
                .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
                Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");
    }
}
