using Daylily.Common.Database.BLL;
using Daylily.Common.Interface.Elo;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
{
    public class MyElo : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            BllUserRole bllUserRole = new BllUserRole();
            EloApi eloApi = new EloApi();
            var user_info = bllUserRole.GetUserRoleByQQ(long.Parse(message.UserId));
            if (user_info.Count == 0)
                return new CommonMessageResponse("还不知道你是哪位呢..用setid命令?", message, true);
            var obj = eloApi.GetEloByUid(user_info[0].UserId);
            if (obj.Result.ToLower() == "fail")
            {
                if (obj.Message.ToLower() == "unranked")
                    return new CommonMessageResponse("大概你没有参加什么mapping赛事..所以这里没有数据呢..", message, true);
                return new CommonMessageResponse("未知错误..查询不到..", message, true);
            }
            else
                return new CommonMessageResponse($"{obj.User.Name}，你有elo点{Math.Round(obj.User.Elo)}，当前#{obj.User.Ranking}.", message, true);
        }
    }
}
