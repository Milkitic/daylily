using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class EventPlugin : Plugin, IConcurrentBackend
    {
        public override PluginType PluginType => PluginType.Event;
        public bool RunInMultiThreading { get; } = true;
        public bool RunInMultipleInstances { get; } = true;
        public abstract Type AcceptType { get; }
        public override MiddlewareConfig MiddlewareConfig { get; }

        public abstract bool OnEventReceived(object eventObj);
    }
}
