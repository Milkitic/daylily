using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Daylily.Bot.Enum;
using Daylily.Bot.Models;
using Daylily.Bot.PluginBase;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Newtonsoft.Json;

namespace Daylily.Bot
{
    public static class PluginManager
    {
        public static ConcurrentDictionary<string, Type> CommandMap { get; } =
            new ConcurrentDictionary<string, Type>();
        public static ConcurrentDictionary<string, CommandPlugin> CommandMapStatic { get; } =
            new ConcurrentDictionary<string, CommandPlugin>();

        public static List<ServicePlugin> ServiceList { get; } = new List<ServicePlugin>();
        public static List<ApplicationPlugin> ApplicationList { get; } = new List<ApplicationPlugin>();

        public static ConcurrentDictionary<string, Assembly> AssemblyList { get; } =
            new ConcurrentDictionary<string, Assembly>();

        private static readonly string PluginDir = Domain.PluginPath;
        private static readonly string ExtendedDir = Domain.ExtendedPluginPath;

        public static void LoadAllPlugins(StartupConfig startupConfig)
        {
            //Logger.Info("===加载内部插件中==");
            //LoadBuiltIn(args);
            Logger.Info("===加载外部插件中==");
            LoadFromFile(startupConfig);
            Logger.Info("===加载扩展插件中==");
            LoadExtend();
        }

        private static void LoadBuiltIn(StartupConfig startupConfig)
        {
            Type[] iType =
            {

            };

            foreach (var item in iType)
            {
                InsertPlugin(item, startupConfig);
            }
        }

        private static void LoadFromFile(StartupConfig startupConfig)
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
                            if (type.BaseType.BaseType != typeof(PluginBase.Plugin)) continue;
                            typeName = type.Name ?? "";
                            InsertPlugin(type, startupConfig);

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

                ExtendPlugin extendPlugin = new ExtendPlugin()
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

                if (extendPlugin.Commands.Length != 0)
                {
                    foreach (var item in extendPlugin.Commands)
                    {
                        CommandMap.TryAdd(item, extendPlugin.GetType());
                        CommandMapStatic.TryAdd(item, extendPlugin);
                    }
                    Logger.Origin($"命令 \"{extendMeta.Name}\" ({string.Join(',', extendPlugin.Commands)}) 已经加载完毕。");

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

        public static void AddPlugin<T>(StartupConfig startupConfig)
        {
            Type type = typeof(T);
            PluginBase.Plugin plugin = Activator.CreateInstance(type) as PluginBase.Plugin;
            InsertPlugin(plugin, startupConfig);
        }

        private static void InsertPlugin(Type type, StartupConfig startupConfig)
        {
            try
            {
                PluginBase.Plugin plugin = Activator.CreateInstance(type) as PluginBase.Plugin;
                if (plugin.PluginType != PluginType.Command)
                {
                    InsertPlugin(plugin, startupConfig);
                }
                else
                {
                    CommandPlugin cmdPlugin = (CommandPlugin)plugin;
                    cmdPlugin.OnInitialized(null);
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
                Logger.Error($"加载插件{type.Name}失败。");
            }
        }

        private static void InsertPlugin(PluginBase.Plugin plugin, StartupConfig startupConfig)
        {
            switch (plugin.PluginType)
            {
                case PluginType.Application:
                    ApplicationList.Add((ApplicationPlugin)plugin);
                    Logger.Origin($"应用 \"{plugin.Name}\" 已经加载完毕。");
                    break;
                case PluginType.Service:
                default:
                    ServicePlugin svcPlugin = (ServicePlugin)plugin;
                    svcPlugin.Execute(null);
                    ServiceList.Add(svcPlugin);
                    Logger.Origin($"服务 \"{svcPlugin.Name}\" 已经加载完毕。");
                    break;
            }
        }
    }
}
