using Daylily.Bot.Message;

namespace Daylily.Bot.Backend
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
