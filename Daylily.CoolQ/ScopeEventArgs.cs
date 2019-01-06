using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.CoolQ
{
    public class ScopeEventArgs : EventArgs
    {
        public Message.CoolQRouteMessage RouteMessage { get; set; }
        public List<Bot.Backend.Plugins.ApplicationPlugin> ApplicationPlugins { get; set; }
    }
}
