namespace Daylily.Bot.Backend.Plugin
{
    public abstract class ServicePlugin : PluginBase
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig();
    }
}
