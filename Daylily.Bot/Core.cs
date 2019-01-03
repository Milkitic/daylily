using Daylily.Bot.Interface;
using Daylily.Bot.Models;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SysConsole = System.Console;

namespace Daylily.Bot
{
    public class Core
    {
        private readonly List<IFrontend> _frontends = new List<IFrontend>();
        public IEnumerable<IFrontend> Frontends => _frontends;

        private IDispatcher _dispatcher;
        public IDispatcher Dispatcher => _dispatcher;

        public Core(StartupConfig startupConfig)
        {
            Logger.Raw(@".__       . .   
|  \ _.  .|*|  .
|__/(_]\_||||\_|
       ._|   ._|");
            var str = string.Format("{0} {1} based on {2}",
                startupConfig.ApplicationMetadata.ApplicationName.Split('.')[0],
                startupConfig.ApplicationMetadata.BotVersion.ToString().TrimEnd('.', '0'),
                startupConfig.ApplicationMetadata.FrameworkName.FullName);

            Logger.Raw(str);

            CreateDirectories(); // 创建目录
            LoadSecret(); // 加载配置

            PluginManager.LoadAllPlugins(startupConfig);
        }

        public IDispatcher ConfigDispatcher(IDispatcher dispatcher, Action<IDispatcher> config = null)
        {
            _dispatcher = dispatcher.Config(config);
            return dispatcher;
        }

        public IFrontend AddFrontend(IFrontend frontend, Action<IFrontend> config = null)
        {
            _frontends.Add(frontend.Config(config));
            return frontend;
        }

        public T GetFrontend<T>() where T : IFrontend
        {
            return (T)_frontends.FirstOrDefault(k => k.GetType() == typeof(T));
        }

        public IFrontend GetFrontend(Type type)
        {
            return _frontends.FirstOrDefault(k => k.GetType() == type);
        }

        public void RaiseRawObjectEvents(object obj)
        {
            int? priority = int.MinValue;
            bool handled = false;
            foreach (var frontend in Frontends.OrderByDescending(k => k.MiddlewareConfig?.Priority))
            {
                int? p = frontend.MiddlewareConfig?.Priority;
                if (p < priority && handled)
                {
                    break;
                }

                priority = frontend.MiddlewareConfig?.Priority;
                handled = frontend.RawObject_Received(obj);
            }
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
