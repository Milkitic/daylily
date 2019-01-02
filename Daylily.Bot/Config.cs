using Daylily.Bot.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;

namespace Daylily.Bot
{
    public class Config
    {
        public Config(IDispatcher dispatcher, IFrontend[] frontends, Metadata metadata)
        {
            ApplicationMetadata = metadata;
            Dispatcher = dispatcher;
            Frontends = frontends;
        }

        public Metadata ApplicationMetadata { get; }

        public IDispatcher Dispatcher { get; }

        public IFrontend[] Frontends { get; }

        public struct Metadata
        {
            public string ApplicationName { get; set; }
            public FrameworkName FrameworkName { get; set; }
            public Version BotVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
