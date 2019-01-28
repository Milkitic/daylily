using Daylily.Bot.Backend;
using Daylily.Bot.Messaging;
using Daylily.CoolQ;
using Daylily.CoolQ.CoolQHttp;
using Daylily.CoolQ.Messaging;
using Daylily.CoolQ.Plugin;
using System;

namespace Daylily.Plugin.Basic
{
    [Name("福袋撤回")]
    [Author("yf_extension")]
    [Version(2, 0, 0, PluginVersion.Beta)]
    [Help("福袋自动撤回插件。")]
    public class PackDetector : CoolQApplicationPlugin
    {
        public override Guid Guid { get; } = new Guid("41391c1c-141e-4af5-a33d-b200ff97f3be");
        public override CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope)
        {
            var routeMsg = scope.RouteMessage;
            if (routeMsg.RawMessage == "收到福袋，请使用新版手机QQ查看")
            {
                CoolQHttpApiClient.DeleteMessage(routeMsg.MessageId);
            }

            return null;
            //return routeMsg.ToSource("已撤回福袋。若需关闭，请使用 \"/plugin -disable PackDetector\"");
        }
    }
}
