using Daylily.Bot.Backend.Plugins;
using Daylily.Bot.Models;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daylily.Bot.Backend
{
    public abstract class PluginManager
    {
        public struct TaggedPlugin
        {
            public TaggedPlugin(string tag, Plugin plugin) : this()
            {
                Tag = tag;
                Plugin = plugin;
            }

            public string Tag { get; set; }
            public Plugin Plugin { get; set; }
        }

        public struct TaggedAssembly
        {
            public TaggedAssembly(string tag, Assembly assembly) : this()
            {
                Tag = tag;
                Assembly = assembly;
            }

            public string Tag { get; set; }
            public Assembly Assembly { get; set; }
        }

        public IEnumerable<TaggedPlugin> Commands =>
            TaggedPlugins.Where(k => k.Plugin.PluginType == PluginType.Command);
        public IEnumerable<Plugin> Applications =>
            TaggedPlugins.Where(k => k.Plugin.PluginType == PluginType.Application).Select(k => k.Plugin);
        public IEnumerable<Plugin> Services =>
            TaggedPlugins.Where(k => k.Plugin.PluginType == PluginType.Service).Select(k => k.Plugin);

        protected List<TaggedPlugin> TaggedPlugins { get; set; }
        protected List<TaggedAssembly> Assemblies { get; set; }

        protected static readonly string BackendDirectory = Domain.PluginPath;
        protected static readonly string ExtendedDirectory = Domain.ExtendedPluginPath;

        public T GetPlugin<T>() where T : Plugin
        {
            return (T)TaggedPlugins.FirstOrDefault(k => k.Plugin.GetType() == typeof(T)).Plugin;
        }

        public void AddPlugin<T>(StartupConfig startupConfig)
        {
            Type type = typeof(T);
            InsertPlugin(type, startupConfig);
        }

        public void RemovePlugin<T>()
        {
            foreach (var item in TaggedPlugins)
            {
                if (typeof(T) != item.Plugin.GetType()) continue;
                TaggedPlugins.Remove(item);
            }
        }

        public virtual void LoadPlugins(StartupConfig startupConfig)
        {
            Type[] supportedTypes =
            {
                typeof(CommandPlugin),
                typeof(ApplicationPlugin),
                typeof(ServicePlugin)
            };

            foreach (var item in Directory.GetFiles(BackendDirectory, "*.dll"))
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
                            if (supportedTypes.Any(supported => type.IsSubclassOf(supported)))
                            {
                                typeName = type.Name;
                                InsertPlugin(type, startupConfig);

                                isValid = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(typeName + " 抛出了未处理的异常。");
                            Logger.Exception(ex);
                        }
                    }

                    if (isValid)
                        Assemblies.Add(new TaggedAssembly(fi.Name, asm));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                if (!isValid)
                    Logger.Warn($"\"{fi.Name}\" 不是合法的插件扩展。");
            }
        }

        protected virtual void InsertPlugin(Type type, StartupConfig startupConfig)
        {
            try
            {
                Plugin plugin = (Plugin)Activator.CreateInstance(type);
                string pluginType, error = "", commands = "";
                switch (plugin.PluginType)
                {
                    case PluginType.Command:
                        pluginType = "命令";
                        CommandPlugin cmdPlugin = (CommandPlugin)plugin;
                        CachedCommands.Add(type);
                        if (cmdPlugin.Commands != null && cmdPlugin.Commands.Length > 0)
                        {
                            foreach (var cmd in cmdPlugin.Commands)
                            {
                                TaggedPlugins.Add(new TaggedPlugin(cmd, cmdPlugin));
                            }

                            commands = $"({string.Join(",", cmdPlugin.Commands)}) ";
                        }
                        else
                        {
                            error = "但此命令插件未设置命令。";
                        }

                        break;
                    case PluginType.Unknown:
                        throw new NotSupportedException();
                    case PluginType.Application:
                    case PluginType.Service:
                    default:
                        pluginType = plugin.PluginType == PluginType.Application ? "应用" : "服务";
                        TaggedPlugins.Add(new TaggedPlugin(null, plugin));
                        break;
                }

                plugin.OnInitialized(null);
                Logger.Origin($"{pluginType} \"{plugin.Name}\" {commands}已经加载完毕。{error}");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex.InnerException ?? ex);
                Logger.Error($"加载插件{type.Name}失败。");
            }
        }

        public List<Type> CachedCommands { get; set; }
    }

}
