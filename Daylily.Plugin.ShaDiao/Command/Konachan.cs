using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.CoolQ;

namespace Daylily.Plugin.ShaDiao.Command
{
    [Name("konachan")]
    [Author("bleatingsheep")]
    [Version(0, 0, 23, PluginVersion.Stable)]
    [Help("设了")]
    [Command("konachan", "yandere", "safebooru")]
    public class Konachan : CommandPlugin
    {
        private static IDictionary<string, string> WebsiteMap => new Dictionary<string, string>
        {
            { "konachan".ToUpperInvariant(), "https://konachan.net" },
            { "yandere".ToUpperInvariant(), "https://yande.re" },
        };

        private static readonly IReadOnlyDictionary<string, string> Websites = new ReadOnlyDictionary<string, string>(WebsiteMap);

        private string GetWebsite(CommonMessage messageObj)
        {
            var name = messageObj.Command.ToUpperInvariant();
            return Websites.GetValueOrDefault(name);
        }

        public override void Initialize(string[] args)
        {
            return;
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            var domain = GetWebsite(messageObj);
            if (string.IsNullOrEmpty(domain))
            {
                return new CommonMessageResponse("并不支持这个网站哦~~", messageObj);
            }

            var k = new Moebooru.Api(domain);
            var result = k.PopularRecentAsync().Result;
            var post = result?.FirstOrDefault();
            return post == null ? null : new CommonMessageResponse(new FileImage(post.JpegUrl, true).ToString(), messageObj);
        }
    }
}
