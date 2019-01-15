using System;
using Daylily.Bot.Message;
using Daylily.Common;

namespace Daylily.Bot.Backend
{
    public interface IBackend : IMiddleware
    {
        string Name { get; }
        string[] Author { get; }
        int Major { get; }
        int Minor { get; }
        int Patch { get; }
        PluginVersion State { get; }
        string[] Helps { get; }
        Authority TargetAuthority { get; }
        Guid Guid { get; }

        void OnInitialized(StartupConfig startup);
        void OnErrorOccured(ExceptionEventArgs args);
        void AllPlugins_Initialized(StartupConfig startup);
    }
}
