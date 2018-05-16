using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Interface;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class Elo : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.PermissionLevel == PermissionLevel.Public)
                return new CommonMessageResponse("只有本群管理才能发动此技能…", message, true);
            EloApi eloApi = new EloApi();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(message.Parameter);
            if (userList.Length == 0)
                return new CommonMessageResponse("官网没找到..不要乱填啊", message, true);

            var obj = eloApi.GetEloByUid(long.Parse(userList[0].user_id));
            if (obj.Result.ToLower() == "fail")
            {
                if (obj.Message.ToLower() == "unranked")
                {
                    return new CommonMessageResponse(userList[0].username + "大概没有参加什么mapping赛事..所以没有数据..", message, true);
                }
                return new CommonMessageResponse("未知错误..查询不到..", message, true);
            }
            else
                return new CommonMessageResponse($"{obj.User.Name}，有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.", message, true);
        }
    }
}
