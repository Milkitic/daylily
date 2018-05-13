using DaylilyWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Assist
{
    public static class Mapper
    {
        public static List<string> NormalPlugins { get; set; } = new List<string>();

        private static Dictionary<string, string> InnerCmdPlugin { get; set; } = new Dictionary<string, string>();
        private static JsonSettings FileCmdPlugins { get; set; } = new JsonSettings();

        private static readonly string PLUGIN_DIR = Path.Combine(Environment.CurrentDirectory, "plugins");
        private static readonly string SETTINGS_FILE = Path.Combine(PLUGIN_DIR, "plugins.json");

        public static void Init()
        {
            // 偷懒测试用
            //InnerCmdPlugin.Add("mykds", "MyKudosu");
            //InnerCmdPlugin.Add("test", "Test");
            //InnerCmdPlugin.Add("h", "Help");
            //InnerCmdPlugin.Add("help", "Help");
            //InnerCmdPlugin.Add("setid", "SetId");
            //InnerCmdPlugin.Add("myelo", "MyElo");
            //InnerCmdPlugin.Add("elo", "Elo");
            //InnerCmdPlugin.Add("sleep", "Sleep");
            //InnerCmdPlugin.Add("slip", "Sleep");
            InnerCmdPlugin.Add("pp", "PpPlus");
            InnerCmdPlugin.Add("send", "Send");
            InnerCmdPlugin.Add("kd", "Kudosu");

            // 内置功能
            NormalPlugins.Add("PornDetector");
            NormalPlugins.Add("DragonDetectorAlpha");
            NormalPlugins.Add("PandaDetectorAlpha");
            NormalPlugins.Add("CheckCqAt");

            if (!Directory.Exists(PLUGIN_DIR))
                Directory.CreateDirectory(PLUGIN_DIR);

            if (!File.Exists(SETTINGS_FILE))
                CreateJson();
            else
            {
                string jsonString = File.ReadAllText(SETTINGS_FILE);
                FileCmdPlugins = JsonConvert.DeserializeObject<JsonSettings>(jsonString);
            }
        }

        public static string GetClassName(string name, out string fileName)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> item in FileCmdPlugins.Plugins)
            {
                if (item.Value.Keys.Contains(name))
                {
                    fileName = Path.Combine(PLUGIN_DIR, item.Key);
                    return item.Value[name];
                }
            }
            fileName = null;
            if (!InnerCmdPlugin.Keys.Contains(name))
                return null;
            return InnerCmdPlugin[name];
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

            File.WriteAllText(Path.Combine(PLUGIN_DIR, "plugins.json"), jsonFile);
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
            else
            {
                return str;
            }
        }
    }
}
