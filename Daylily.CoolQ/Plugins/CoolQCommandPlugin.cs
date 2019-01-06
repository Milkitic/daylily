using Daylily.Bot;
using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.CoolQ.Plugins
{
    public abstract class CoolQCommandPlugin : CommandPlugin
    {
        public abstract CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs routeMsg);

        public override RouteMessage OnMessageReceived(ScopeEventArgs routeMsg)
        {
            return OnMessageReceived((CoolQScopeEventArgs)routeMsg);
        }
    }
}
