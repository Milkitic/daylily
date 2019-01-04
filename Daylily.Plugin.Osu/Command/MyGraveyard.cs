using Bleatingsheep.Osu.ApiV2b.Models;
using Daylily.Bot.Message;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.RequestUtils;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.Osu
{
    [Name("随机挖坑")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Beta)]
    [Help("从发送者的Graveyard Beatmaps中随机挖一张图。")]
    [Command("挖坑")]
    public class MyGraveyard : CoolQCommandPlugin
    {
        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            BllUserRole bllUserRole = new BllUserRole();
            List<TblUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(routeMsg.UserId));
            if (userInfo.Count == 0)
                return routeMsg.ToSource(DefaultReply.IdNotBound, true);

            var id = userInfo[0].UserId.ToString();

            List<Beatmapset> totalList = new List<Beatmapset>();
            Beatmapset[] tmpArray;
            int page = 0;
            const int count = 10;
            do
            {
                string json = WebRequestUtil.GetResponseString(
                    WebRequestUtil.CreateGetHttpResponse(
                        "https://osu.ppy.sh/users/" + id + "/beatmapsets/graveyard?offset=" + page + "&limit=" + count));
                Logger.Debug("GET JSON");

                tmpArray = JsonConvert.DeserializeObject<Beatmapset[]>(json);
                totalList.AddRange(tmpArray);
                page += count;

                if (tmpArray.Length != count) break;
            } while (tmpArray.Length != 0);

            if (totalList.Count == 0)
            {
                return routeMsg.ToSource("惊了，你竟然会没坑！", true);
            }

            Random rnd = new Random();
            Beatmapset beatmap = totalList[rnd.Next(totalList.Count)];
            var cqMusic = new CustomMusic("https://osu.ppy.sh/s/" + beatmap.Id, $"https://b.ppy.sh/preview/{beatmap.Id}.mp3", beatmap.Title,
                $"{beatmap.Artist}\r\n({beatmap.FavouriteCount} fav)", $"https://b.ppy.sh/thumb/{beatmap.Id}l.jpg");

            return routeMsg.ToSource(cqMusic.ToString());
        }
    }
}
