using Daylily.Bot.Enum;
using Daylily.Bot.Models;

namespace Daylily.Bot.PluginBase
{
    public abstract class ApplicationPlugin : ResponsivePlugin
    {
        public sealed override PluginType PluginType => PluginType.Application;
        public override bool RunInMultiThreading { get; } = true;
        public override bool RunInMultipleInstances { get; } = false;
        public override MiddlewareConfig MiddlewareConfig { get; } = new MiddlewareConfig();
    }
}
