using System;
using Daylily.Common.Database.BLL;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    public class MyElo : AppConstruct
    {
        public override string Name => "ELO查询";
        public override string Author => "yf_extension";
        public override PluginVersion Version => PluginVersion.Stable;
        public override string VersionNumber => "1.0";
        public override string Description => "获取自己的elo信息";
        public override string Command => null; // 弃用，晚些整合
        public override AppType AppType => AppType.Command;

        public override void OnLoad(string[] args)
        {
            throw new NotImplementedException();
        }

        public override CommonMessageResponse OnExecute(CommonMessage message)
        {
            BllUserRole bllUserRole = new BllUserRole();
            EloApi eloApi = new EloApi();

            var userInfo = bllUserRole.GetUserRoleByQq(long.Parse(message.UserId));
            if (userInfo.Count == 0)
                return new CommonMessageResponse(LoliReply.IdNotBound, message, true);

            var obj = eloApi.GetEloByUid(userInfo[0].UserId);
            if (obj.Result.ToLower() == "fail")
            {
                return obj.Message.ToLower() == "unranked"
                    ? new CommonMessageResponse("大概你没有参加什么mapping赛事..所以这里没有数据呢..", message, true)
                    : new CommonMessageResponse("未知错误..查询不到..", message, true);
            }

            return new CommonMessageResponse($"{obj.User.Name}，你有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.",
                message, true);
        }
    }
}
