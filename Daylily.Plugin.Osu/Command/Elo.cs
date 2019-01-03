using System;
using System.Collections.Generic;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;

namespace Daylily.Plugin.Osu
{
    [Name("ELO查询")]
    [Author("yf_extension")]
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("获取玩家的ELO信息。", "Mapping ELO Project 是以 ELO 等级分制度 来对选手在作图竞赛中的表现进行量化衡量以及排名的一个项目。", "详情：t/728158")]
    [Command("elo")]
    public class Elo : CommandPlugin
    {
        [FreeArg]
        [Help("查询指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override void OnInitialized(string[] args)
        {

        }

        public override CommonMessageResponse OnMessageReceived(CommonMessage messageObj)
        {
            string id;
            string uname;
            if (OsuId == null)
            {
                BllUserRole bllUserRole = new BllUserRole();
                List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
                if (userInfo.Count == 0)
                    return new CommonMessageResponse(LoliReply.IdNotBound, messageObj, true);

                id = userInfo[0].UserId.ToString();
                uname = userInfo[0].CurrentUname;
            }
            else
            {
                int userNum = OldSiteApi.GetUser(OsuId, out var userObj);
                if (userNum == 0)
                    return new CommonMessageResponse(LoliReply.IdNotFound, messageObj, true);
                if (userNum > 1)
                {
                    // ignored
                }

                id = userObj.user_id;
                uname = userObj.username;
            }

            var eloInfo = EloApi.GetEloByUid(long.Parse(id));
            switch (eloInfo.Result.ToLower())
            {
                case "fail" when eloInfo.Message.ToLower() == "unranked":
                    return new CommonMessageResponse(uname + "大概没有参加什么mapping赛事..所以没有数据..", messageObj,
                        true);
                case "fail":
                    return new CommonMessageResponse("未知错误..查询不到..", messageObj, true);
                default:
                    return new CommonMessageResponse(
                        $"{eloInfo.User.Name}，有elo点{Math.Round(eloInfo.User.Elo, 2)}，当前#{eloInfo.User.Ranking}.", messageObj, true);
            }
        }
    }
}
