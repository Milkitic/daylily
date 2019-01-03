using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface IBackend
    {
        string Name { get; }
        string[] Author { get; }
        int Major { get; }
        int Minor { get; }
        int Patch { get; }
        PluginVersion State { get; }
        string[] Helps { get; }
        Authority Authority { get; }
        BackendConfig BackendConfig { get; }

        void OnInitialized(string[] args);
        void OnErrorOccured(ExceptionEventArgs args);
        void AllPlugins_Initialized(string[] args);
    }
}
