using DaylilyWeb.Database.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using DaylilyWeb.Database.Model;
namespace DaylilyWeb.Function.Application.Command
{
    public class SetId : Application
    {
        public override string Execute(string @params, string user, string group, bool isRoot, ref bool ifAt)
        {
            ifAt = true;
            if (@params == null)
                return null;
            BllUserRole bllUserRole = new BllUserRole();
            OsuClient osu = new OsuClient("7453fe3dda8da1a80bc69325dbef60e77b0676cf");
            OsuUser[] userList = osu.GetUser(@params);
            if (userList.Length == 0)
                return "官网里没有这个id";
            else
            {
                OsuUser userObj = userList[0];
                var role = bllUserRole.GetUserRoleByQQ(long.Parse(user));
                if (role.Count != 0)
                    return "你的qq已经绑了一个Id叫" + role[0].CurrentUname + "的玩家，请找Mother Ship解绑";
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
                    return "由于各种强大的原因，绑定失败";
                else
                    return "以后你就是" + userObj.username + "了";
            }
            return null;
        }
    }
}
