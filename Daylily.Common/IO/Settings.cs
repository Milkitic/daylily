using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Daylily.Common.Utils.LoggerUtils;

namespace Daylily.Common.IO
{
    public static class Settings
    {
        private static readonly string SettingsPath = Path.Combine(Domain.CurrentDirectory, "BotSettings");
        public static T LoadSettings<T>(string fileName)
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

        public static void SaveSettings<T>(T cls, string fileName)
        {
            Type clsT = cls.GetType();

            string saveName = Path.Combine(SettingsPath, (fileName ?? clsT.Name) + ".json");

            if (!Directory.Exists(SettingsPath))
                Directory.CreateDirectory(SettingsPath);

            ConcurrentFile.WriteAllText(saveName, Newtonsoft.Json.JsonConvert.SerializeObject(cls));
        }
    }
}
