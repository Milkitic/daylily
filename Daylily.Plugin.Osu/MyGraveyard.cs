using CSharpOsu.V1.User;
using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common.Logging;
using Daylily.Common.Web;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using Daylily.Osu;
using Daylily.Osu.Cabbage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Daylily.Plugin.Osu
{
    [Name("随机挖坑")]
    [Author("yf_extension")]
    [Version(2, 1, 0, PluginVersion.Beta)]
    [Help("从发送者的Graveyard Beatmaps中随机挖一张图。")]
    [Command("挖坑")]
    public class MyGraveyard : CoolQCommandPlugin
    {
        public override Guid Guid => new Guid("cbfe5649-6898-4182-aad3-7121f786b4cd");

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            BllUserRole bllUserRole = new BllUserRole();
            List<TableUserRole> userInfo = bllUserRole.GetUserRoleByQq(long.Parse(routeMsg.UserId));
            if (userInfo.Count == 0)
                return routeMsg.ToSource(DefaultReply.IdNotBound, true);

            var id = userInfo[0].UserId;

            var client = new OldSiteApiClient();
            var beatmapSets = client.GetBeatmapSetsByCreator(new UserId(id));

            if (beatmapSets.Length == 0)
            {
                return routeMsg.ToSource("惊了，你竟然会没坑！", true);
            }

            var beatmapSet = beatmapSets[StaticRandom.Next(beatmapSets.Length)];
            var cqMusic = new CustomMusic(
                $"https://osu.ppy.sh/s/{beatmapSet.Id}",
                $"https://b.ppy.sh/preview/{beatmapSet.Id}.mp3", 
                beatmapSet.Title,
                $"{beatmapSet.Artist}\r\n({beatmapSet.FavouriteCount} fav)", 
                $"https://b.ppy.sh/thumb/{beatmapSet.Id}l.jpg");

            return routeMsg.ToSource(cqMusic.ToString());
        }
    }
}
