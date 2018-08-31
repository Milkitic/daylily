using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Daylily.Bot.Console;
using Daylily.Bot.Interface;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.StringUtils;
using Daylily.CoolQ;
using Daylily.CoolQ.Interface.CqHttp;
using Daylily.CoolQ.Models.CqResponse;
using Daylily.Cos;
using Daylily.Osu;
using Daylily.Osu.Database;
using SysConsole = System.Console;

namespace Daylily.Bot
{
    public static class Core
    {
        public static event JsonReceivedEventHandler JsonReceived;
        public static event MessageReceivedEventHandler MessageReceived;

        private static IJsonDeserializer _jsonDeserializer;
        private static IDispatcher _dispatcher;


        public static void InitCore(string[] args, IJsonDeserializer jsonDeserializer, IDispatcher dispatcher)
        {
            Logger.Raw(@".__       . .   
|  \ _.  .|*|  .
|__/(_]\_||||\_|
       ._|   ._|");
            Logger.Raw(args[0]);

            CreateDirectories(); // 创建目录
            LoadSecret(); // 加载配置
            // signup
            _jsonDeserializer = jsonDeserializer;
            _dispatcher = dispatcher;

            Startup.RunConsole();

            PluginManager.LoadAllPlugins(args);
        }

        public static void ReceiveJson(string json)
        {
            if (JsonReceived == null)
            {
                Logger.Error("未配置json解析");
                return;
            }

            JsonReceived.Invoke(null, new JsonReceivedEventArgs
            {
                JsonString = json
            });
        }

        public static void ReceiveMessage(Msg msg)
        {
            if (MessageReceived == null)
            {
                Logger.Error("未配置message解析");
                return;
            }

            MessageReceived.Invoke(null, new MessageReceivedEventArgs
            {
                MessageObj = msg
            });
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private static void LoadSecret()
        {
            var file = new FileInfo(Path.Combine(Domain.SecretPath, "secret.json"));
            string json;
            Secret secret;
            if (!file.Exists)
            {
                secret = new Secret();
                json = Newtonsoft.Json.JsonConvert.SerializeObject(secret);
                ConcurrentFile.WriteAllText(file.FullName, json.ToJsonFormat());

                Logger.Error("请完善secret配置。");
                SysConsole.ReadKey();
                Environment.Exit(0);
            }

            json = ConcurrentFile.ReadAllText(file.FullName);
            secret = Newtonsoft.Json.JsonConvert.DeserializeObject<Secret>(json);

            // 读设置
            DbHelper.ConnectionString.Add("cabbage", secret.ConnectionStrings.DefaultConnection);
            DbHelper.ConnectionString.Add("daylily", secret.ConnectionStrings.MyConnection);

            OsuApiKey.ApiKey = secret.OsuSettings.ApiKey;
            OsuApiKey.UserName = secret.OsuSettings.UserName;
            OsuApiKey.Password = secret.OsuSettings.Password;

            Signature.AppId = secret.CosSettings.AppId;
            Signature.SecretId = secret.CosSettings.SecretId;
            Signature.SecretKey = secret.CosSettings.SecretKey;
            Signature.BucketName = secret.CosSettings.BucketName;

            CqApi.ApiUrl = secret.BotSettings.PostUrl;
            CqCode.CqPath = secret.BotSettings.CqDir;
            CoolQDispatcher.CommandFlag = secret.BotSettings.CommandFlag;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        private static void CreateDirectories()
        {
            Type t = typeof(Domain);
            var infos = t.GetProperties();
            foreach (var item in infos)
            {
                try
                {
                    SysConsole.WriteLine(item.Name);
                    string path = item.GetValue(null, null) as string;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    Logger.Error("未创建" + item.Name);
                }
            }
        }
    }
}
