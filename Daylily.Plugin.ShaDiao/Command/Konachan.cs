using Daylily.Bot.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Daylily.Bot.Backend;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;

namespace Daylily.Plugin.ShaDiao
{
    [Name("konachan")]
    [Author("bleatingsheep")]
    [Version(2, 0, 23, PluginVersion.Stable)]
    [Help("设了")]
    [Command("konachan", "yandere", "safebooru")]
    public class Konachan : CoolQCommandPlugin
    {
        private static IDictionary<string, string> WebsiteMap => new Dictionary<string, string>
        {
            { "konachan".ToUpperInvariant(), "https://konachan.net" },
            { "yandere".ToUpperInvariant(), "https://yande.re" },
        };

        private static readonly IReadOnlyDictionary<string, string> Websites = new ReadOnlyDictionary<string, string>(WebsiteMap);

        private string GetWebsite(CoolQRouteMessage routeMsg)
        {
            var name = routeMsg.Command.ToUpperInvariant();
            return Websites.GetValueOrDefault(name);
        }

        public override void OnInitialized(string[] args)
        {
            return;
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg)
        {
            var domain = GetWebsite(routeMsg);
            if (string.IsNullOrEmpty(domain))
            {
                return routeMsg.ToSource("并不支持这个网站哦~~");
            }

            var k = new Api(domain);
            var result = k.PopularRecentAsync().Result;
            var post = result?.FirstOrDefault();
            return post == null ? null : routeMsg.ToSource(new FileImage(new Uri(post.JpegUrl)).ToString());
        }
    }
}
