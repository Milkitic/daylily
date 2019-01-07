using Daylily.Bot.Backend;
using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.CoolQ;
using Daylily.CoolQ.Message;
using Daylily.CoolQ.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Daylily.Plugin.Fun
{
    [Name("@检测")]
    [Author("yf_extension")]
    [Version(2, 0, 1, PluginVersion.Stable)]
    [Help("当自己被at时回击at对方")]
    public class CheckCqAtApp : CoolQApplicationPlugin
    {
        public override Guid Guid => new Guid("e6d765b3-a015-4192-9cc1-0cfa5c13ec55");

        private static readonly string PandaDir = Path.Combine(Domain.ResourcePath, "panda");

        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.MessageType == MessageType.Private)
                return null;

            string[] ids = CoolQCode.GetAt(routeMsg.RawMessage);
            if (ids == null || !ids.Contains("2181697779") && !ids.Contains("3421735167")) return null;
            Thread.Sleep(StaticRandom.Next(200, 300));
            if (StaticRandom.NextDouble() < 0.9)
                return routeMsg.ToSource("", true);
            else
            {
                var cqImg = new FileImage(Path.Combine(PandaDir, "at.jpg"));
                return routeMsg.ToSource(cqImg.ToString());
            }
        }
    }
}
