using System;
using System.Linq;
using Daylily.Common;
using Daylily.Common.Logging;

namespace Daylily.Bot.Backend.Plugin
{
    public abstract class CommandPlugin : MessagePlugin, IInjectableBackend
    {
        public sealed override PluginType PluginType => PluginType.Command;
        public override bool RunInMultiThreading { get; } = true;
        public override bool RunInMultipleInstances { get; } = true;
        public override MiddlewareConfig MiddlewareConfig { get; } = new BackendConfig();
        protected ConcurrentRandom Random { get; } = new ConcurrentRandom();

        public virtual void OnCommandBindingFailed(BindingFailedEventArgs args)
        {

        }

        public string[] Commands { get; set; }

        protected CommandPlugin()
        {
            Type t = GetType();
            if (!t.IsDefined(typeof(CommandAttribute), false))
            {
                Logger.Warn($"\"{Name}\"尚未设置命令，因此无法被用户激活。");
            }
            else
            {
                var attrs = (CommandAttribute[])t.GetCustomAttributes(typeof(CommandAttribute), false);
                Commands = attrs.First().Commands;
            }
        }
    }
}
