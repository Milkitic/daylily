using System;
using System.Runtime.Versioning;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Frontend;

namespace Daylily.Bot
{
    public class StartupConfig
    {
        public StartupConfig(IDispatcher messageDispatcher, IFrontend[] frontends, Metadata metadata,
            Action<IDispatcher> generalDispatcherConfig = null, Action<IFrontend> generalFrontendsConfig = null)
        {
            ApplicationMetadata = metadata;
            GeneralDispatcherConfig = generalDispatcherConfig;
            GeneralFrontendsConfig = generalFrontendsConfig;
            MessageDispatcher = messageDispatcher;
            Frontends = frontends;
        }

        public Metadata ApplicationMetadata { get; }

        public IDispatcher MessageDispatcher { get; }

        public IFrontend[] Frontends { get; }

        public Action<IDispatcher> GeneralDispatcherConfig { get; }
        public Action<IFrontend> GeneralFrontendsConfig { get; }

        public struct Metadata
        {
            public string ApplicationName { get; set; }
            public FrameworkName FrameworkName { get; set; }
            public Version BotVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
