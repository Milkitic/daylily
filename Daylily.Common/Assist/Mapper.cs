using Daylily.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Daylily.Common.Assist
{
    public static class Mapper
    {
        public static Dictionary<string, string> ServicePlugins { get; set; } = new Dictionary<string, string>();
        public static List<string> NormalPlugins { get; set; } = new List<string>();

        private static Dictionary<string, string> InnerCmdPlugin { get; set; } = new Dictionary<string, string>();
        private static JsonSettings FileCmdPlugins { get; set; } = new JsonSettings();

        private static readonly string ServiceDir = Path.Combine(Environment.CurrentDirectory, "services");
        private static readonly string PluginDir = Path.Combine(Environment.CurrentDirectory, "plugins");
        private static readonly string SettingsFile = Path.Combine(PluginDir, "plugins.json");

        public static void Init()
        {
            // 偷懒测试用
            InnerCmdPlugin.Add("send", "Send");
            InnerCmdPlugin.Add("kd", "Kudosu");
            InnerCmdPlugin.Add("sdown", "Shutdown");
            InnerCmdPlugin.Add("rcon", "Rcon");
            InnerCmdPlugin.Add("panda", "Panda");

            // 内置功能
            NormalPlugins.Add("PornDetector");
            NormalPlugins.Add("DragonDetectorAlpha");
            NormalPlugins.Add("PandaDetectorAlpha");
            NormalPlugins.Add("CheckCqAt");

            if (!Directory.Exists(PluginDir))
                Directory.CreateDirectory(PluginDir);
            if (!Directory.Exists(ServiceDir))
                Directory.CreateDirectory(ServiceDir);

            foreach (var item in new DirectoryInfo(ServiceDir).GetFiles())
            {
                if (item.Extension.ToLower() == ".dll")
                {
                    ServicePlugins.Add(item.Name, item.FullName);
                }
            }

            if (!File.Exists(SettingsFile))
                CreateJson();
            else
            {
                string jsonString = File.ReadAllText(SettingsFile);
                FileCmdPlugins = JsonConvert.DeserializeObject<JsonSettings>(jsonString);
            }
        }

        public static string GetClassName(string name, out string fileName)
        {
            foreach (var item in FileCmdPlugins.Plugins)
            {
                if (!item.Value.Keys.Contains(name))
                    continue;
                fileName = Path.Combine(PluginDir, item.Key);
                return item.Value[name];
            }
            fileName = null;
            return !InnerCmdPlugin.Keys.Contains(name) ? null : InnerCmdPlugin[name];
        }

        private static void CreateJson()
        {
            JsonSettings jSettings = new JsonSettings();
            jSettings.Plugins.Add("Daylily.Plugin.Core.dll",
                new Dictionary<string, string> {
                { "ping", "Daylily.Plugin.Command.Ping" },
                { "cd", "Daylily.Plugin.Command.Cd" },
                { "roll", "Daylily.Plugin.Command.Roll" }
                });

            jSettings.Plugins.Add("Daylily.Plugin.Debug.dll",
                new Dictionary<string, string> {
                { "echo", "Daylily.Plugin.Echo" },
                { "calc", "Daylily.Plugin.Calc" }
                });
            string jsonFile = ConvertJsonString(JsonConvert.SerializeObject(jSettings));

            File.WriteAllText(Path.Combine(PluginDir, "plugins.json"), jsonFile);
        }

        public static string ConvertJsonString(string str)
        {
            //格式化json字符串  
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            return str;
        }
    }
}
