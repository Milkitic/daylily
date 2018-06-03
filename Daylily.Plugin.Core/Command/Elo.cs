using System;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Interface;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    public class Elo : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.PermissionLevel == PermissionLevel.Public)
                return new CommonMessageResponse(LoliReply.AdminOnly, message, true);
            if (string.IsNullOrEmpty(message.Parameter))
                return new CommonMessageResponse(LoliReply.ParamMissing, message, true);

            EloApi eloApi = new EloApi();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(message.Parameter);
            if (userList.Length == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, message, true);

            var obj = eloApi.GetEloByUid(long.Parse(userList[0].user_id));
            switch (obj.Result.ToLower())
            {
                case "fail" when obj.Message.ToLower() == "unranked":
                    return new CommonMessageResponse(userList[0].username + "大概没有参加什么mapping赛事..所以没有数据..", message,
                        true);
                case "fail":
                    return new CommonMessageResponse("未知错误..查询不到..", message, true);
                default:
                    return new CommonMessageResponse(
                        $"{obj.User.Name}，有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.", message, true);
            }
        }
    }
}
