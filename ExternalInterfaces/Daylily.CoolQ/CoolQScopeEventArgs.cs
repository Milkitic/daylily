using Daylily.Bot;
using Daylily.CoolQ.Messaging;

namespace Daylily.CoolQ
{
    public class CoolQScopeEventArgs : ScopeEventArgs
    {
        public new CoolQRouteMessage RouteMessage { get; set; }
    }
}
