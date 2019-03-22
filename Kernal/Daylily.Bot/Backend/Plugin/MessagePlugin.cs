using Daylily.Bot.Messaging;

namespace Daylily.Bot.Backend.Plugin
{
    public abstract class MessagePlugin : PluginBase, IConcurrentBackend
    {
        public override PluginType PluginType => PluginType.Unknown;
        public abstract bool RunInMultiThreading { get; }
        public abstract bool RunInMultipleInstances { get; }

        public abstract RouteMessage OnMessageReceived(ScopeEventArgs scope);
    }
}
