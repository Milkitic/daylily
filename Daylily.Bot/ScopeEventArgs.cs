using Daylily.Bot.Message;
using System;
using System.Collections.Generic;

namespace Daylily.Bot
{
    public class ScopeEventArgs : EventArgs
    {
        public RouteMessage RouteMessage { get; set; }
        public List<Bot.Backend.Plugins.ApplicationPlugin> DisabledApplications { get; set; }
    }
}
