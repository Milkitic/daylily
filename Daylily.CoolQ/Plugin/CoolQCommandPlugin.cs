using Daylily.Bot;
using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugin;
using Daylily.Bot.Messaging;
using Daylily.CoolQ.Messaging;
using System.Linq;

namespace Daylily.CoolQ.Plugin
{
    public abstract class CoolQCommandPlugin : CommandPlugin
    {
        public abstract CoolQRouteMessage OnMessageReceived(CoolQScopeEventArgs scope);

        public override RouteMessage OnMessageReceived(ScopeEventArgs scope)
        {
            return OnMessageReceived((CoolQScopeEventArgs)scope);
        }

        public override void OnCommandBindingFailed(BindingFailedEventArgs args)
        {
            SendMessage(
                ((CoolQScopeEventArgs)args.Scope).RouteMessage.ToSource(
                    $"参数有误...发送 \"/help {Commands.FirstOrDefault()}\" 了解如何使用。"
                )
            );
        }
    }
}
