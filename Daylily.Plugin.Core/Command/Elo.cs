using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Database.BLL;
using Daylily.Common.Database.Model;
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
    [Version(0, 1, 0, PluginVersion.Stable)]
    [Help("获取玩家的ELO信息。", "Mapping ELO Project 是以 ELO 等级分制度 来对选手在作图竞赛中的表现进行量化衡量以及排名的一个项目。", "详情：t/728158")]
    [Command("elo")]
    public class Elo : CommandApp
    {
        [FreeArg]
        [Help("查询指定的osu用户名。若带空格，请使用引号。")]
        public string OsuId { get; set; }

        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
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
                OsuClient osu = new OsuClient(OsuApi.ApiKey);
                OsuUser[] userList = osu.GetUser(OsuId);
                if (userList.Length == 0)
                    return new CommonMessageResponse(LoliReply.IdNotFound, messageObj, true);

                OsuUser userObj = userList[0];
                id = userObj.user_id;
                uname = userObj.username;
            }

            EloApi eloApi = new EloApi();
            var obj = eloApi.GetEloByUid(long.Parse(id));
            switch (obj.Result.ToLower())
            {
                case "fail" when obj.Message.ToLower() == "unranked":
                    return new CommonMessageResponse(uname + "大概没有参加什么mapping赛事..所以没有数据..", messageObj,
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
