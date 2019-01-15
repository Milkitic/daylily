using Daylily.Bot.Message;
using Daylily.Common;
using Daylily.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daylily.Common.Logging;

namespace Daylily.Bot.Backend.Plugins
{
    public abstract class Plugin : IBackend
    {
        public abstract PluginType PluginType { get; }
        public string Name { get; set; }
        public string[] Author { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public PluginVersion State { get; set; }
        public string[] Helps { get; set; }
        public Authority TargetAuthority { get; set; }
        public abstract MiddlewareConfig MiddlewareConfig { get; }
        public string Version => string.Concat(Major, ".", Minor, ".", Patch);
        public abstract Guid Guid { get; }

        public event ExceptionEventHandler ErrorOccured;

        public virtual void OnInitialized(StartupConfig startup)
        {

        }

        public virtual void OnErrorOccured(ExceptionEventArgs args)
        {

        }

        public virtual void AllPlugins_Initialized(StartupConfig startup)
        {

        }

        protected Plugin()
        {
            Type t = GetType();
            if (!t.IsDefined(typeof(NameAttribute), false)) Name = t.Name;
            if (!t.IsDefined(typeof(AuthorAttribute), false)) Author = new[] { "anonym" };
            if (!t.IsDefined(typeof(HelpAttribute), false)) Helps = new[] { "Nothing here." };
            if (!t.IsDefined(typeof(VersionAttribute), false))
            {
                Major = 0;
                Minor = 0;
                Patch = 1;
                State = PluginVersion.Alpha;
            }

            var attrs = t.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                switch (attr)
                {
                    case NameAttribute name:
                        Name = name.Name ?? t.Name;
                        break;
                    case AuthorAttribute author:
                        Author = author.Author ?? new[] { "anonym" };
                        break;
                    case VersionAttribute ver:
                        Major = ver.Major;
                        Minor = ver.Minor;
                        Patch = ver.Patch;
                        State = ver.PluginVersion;
                        if (State == PluginVersion.Alpha)
                            Logger.Warn($"\"{Name}\" is currently in \"{State}\" state, which may leads to crash.");
                        break;
                    case HelpAttribute help:
                        Helps = help.Helps != null
                            ? help.Helps.Select(k => k.EndsWith("。") ? k : k + "。").ToArray()
                            : new[] { "Nothing here." };
                        TargetAuthority = help.Authority;
                        break;
                }
            }
        }

        protected string SettingsPath => Path.Combine(Domain.PluginPath, GetType().Name);

        protected ConcurrentRandom GlobalRandom => DaylilyCore.Current.GlobalRandom;
        protected static ConcurrentRandom StaticRandom { get; } = new ConcurrentRandom();
        protected static void SendMessage(RouteMessage routeMsg) => DaylilyCore.Current.MessageDispatcher?.SendMessage(routeMsg);
        protected virtual void SaveSettings<T>(T cls, string fileName = null, bool writeLog = false)
        {
            Type clsT = cls.GetType();

            string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

            if (!Directory.Exists(SettingsPath))
                Directory.CreateDirectory(SettingsPath);

            ConcurrentFile.WriteAllText(saveName,
                Newtonsoft.Json.JsonConvert.SerializeObject(cls, Newtonsoft.Json.Formatting.Indented));
            if (writeLog)
            {
                var fileInfo = new FileInfo(saveName);
                Logger.Success($"Saved settings to \"{Path.Combine("~", fileInfo.Directory?.Name, fileInfo.Name)}\".");
            }
        }

        protected virtual T LoadSettings<T>(string fileName = null, bool writeLog = false)
        {
            try
            {
                Type clsT = typeof(T);

                string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

                if (!Directory.Exists(SettingsPath))
                    Directory.CreateDirectory(SettingsPath);

                string json = ConcurrentFile.ReadAllText(saveName);
                if (writeLog)
                {
                    var fileInfo = new FileInfo(saveName);
                    Logger.Success($"Loaded settings from \"{Path.Combine("~", fileInfo.Directory?.Name, fileInfo.Name)}\".");
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch (FileNotFoundException)
            {
                return default;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                throw;
            }
        }

        protected void SaveLogs(string content, string fileName)
        {
            string saveName = Path.Combine(SettingsPath, fileName + ".log");

            if (!Directory.Exists(SettingsPath))
                Directory.CreateDirectory(SettingsPath);

            string fullContent = DateTime.Now + Environment.NewLine + content + Environment.NewLine;

            ConcurrentFile.AppendAllText(saveName, fullContent);
        }

    }
}
