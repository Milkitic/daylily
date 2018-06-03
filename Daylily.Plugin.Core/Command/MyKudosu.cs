using System;
using System.Collections.Generic;
using Daylily.Common.Assist;
using Daylily.Common.Database.BLL;
using Daylily.Common.Models;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Newtonsoft.Json;

namespace Daylily.Plugin.Core.Command
{
    public class MyKudosu : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            //if (group != null) // 不给予群聊权限
            //    return null;
            BllUserRole bllUserRole = new BllUserRole();
            var userInfo = bllUserRole.GetUserRoleByQq(long.Parse(message.UserId));

            if (userInfo.Count == 0)
                return new CommonMessageResponse(LoliReply.IdNotBound, message, true);
            List<KudosuInfo> totalList = new List<KudosuInfo>();
            const int page = 0;
            const int count = 20;
            //do
            //{
            string json = WebRequestHelper.GetResponseString(WebRequestHelper.CreateGetHttpResponse(
                "https://osu.ppy.sh/users/" + userInfo[0].UserId + "/kudosu?offset=" + page + "&limit=" + count));
            List<KudosuInfo> tmpList = JsonConvert.DeserializeObject<List<KudosuInfo>>(json);
            foreach (var item in tmpList)
            {
                totalList.Add(item);
            }

            //} while (tmp_list.Count != 0);
            if (totalList.Count == 0)
                return new CommonMessageResponse("你竟然连一张图都没摸过？？", message, true);

            var recent = totalList[0];
            if (recent.Giver == null)
            {
                string msg = recent.Created_At.ToLongDateString() + "那天，你在" + recent.Post.Title +
                             $"({recent.Post.Url})" + (recent.Action == "vote.reset" ? "中的赞被取消了" : "中被点了个赞");
                return new CommonMessageResponse(msg, message, true);
            }

            string msg2 = recent.Created_At.ToLongDateString() + "那天，你通过" + recent.Post.Title +
                          $"的v1摸({recent.Post.Url})中{(recent.Action == "give" ? "向" : "被")}{recent.Giver.Username}{(recent.Action == "give" ? "买" : "扣")}了个币";
            return new CommonMessageResponse(msg2, message, true);
        }

        class KudosuInfo
        {
            public int Id { get; set; }
            public string Action { get; set; }
            public int Amount { get; set; }
            public string Model { get; set; }
            public DateTime Created_At { get; set; }
            public Giver Giver { get; set; }
            public Post Post { get; set; }
            public Details Details { get; set; }
        }

        class Giver
        {
            public string Url { get; set; }
            public string Username { get; set; }
        }

        class Post
        {
            public string Url { get; set; }
            public string Title { get; set; }
        }

        class Details
        {
            public string Event { get; set; }
        }
    }
}
