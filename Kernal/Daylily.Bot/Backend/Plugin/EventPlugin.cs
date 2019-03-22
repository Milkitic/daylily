using System;

namespace Daylily.Bot.Backend.Plugin
{
    public abstract class EventPlugin : PluginBase, IConcurrentBackend
    {
        public override PluginType PluginType => PluginType.Event;
        public bool RunInMultiThreading { get; } = true;
        public bool RunInMultipleInstances { get; } = true;
        public abstract Type AcceptType { get; }
        public override MiddlewareConfig MiddlewareConfig { get; }

        public abstract bool OnEventReceived(object eventObj);
    }
}
