using Daylily.Bot.Backend;
using Daylily.Bot.Dispatcher;
using Daylily.Bot.Frontend;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.Common.Utils.StringUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SysConsole = System.Console;

namespace Daylily.Bot
{
    public class DaylilyCore
    {
        public static DaylilyCore Current { get; private set; }
        private readonly List<IFrontend> _frontends = new List<IFrontend>();
        public IEnumerable<IFrontend> Frontends => _frontends;

        private IDispatcher _dispatcher;
        public IDispatcher Dispatcher => _dispatcher;
        public IMessageDispatcher MessageDispatcher
        {
            get
            {
                var dispatcher = _dispatcher as IMessageDispatcher;
                if (dispatcher == null)
                    Logger.Warn($"No implement class of {nameof(IMessageDispatcher)}.");
                return dispatcher;
            }
        }

        public IEventDispatcher EventDispatcher
        {
            get
            {
                var dispatcher = _dispatcher as IEventDispatcher;
                if (dispatcher == null)
                    Logger.Warn($"No implement class of {nameof(IEventDispatcher)}.");
                return dispatcher;
            }
        }

        public ISessionDispatcher SessionDispatcher
        {
            get
            {
                var dispatcher = _dispatcher as ISessionDispatcher;
                if (dispatcher == null)
                    Logger.Warn($"No implement class of {nameof(ISessionDispatcher)}.");
                return dispatcher;
            }
        }

        public PluginManager PluginManager { get; set; } = new PluginManager();

        public Random GlobalRandom { get; } = new Random();

        public string CommandFlag = "/";

        public DaylilyCore(StartupConfig startupConfig, Action configCallback)
        {
            Current = this;
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
            configCallback?.Invoke();

            PluginManager.LoadPlugins(startupConfig);
        }

        public IDispatcher ConfigDispatcher(IDispatcher messageDispatcher, Action<IDispatcher> config = null)
        {
            _dispatcher = messageDispatcher.Config(config);
            return messageDispatcher;
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
                    //SysConsole.WriteLine(item.Name);
                    string path = item.GetValue(null, null) as string;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Cannot create {item.Name}: {ex.Message}");
                }
            }
        }
    }
}
