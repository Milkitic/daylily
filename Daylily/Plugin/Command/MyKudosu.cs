using Daylily.Assist;
using Daylily.Database.BLL;
using Daylily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Daylily.Plugin.Command
{
    public class MyKudosu : Application
    {
        public override string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt)
        {
            //if (group != null) // 不给予群聊权限
            //    return null;
            BllUserRole bllUserRole = new BllUserRole();
            var user_info = bllUserRole.GetUserRoleByQQ(long.Parse(user));
            ifAt = true;
            if (user_info.Count == 0)
                 return "你都还没有绑ID，用白菜的setid +id";
            List<KudosuInfo> total_list = new List<KudosuInfo>();
            List<KudosuInfo> tmp_list = new List<KudosuInfo>();
            int page = 0;
            int count = 20;
            //do
            //{
            string json = WebRequestHelper.GetResponseString(WebRequestHelper.CreateGetHttpResponse("https://osu.ppy.sh/users/" + user_info[0].UserId + "/kudosu?offset=" + page + "&limit=" + count));
            tmp_list = JsonConvert.DeserializeObject<List<KudosuInfo>>(json);
            foreach (var item in tmp_list)
            {
                total_list.Add(item);
            }
            page += count;
            //} while (tmp_list.Count != 0);
            if (total_list.Count == 0) return "你竟然连一张图都没摸过？？";
            var recent = total_list[0];
            if (recent.Giver == null)
            {
                string tmp = recent.Created_At.ToLongDateString() + "那天，你在" + recent.Post.Title + $"({recent.Post.Url})" + (recent.Action == "vote.reset" ? "中的赞被取消了" : "中被点了个赞");
                return tmp;
            }
            return recent.Created_At.ToLongDateString() + "那天，你通过" + recent.Post.Title + $"的v1摸({recent.Post.Url})中{(recent.Action == "give" ? "向" : "被")}{recent.Giver.Username}{(recent.Action == "give" ? "买" : "扣")}了个币";
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
