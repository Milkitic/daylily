using System;
using System.IO;

namespace Daylily.Common
{
    public static class Domain
    {
        public static string SecretPath => Path.Combine(new DirectoryInfo(CurrentPath).Parent.FullName, "DaylilySecret");

        public static string CurrentPath => AppDomain.CurrentDomain.BaseDirectory;

        public static string CachePath => Path.Combine(CurrentPath, "_cache");
        public static string CacheImagePath => Path.Combine(CachePath, "_images");

        public static string ResourcePath => Path.Combine(CurrentPath, "Resource");

        public static string PluginPath => Path.Combine(CurrentPath, "Plugin");
        //public static string ExtendedPluginPath => Path.Combine(PluginPath, "Extended");

        public static string LogPath => Path.Combine(CurrentPath, "Log");

        public static string SettingsPath => Path.Combine(CurrentPath, "BotSettings");

    }
}
