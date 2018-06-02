using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Database.BLL;
using Daylily.Common.Database.Model;
using Daylily.Common.Interface;
using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    public class SetId : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (string.IsNullOrEmpty(message.Parameter))
                return null;
            BllUserRole bllUserRole = new BllUserRole();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(message.Parameter);

            if (userList.Length == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, message);

            OsuUser userObj = userList[0];
            var role = bllUserRole.GetUserRoleByQq(long.Parse(message.UserId));
            if (role.Count != 0)
            {
                if (role[0].CurrentUname.ToLower() == message.Parameter.ToLower())
                    return new CommonMessageResponse("我认识你，" + role[0].CurrentUname + ".", message, true);
                string msg = role[0].CurrentUname + "先森，别以为我不认识你哦. 嗯? 你真不是? 那请找Mother Ship吧..";
                return new CommonMessageResponse(msg, message, true);
            }
            var newRole = new TblUserRole
            {
                UserId = long.Parse(userObj.user_id),
                Role = "creep",
                QQ = long.Parse(message.UserId),
                LegacyUname = "[]",
                CurrentUname = userObj.username,
                IsBanned = false,
                RepeatCount = 0,
                SpeakingCount = 0,
                Mode = 0,
            };
            int c = bllUserRole.InsertUserRole(newRole);
            return c < 1 ?
                new CommonMessageResponse("由于各种强大的原因，绑定失败..", message) :
                new CommonMessageResponse("明白了，" + userObj.username + "，多好的名字呢.", message);
        }
    }
}
