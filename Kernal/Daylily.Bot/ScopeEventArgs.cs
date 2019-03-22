using System;
using System.Collections.Generic;
using Daylily.Bot.Backend.Plugin;
using Daylily.Bot.Messaging;

namespace Daylily.Bot
{
    public class ScopeEventArgs : EventArgs
    {
        public RouteMessage RouteMessage { get; set; }
        public List<ApplicationPlugin> DisabledApplications { get; set; }
    }
}
