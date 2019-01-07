using Daylily.Bot;
using Daylily.CoolQ.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.CoolQ
{
    public class CoolQScopeEventArgs : ScopeEventArgs
    {
        public new CoolQRouteMessage RouteMessage { get; set; }
    }
}
