using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Daylily.Plugin.Osu
{
    [Name("m4m匹配提示")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("用于提醒群友使用m4m插件。")]
    class M4MMatchNoticeApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("8f8cb4ce-379f-4a0a-9dcc-d1a1fb8bdd9c");

        internal static ConcurrentDictionary<string, DateTime> Tipped;

        public M4MMatchNoticeApp()
        {
            Tipped = LoadSettings<ConcurrentDictionary<string, DateTime>>("Tipped") ??
                      new ConcurrentDictionary<string, DateTime>();
        }

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;
            var msg = routeMsg.RawMessage.ToUpper();

            bool action = msg.Contains("摸图") || msg.Contains("看图") || msg.Contains("M4M");
            bool ask = msg.Contains("吗") || msg.Contains("么") || msg.Contains("?") || msg.Contains("？");
            bool[] matched =
            {
                msg.Contains("帮") && msg.Contains("摸"),
                msg.Contains("求摸"),
                msg.Contains("有") && action && ask,
                msg.Contains("有没有") && action,
            };

            var id = routeMsg.UserId;
            if (matched.Any(b => b) && (!Tipped.ContainsKey(id) ||
                                        Tipped.ContainsKey(id) &&
                                        Tipped[id] - DateTime.Now > new TimeSpan(7, 0, 0, 0)))
            {
                if (Tipped.ContainsKey(id))
                    Tipped[id] = DateTime.Now;
                else
                    Tipped.TryAdd(id, DateTime.Now);
                SaveSettings();
                return routeMsg.ToSource("你是在找人帮忙摸图吗？不想无助等待，立刻向我私聊\"/m4m\"。", true);
            }

            return null;
        }

        internal void SaveSettings()
        {
            SaveSettings(Tipped, "Tipped");
        }
    }
}
