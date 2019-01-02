using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using Daylily.Bot.Interface;

namespace Daylily.Bot.Models
{
    public class StartupConfig
    {
        public StartupConfig(IDispatcher dispatcher, IFrontend[] frontends, Metadata metadata,
            Action<IDispatcher> generalDispatcherConfig = null, Action<IFrontend> generalFrontendsConfig = null)
        {
            ApplicationMetadata = metadata;
            GeneralDispatcherConfig = generalDispatcherConfig;
            GeneralFrontendsConfig = generalFrontendsConfig;
            Dispatcher = dispatcher;
            Frontends = frontends;
        }

        public Metadata ApplicationMetadata { get; }

        public IDispatcher Dispatcher { get; }

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
