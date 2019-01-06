using Daylily.Bot.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class MessagePlugin : Plugin, IConcurrentBackend
    {
        public override PluginType PluginType => PluginType.Unknown;
        public abstract bool RunInMultiThreading { get; }
        public abstract bool RunInMultipleInstances { get; }

        public abstract RouteMessage OnMessageReceived(ScopeEventArgs routeMsg);
    }
}
