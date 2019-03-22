using Daylily.Bot;
using Daylily.Bot.Backend.Plugin;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Messaging;

namespace Daylily.CoolQ.Plugin
{
    public abstract class CoolQApplicationPlugin : ApplicationPlugin
    {
        public abstract CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope);

        public override RouteMessage OnMessageReceived(ScopeEventArgs scope)
        {
            return OnMessageReceived((CoolQScopeEventArgs)scope);
        }
    }
}
