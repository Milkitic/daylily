using DaylilyWeb.Database.BLL;
using DaylilyWeb.Interface.Elo;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application.Command
{
    public class MyElo : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            BllUserRole bllUserRole = new BllUserRole();
            EloApi eloApi = new EloApi();
            var user_info = bllUserRole.GetUserRoleByQQ(long.Parse(user));
            ifAt = true;
            if (user_info.Count == 0)
                return "还不知道你是哪位呢..用setid命令?";
            var obj = eloApi.GetEloByUid(user_info[0].UserId);
            if (obj.Result.ToLower() == "fail")
            {
                if (obj.Message.ToLower() == "unranked")
                {
                    return "大概你没有参加什么mapping赛事..所以这里没有数据呢..";
                }
                return "未知错误..查询不到..";
            }
            else
                return $"{obj.User.Name}，你有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.";
        }
    }
}
