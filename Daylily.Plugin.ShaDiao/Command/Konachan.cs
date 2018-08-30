using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daylily.Bot.Attributes;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.CoolQ;

namespace Daylily.Plugin.ShaDiao.Command
{
    [Command("konachan")]
    public class Konachan : CommandPlugin
    {
        private const string Website = "https://konachan.net";

        public override void Initialize(string[] args)
        {
            return;
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            var k = new Moebooru.Api(Website);
            var result = k.PopularRecentAsync().Result;
            if (!result.Any())
                return null;
            var post = result.First();
            return new CommonMessageResponse(new FileImage(post.JpegUrl, true).ToString(), messageObj);
        }
    }
}
