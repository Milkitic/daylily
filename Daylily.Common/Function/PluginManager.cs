using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Function.Application;
using Daylily.Common.Function.Application.Command;
using Daylily.Common.IO;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Extension;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;
using Daylily.Common.Utils.LogUtils;
using Newtonsoft.Json;

namespace Daylily.Common.Function
{
    public static class PluginManager
    {
        //public static ConcurrentDictionary<string, CommandApp> CommandMap { get; } = new ConcurrentDictionary<string, CommandApp>();
        public static ConcurrentDictionary<string, Type> CommandMap { get; } =
            new ConcurrentDictionary<string, Type>();
        public static ConcurrentDictionary<string, CommandApp> CommandMapStatic { get; } =
            new ConcurrentDictionary<string, CommandApp>();

        public static List<ServiceApp> ServiceList { get; } = new List<ServiceApp>();
        public static List<ApplicationApp> ApplicationList { get; } = new List<ApplicationApp>();

        public static ConcurrentDictionary<string, Assembly> AssemblyList { get; } =
            new ConcurrentDictionary<string, Assembly>();

        private static readonly string PluginDir = Path.Combine(Domain.CurrentDirectory, "plugins");
        private static readonly string ExtendedDir = Path.Combine(PluginDir, "extended");

        public static void LoadAllPlugins(string[] args)
        {
            Logger.Info("===加载内部插件中==");
            LoadBuiltIn(args);
            Logger.Info("===加载外部插件中==");
            LoadFromFile(args);
            Logger.Info("===加载扩展插件中==");
            LoadExtend();
        }

        private static void LoadBuiltIn(string[] args)
        {
            Type[] iType =
            {
                typeof(CheckCqAt),
                //typeof(DragonDetectorAlpha),
                typeof(PandaDetector),
                typeof(PornDetector),
                typeof(Repeat),
                typeof(GroupQuiet),
                typeof(KeywordTrigger),

                typeof(MyGraveyard),
                typeof(Kudosu),
                typeof(Panda),
                typeof(Rcon),
                typeof(Send),
                typeof(Shutdown),
                typeof(Plugin),
            };

            foreach (var item in iType)
            {
                InsertPlugin(item, args);
            }
        }

        private static void LoadFromFile(string[] args)
        {
            foreach (var item in Directory.GetFiles(PluginDir, "*.dll"))
            {
                bool isValid = false;
                FileInfo fi = new FileInfo(item);
                try
                {
                    Logger.Info("已发现" + fi.Name);

                    //Assembly asm = Assembly.LoadFile(fi.FullName);
                    Assembly asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(fi.FullName);
                    // 目前无Unload

                    Type[] t = asm.GetExportedTypes();
                    foreach (Type type in t)
                    {
                        string typeName = "";
                        try
                        {
                            if (type.BaseType.BaseType != typeof(AppConstruct)) continue;
                            typeName = type.Name ?? "";
                            InsertPlugin(type, args);

                            isValid = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(typeName + " 抛出了未处理的异常。");
                            Logger.Exception(ex);
                        }
                    }

                    if (isValid)
                        AssemblyList.GetOrAdd(fi.Name, asm);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                if (!isValid)
                    Logger.Warn($"\"{fi.Name}\" 不是合法的插件扩展。");
            }
        }

        private static void LoadExtend()
        {
            if (!Directory.Exists(ExtendedDir))
                Directory.CreateDirectory(ExtendedDir);

            foreach (var dir in Directory.GetDirectories(ExtendedDir))
            {
                string metaFile = Path.Combine(dir, "metadata.json");
                if (!File.Exists(metaFile))
                {
                    Logger.Error(dir + "内未包含metadata.json");
                    continue;
                }
                Logger.Info("已发现 " + new DirectoryInfo(dir).Name);

                string json = ConcurrentFile.ReadAllText(metaFile);
                ExtendMeta extendMeta = JsonConvert.DeserializeObject<ExtendMeta>(json);

                ExtendApp extendApp = new ExtendApp()
                {
                    Program = extendMeta.Program,
                    File = new FileInfo(Path.Combine(dir, extendMeta.File)).FullName,
                    Name = extendMeta.Name,
                    Author = extendMeta.Author,
                    Major = extendMeta.Major,
                    Minor = extendMeta.Minor,
                    Patch = extendMeta.Patch,
                    State = extendMeta.State,
                    Helps = extendMeta.Help,
                    Commands = extendMeta.Command,
                };

                if (extendApp.Commands.Length != 0)
                {
                    foreach (var item in extendApp.Commands)
                    {
                        CommandMap.TryAdd(item, extendApp.GetType());
                        CommandMapStatic.TryAdd(item, extendApp);
                    }
                    Logger.Origin($"命令 \"{extendMeta.Name}\" ({string.Join(',', extendApp.Commands)}) 已经加载完毕。");

                }
                else
                {
                    Logger.Warn($"\"{extendMeta.Name}\"尚未设置命令，因此无法被用户激活。");
                    Logger.Origin($"命令 \"{extendMeta.Name}\" 已经加载完毕。");
                }
            }
        }

        public static void RemovePlugin<T>()
        {
            foreach (var item in CommandMap)
            {
                if (typeof(T) != item.Value.GetType()) continue;
                CommandMap.Remove(item.Key, out _);
            }

            foreach (var item in ServiceList)
            {
                if (typeof(T) != item.GetType()) continue;
                ServiceList.Remove(item);
            }

            foreach (var item in ApplicationList)
            {
                if (typeof(T) != item.GetType()) continue;
                ApplicationList.Remove(item);
            }
        }

        public static void AddPlugin<T>(string[] args)
        {
            Type type = typeof(T);
            AppConstruct plugin = Activator.CreateInstance(type) as AppConstruct;
            InsertPlugin(plugin, args);
        }

        private static void InsertPlugin(Type type, string[] args)
        {
            try
            {
                AppConstruct plugin = Activator.CreateInstance(type) as AppConstruct;
                if (plugin.AppType != AppType.Command)
                {
                    InsertPlugin(plugin, args);
                }
                else
                {
                    CommandApp cmdPlugin = (CommandApp)plugin;
                    cmdPlugin.Initialize(args);
                    string str = "";
                    if (cmdPlugin.Commands != null)
                    {
                        str = "(";
                        foreach (var cmd in cmdPlugin.Commands)
                        {
                            //CommandMap.TryAdd(cmd, (CommandApp)plugin);
                            CommandMap.TryAdd(cmd, type);
                            CommandMapStatic.TryAdd(cmd, cmdPlugin);
                            str += cmd + ",";
                        }

                        str = str.TrimEnd(',') + ") ";
                    }

                    Logger.Origin($"命令 \"{plugin.Name}\" {str}已经加载完毕。");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex.InnerException ?? ex);
            }
        }

        private static void InsertPlugin(AppConstruct plugin, string[] args)
        {
            switch (plugin.AppType)
            {
                case AppType.Application:
                    ApplicationList.Add((ApplicationApp)plugin);
                    Logger.Origin($"应用 \"{plugin.Name}\" 已经加载完毕。");
                    break;
                case AppType.Service:
                default:
                    ServiceApp svcPlugin = (ServiceApp)plugin;
                    Task.Run(() => { svcPlugin.RunTask(args); });
                    ServiceList.Add(svcPlugin);
                    Logger.Origin($"服务 \"{svcPlugin.Name}\" 已经加载完毕。");
                    break;
            }
        }
    }
}
