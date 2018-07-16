using System;
using System.Threading.Tasks;
using Daylily.Common.Database.BLL;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("ELO查询")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("获取自己的elo信息")]
    //[Command] // 弃用，晚些整合
    public class MyElo : CommandApp
    {
        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            BllUserRole bllUserRole = new BllUserRole();
            EloApi eloApi = new EloApi();

            var userInfo = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
            if (userInfo.Count == 0)
                return new CommonMessageResponse(LoliReply.IdNotBound, messageObj, true);

            var obj = eloApi.GetEloByUid(userInfo[0].UserId);
            if (obj.Result.ToLower() == "fail")
            {
                return obj.Message.ToLower() == "unranked"
                    ? new CommonMessageResponse("大概你没有参加什么mapping赛事..所以这里没有数据呢..", messageObj, true)
                    : new CommonMessageResponse("未知错误..查询不到..", messageObj, true);
            }

            return new CommonMessageResponse($"{obj.User.Name}，你有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.",
                messageObj, true);
        }
    }
}
