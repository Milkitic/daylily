namespace Daylily.Bot.Backend.Plugin
{
    public abstract class ApplicationPlugin : MessagePlugin
    {
        public sealed override PluginType PluginType => PluginType.Application;
        public override bool RunInMultiThreading { get; } = true;
        public override bool RunInMultipleInstances { get; } = false;
        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig();
    }
}
