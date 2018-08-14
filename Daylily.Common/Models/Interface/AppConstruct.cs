using System;
using System.IO;
using Daylily.Common.Assist;
using Daylily.Common.Interface.CQHttp;
using Daylily.Common.IO;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.CQResponse.Api;
using Daylily.Common.Models.CQResponse.Api.Abstract;
using Daylily.Common.Models.Enum;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Models.Interface
{
    public abstract class AppConstruct
    {
        #region public members

        public abstract AppType AppType { get; }
        public string Name { get; internal set; }
        public string[] Author { get; internal set; }
        public int Major { get; internal set; }
        public int Minor { get; internal set; }
        public int Patch { get; internal set; }
        public string Version => string.Concat(Major, ".", Minor, ".", Patch);
        public PluginVersion State { get; internal set; }
        public string[] Helps { get; internal set; }
        public PermissionLevel HelpType { get; internal set; }

        #endregion public members

        #region protected members

        protected AppConstruct()
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
                        HelpType = help.HelpType;
                        break;
                }
            }
        }

        protected PermissionLevel CurrentLevel { get; set; }

        protected string SettingsPath => Path.Combine(_pluginDir, GetType().Name);

        protected static readonly Random Rnd = new Random();

        protected static void SendMessage(CommonMessageResponse response) => CqApi.SendMessage(response);

        protected static void SendMessage(CommonMessageResponse response, string groupId, string discussId,
            MessageType messageType) => CqApi.SendMessage(response, groupId, discussId, messageType);

        protected void SaveSettings<T>(T cls, string fileName = null)
        {
            Type clsT = cls.GetType();

            string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

            if (!Directory.Exists(SettingsPath))
                Directory.CreateDirectory(SettingsPath);

            ConcurrentFile.WriteAllText(saveName, Newtonsoft.Json.JsonConvert.SerializeObject(cls));
        }

        protected T LoadSettings<T>(string fileName = null)
        {
            try
            {
                Type clsT = typeof(T);

                string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

                if (!Directory.Exists(SettingsPath))
                    Directory.CreateDirectory(SettingsPath);

                string json = ConcurrentFile.ReadAllText(saveName);
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

        #endregion protected members

        #region private members

        private readonly string _pluginDir = Path.Combine(Domain.CurrentDirectory, "plugins");

        #endregion private members
    }
}
