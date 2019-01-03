using Daylily.Bot.Attributes;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Common.Utils.LoggerUtils;
using System;

namespace Daylily.Bot.PluginBase
{
    public abstract class CommandPlugin : ResponsivePlugin
    {
        public sealed override PluginType PluginType => PluginType.Command;
        public string[] Commands { get; internal set; }
        public override bool RunInMultiThreading { get; } = true;
        public override bool RunInMultipleInstances { get; } = true;

        public virtual void OnCommandBindingFailed(BindingFailedEventArgs args)
        {

        }

        public override MiddlewareConfig MiddlewareConfig { get; } = new MiddlewareConfig();

        protected CommandPlugin()
        {
            Type t = GetType();
            if (!t.IsDefined(typeof(CommandAttribute), false))
            {
                if (t != typeof(ExtendPlugin)) Logger.Warn($"\"{Name}\"尚未设置命令，因此无法被用户激活。");
            }
            else
            {
                var attrs = (CommandAttribute[])t.GetCustomAttributes(typeof(CommandAttribute), false);
                Commands = attrs[0].Commands;
            }
        }
    }
}
