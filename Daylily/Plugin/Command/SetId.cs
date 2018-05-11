using Daylily.Database.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Database.Model;
using Daylily.Interface;
using Daylily.Models;

namespace Daylily.Plugin.Command
{
    public class SetId : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId)
        {
            ifAt = true;
            if (@params == null)
                return null;
            BllUserRole bllUserRole = new BllUserRole();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(@params);
            if (userList.Length == 0)
                return "官网没找到..不要乱填啊";
            else
            {
                OsuUser userObj = userList[0];
                var role = bllUserRole.GetUserRoleByQQ(long.Parse(user));
                if (role.Count != 0)
                {
                    if (role[0].CurrentUname.ToLower() == @params.ToLower())
                        return "我认识你，" + role[0].CurrentUname + ".";
                    return role[0].CurrentUname + "先森，别以为我不认识你哦. 嗯? 你真不是? 那请找Mother Ship吧..";
                }
                var newRole = new TblUserRole
                {
                    UserId = long.Parse(userObj.user_id),
                    Role = "creep",
                    QQ = long.Parse(user),
                    LegacyUname = "[]",
                    CurrentUname = userObj.username,
                    IsBanned = false,
                    RepeatCount = 0,
                    SpeakingCount = 0,
                    Mode = 0,
                };
                int c = bllUserRole.InsertUserRole(newRole);
                if (c < 1)
                    return "由于各种强大的原因，绑定失败..";
                else
                    return "明白了，" + userObj.username + "，多好的名字呢.";
            }
        }
    }
}
