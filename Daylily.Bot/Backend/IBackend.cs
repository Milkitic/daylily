using System;
using Daylily.Bot.Message;

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

        void OnInitialized(string[] args);
        void OnErrorOccured(ExceptionEventArgs args);
        void AllPlugins_Initialized(string[] args);
    }
}
