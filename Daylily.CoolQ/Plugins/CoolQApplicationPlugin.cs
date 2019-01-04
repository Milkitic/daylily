using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Message;
using Daylily.CoolQ.Message;

namespace Daylily.CoolQ.Plugins
{
    public abstract class CoolQApplicationPlugin : ApplicationPlugin
    {
        public abstract CoolQRouteMessage OnMessageReceived(CoolQRouteMessage routeMsg);

        public override RouteMessage OnMessageReceived(RouteMessage routeMsg)
        {
            return OnMessageReceived((CoolQRouteMessage)routeMsg);
        }
    }
}
