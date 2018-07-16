using System;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Interface;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("ELO查询")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("获取玩家的elo信息")]
    [Command("elo")]
    public class Elo : CommandApp
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (messageObj.PermissionLevel == PermissionLevel.Public)
                return new CommonMessageResponse(LoliReply.AdminOnly, messageObj, true);
            if (string.IsNullOrEmpty(messageObj.ArgString))
                return new CommonMessageResponse(LoliReply.ParamMissing, messageObj, true);

            EloApi eloApi = new EloApi();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(messageObj.ArgString);
            if (userList.Length == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, messageObj, true);

            var obj = eloApi.GetEloByUid(long.Parse(userList[0].user_id));
            switch (obj.Result.ToLower())
            {
                case "fail" when obj.Message.ToLower() == "unranked":
                    return new CommonMessageResponse(userList[0].username + "大概没有参加什么mapping赛事..所以没有数据..", messageObj,
                        true);
                case "fail":
                    return new CommonMessageResponse("未知错误..查询不到..", messageObj, true);
                default:
                    return new CommonMessageResponse(
                        $"{obj.User.Name}，有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.", messageObj, true);
            }
        }
    }
}
