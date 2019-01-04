using Daylily.Bot.Message;
using Daylily.Bot.Models;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public Authority Authority { get; set; }
        public abstract MiddlewareConfig MiddlewareConfig { get; }
        public string Version => string.Concat(Major, ".", Minor, ".", Patch);

        public virtual void OnInitialized(string[] args)
        {

        }

        public virtual void OnErrorOccured(ExceptionEventArgs args)
        {

        }

        public virtual void AllPlugins_Initialized(string[] args)
        {

        }

        protected Plugin()
        {
            Type t = GetType();
            if (!t.IsDefined(typeof(NameAttribute), false)) Name = t.Name;
            if (!t.IsDefined(typeof(AuthorAttribute), false)) Author = new[] { "undefined" };
            if (!t.IsDefined(typeof(HelpAttribute), false)) Helps = new[] { "尚无帮助信息" };
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
                        Author = author.Author ?? new[] { "undefined" };
                        break;
                    case VersionAttribute ver:
                        Major = ver.Major;
                        Minor = ver.Minor;
                        Patch = ver.Patch;
                        State = ver.PluginVersion;
                        if (State == PluginVersion.Alpha)
                            Logger.Warn($"\"{Name}\" 仅为{State}版本。可能出现大量无法预料的问题。");
                        break;
                    case HelpAttribute help:
                        Helps = help.Helps ?? new[] { "尚无帮助信息" };
                        Authority = help.Authority;
                        break;
                }
            }
        }

        protected string SettingsPath => Path.Combine(Domain.PluginPath, GetType().Name);

        protected Random GlobalRandom => DaylilyCore.Current.GlobalRandom;
        protected static Random StaticRandom { get; } = new Random();
        protected Random Random { get; } = new Random();
        protected static void SendMessage(RouteMessage routeMsg) => DaylilyCore.Current.Dispatcher.SendMessage(routeMsg);
        protected void SaveSettings<T>(T cls, string fileName = null, bool writeLog = false)
        {
            Type clsT = cls.GetType();

            string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

            if (!Directory.Exists(SettingsPath))
                Directory.CreateDirectory(SettingsPath);

            ConcurrentFile.WriteAllText(saveName, Newtonsoft.Json.JsonConvert.SerializeObject(cls));
            if (writeLog)
            {
                var fileInfo = new FileInfo(saveName);
                Logger.Success($"写入了 {Path.Combine("~", fileInfo.Directory?.Name, fileInfo.Name)}。");
            }
        }

        protected T LoadSettings<T>(string fileName = null, bool writeLog = false)
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
                    Logger.Success($"读取了 {Path.Combine("~", fileInfo.Directory?.Name, fileInfo.Name)}。");
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
