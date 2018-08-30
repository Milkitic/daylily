using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bleatingsheep.Osu.ApiV2b.Models;
using Daylily.Bot;
using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common.Utils.StringUtils;
using Daylily.Osu.Database.BLL;
using Daylily.Osu.Database.Model;
using Daylily.Osu.Interface;

namespace Daylily.Plugin.Osu.Application
{
    [Name("m4m匹配提示")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Stable)]
    [Help("用于提醒群友使用m4m插件。")]
    public class M4MMatchNotice : ApplicationPlugin
    {
        internal static ConcurrentDictionary<string, DateTime> Tipped;

        public M4MMatchNotice()
        {
            Tipped = LoadSettings<ConcurrentDictionary<string, DateTime>>("Tipped") ??
                      new ConcurrentDictionary<string, DateTime>();
        }

        public override CommonMessageResponse Message_Received(CommonMessage messageObj)
        {
            if (messageObj.MessageType == MessageType.Private)
                return null;
            var msg = messageObj.Message.ToUpper();

            bool action = msg.Contains("摸图") || msg.Contains("看图") || msg.Contains("M4M");
            bool ask = msg.Contains("吗") || msg.Contains("么") || msg.Contains("?") || msg.Contains("？");
            bool[] matched =
            {
                msg.Contains("帮") && msg.Contains("摸"),
                msg.Contains("求摸"),
                msg.Contains("有") && action && ask,
                msg.Contains("有没有") && action,
            };

            var id = messageObj.UserId;
            if (matched.Any(b => b) && (!Tipped.ContainsKey(id) ||
                                        Tipped.ContainsKey(id) &&
                                        Tipped[id] - DateTime.Now > new TimeSpan(7, 0, 0, 0)))
            {
                if (Tipped.ContainsKey(id))
                    Tipped[id] = DateTime.Now;
                else
                    Tipped.TryAdd(id, DateTime.Now);
                SaveSettings();
                return new CommonMessageResponse("你是在找人帮忙摸图吗？不想无助等待，立刻向我私聊\"/m4m\"。", messageObj, true);
            }

            return null;
        }

        internal void SaveSettings()
        {
            SaveSettings(Tipped, "Tipped");
        }
    }
}
