using Daylily.Bot.Backend;
using Daylily.Bot.Messaging;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using Daylily.CoolQ.Plugin;
using Daylily.Osu;
using Daylily.Osu.Cabbage;
using OSharp.V1.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Plugin.Osu
{
    [Name("随机挖坑")]
    [Author("yf_extension")]
    [Version(2, 1, 2, PluginVersion.Beta)]
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
            var beatmapSets = client.GetBeatmapSetsByCreator(new UserId(id))
                .Where(k => k.Beatmaps.FirstOrDefault()?.LastUpdate.AddDays(28) < DateTimeOffset.Now)
                .ToArray();

            if (beatmapSets.Length == 0)
            {
                return routeMsg.ToSource("你没有Graveyard Beatmaps！", true);
            }

            var beatmapSet = beatmapSets[StaticRandom.Next(beatmapSets.Length)];
            var cqMusic = new CustomMusic(
                $"https://osu.ppy.sh/s/{beatmapSet.Id}",
                $"https://b.ppy.sh/preview/{beatmapSet.Id}.mp3",
                beatmapSet.Title,
                $"{beatmapSet.Artist}\r\n({beatmapSet.FavouriteCount} fav)",
                $"https://b.ppy.sh/thumb/{beatmapSet.Id}l.jpg");

            return routeMsg
                .ToSource(cqMusic.ToString())
                .ForceToSend();
        }
    }
}
