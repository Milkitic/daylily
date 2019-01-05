using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;
using System;

namespace Daylily.Plugin.Osu
{
    [Name("绑定id")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("绑定osu id至发送者QQ。")]
    [Command("setid")]
    public class SetId : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("7c844f99-d416-4a9d-99b8-9b7c575bac93");

        [FreeArg]
        [Help("绑定指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            string osuId = Decode(OsuId);
            if (string.IsNullOrEmpty(osuId))
                return routeMsg.ToSource(DefaultReply.ParamMissing);

            BllUserRole bllUserRole = new BllUserRole();
            int userNum = OldSiteApi.GetUser(OsuId, out var userObj);
            if (userNum == 0)
                return routeMsg.ToSource(DefaultReply.IdNotFound, true);
            if (userNum > 1)
            {
                // ignored
            }

            var role = bllUserRole.GetUserRoleByQq(long.Parse(routeMsg.UserId));
            if (role.Count != 0)
            {
                if (role[0].CurrentUname == userObj.username)
                    return routeMsg.ToSource("我早就认识你啦.", true);
                string msg = role[0].CurrentUname + "，我早就认识你啦. 有什么问题请找Mother Ship（扔锅）";
                return routeMsg.ToSource(msg, true);
            }

            var newRole = new TblUserRole
            {
                UserId = long.Parse(userObj.user_id),
                Role = "creep",
                QQ = long.Parse(routeMsg.UserId),
                LegacyUname = "[]",
                CurrentUname = userObj.username,
                IsBanned = false,
                RepeatCount = 0,
                SpeakingCount = 0,
                Mode = 0,
            };
            int c = bllUserRole.InsertUserRole(newRole);
            return c < 1
                ? routeMsg.ToSource("由于各种强大的原因，绑定失败..")
                : routeMsg.ToSource("明白了，" + userObj.username + "，多好的名字呢.");
        }

        private static string Decode(string source) =>
            source.Replace("\\&amp;", "&tamp;").Replace("\\#91;", "&t#91;").Replace("\\&#93;", "&t#93;").Replace("\\&#44;", "&t#44;")
                .Replace("&amp;", "&").Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").
                Replace("&tamp;", "&amp;").Replace("&t#91;", "&#91;").Replace("&t#93;", "&#93;").Replace("&t#44;", "&#44;");
    }
}
