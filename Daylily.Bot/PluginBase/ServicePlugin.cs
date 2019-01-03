using Daylily.Bot.Enum;
using Daylily.Bot.Models;

namespace Daylily.Bot.PluginBase
{
    public abstract class ServicePlugin : Plugin
    {
        public sealed override PluginType PluginType => PluginType.Service;
        public override MiddlewareConfig MiddlewareConfig { get; } = new MiddlewareConfig();

        public abstract void Execute(string[] args);
    }
}
