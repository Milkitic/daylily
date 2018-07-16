﻿using System;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Common.Database.BLL;
using Daylily.Common.Database.Model;
using Daylily.Common.Interface;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;

namespace Daylily.Plugin.Core.Command
{
    [Name("绑定id")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("绑定osu id")]
    [Command("setid")]
    public class SetId : CommandApp
    {
        public override void Initialize(string[] args)
        {

        }

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            if (string.IsNullOrEmpty(messageObj.ArgString))
                return null;
            BllUserRole bllUserRole = new BllUserRole();
            OsuClient osu = new OsuClient(OsuApi.ApiKey);
            OsuUser[] userList = osu.GetUser(messageObj.ArgString);

            if (userList.Length == 0)
                return new CommonMessageResponse(LoliReply.IdNotFound, messageObj);

            OsuUser userObj = userList[0];
            var role = bllUserRole.GetUserRoleByQq(long.Parse(messageObj.UserId));
            if (role.Count != 0)
            {
                if (role[0].CurrentUname.ToLower() == messageObj.ArgString.ToLower())
                    return new CommonMessageResponse("我认识你，" + role[0].CurrentUname + ".", messageObj, true);
                string msg = role[0].CurrentUname + "先森，别以为我不认识你哦. 嗯? 你真不是? 那请找Mother Ship吧..";
                return new CommonMessageResponse(msg, messageObj, true);
            }

            var newRole = new TblUserRole
            {
                UserId = long.Parse(userObj.user_id),
                Role = "creep",
                QQ = long.Parse(messageObj.UserId),
                LegacyUname = "[]",
                CurrentUname = userObj.username,
                IsBanned = false,
                RepeatCount = 0,
                SpeakingCount = 0,
                Mode = 0,
            };
            int c = bllUserRole.InsertUserRole(newRole);
            return c < 1
                ? new CommonMessageResponse("由于各种强大的原因，绑定失败..", messageObj)
                : new CommonMessageResponse("明白了，" + userObj.username + "，多好的名字呢.", messageObj);
        }
    }
}
