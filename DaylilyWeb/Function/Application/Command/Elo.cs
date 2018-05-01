using CSharpOsu;
using CSharpOsu.Module;
using DaylilyWeb.Database.BLL;
using DaylilyWeb.Interface;
using DaylilyWeb.Interface.Elo;
using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application.Command
{
    public class Elo : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            CurrentLevel = currentLevel;
            ifAt = true;
            if (CurrentLevel == PermissionLevel.Public)
                return "只有本群管理才能发动此技能…";
            EloApi eloApi = new EloApi();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(@params);
            if (userList.Length == 0)
                return "官网没找到..不要乱填啊";

            var obj = eloApi.GetEloByUid(long.Parse(userList[0].user_id));
            if (obj.Result.ToLower() == "fail")
            {
                if (obj.Message.ToLower() == "unranked")
                {
                    return userList[0].username + "大概没有参加什么mapping赛事..所以没有数据..";
                }
                return "未知错误..查询不到..";
            }
            else
                return $"{obj.User.Name}，有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.";


        }
    }
}
