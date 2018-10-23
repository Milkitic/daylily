using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;

namespace Daylily.Plugin.Osu
{
    [Name("绑定id")]
    [Author("yf_extension")]
    [Version(0, 1, 1, PluginVersion.Stable)]
    [Help("绑定osu id至发送者QQ。")]
    [Command("setid")]
    public class SetId : CommandPlugin
    {
        [FreeArg]
        [Help("绑定指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            string osuId = Decode(OsuId);
            if (string.IsNullOrEmpty(osuId))
                return new CommonMessageResponse(LoliReply.ParamMissing, messageObj);

            BllUserRole bllUserRole = new BllUserRole();
            int userNum = OldSiteApi.GetUser(OsuId, out var userObj);
            if (userNum == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, messageObj, true);
            if (userNum > 1)
            {
                // ignored
            }

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
