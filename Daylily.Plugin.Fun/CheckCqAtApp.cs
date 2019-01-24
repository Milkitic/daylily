using Daylily.Bot.Backend;
using Daylily.Bot.Messaging;
using Daylily.Common;
using Daylily.CoolQ;
using Daylily.CoolQ.Messaging;
using Daylily.CoolQ.Plugin;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Daylily.Plugin.Fun
{
    [Name("@检测")]
    [Author("yf_extension")]
    [Version(2, 0, 2, PluginVersion.Stable)]
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
            if (ids == null || !ids.Contains(routeMsg.ReportMeta.SelfId)) return null;
            if (StaticRandom.NextDouble() < 0.9)
                return routeMsg
                    .ToSource("", true)
                    .Delay(TimeSpan.FromSeconds(StaticRandom.Next(5)));
            else
            {
                var cqImg = new FileImage(Path.Combine(PandaDir, "at.jpg"));
                return routeMsg.ToSource(cqImg.ToString());
            }
        }
    }
}
