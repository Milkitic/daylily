using Daylily.Bot.Backend.Plugins;
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
    public class PluginManager
    {
        public static PluginManager Current { get; private set; }

        public PluginManager()
        {
            Current = this;
        }

        public IEnumerable<TaggedClass<CommandPlugin>> Commands =>
            TaggedPlugins
                .Where(k => k.Instance.PluginType == PluginType.Command)
                .Select(k => new TaggedClass<CommandPlugin>(k.Tag, (CommandPlugin)k.Instance));

        public IEnumerable<CommandPlugin> CommandInstances =>
            Commands
                .Select(k => k.Instance)
                .Distinct();

        public IEnumerable<ApplicationPlugin> ApplicationInstances =>
            TaggedPlugins
                  .Where(k => k.Instance.PluginType == PluginType.Application)
                  .Select(k => (ApplicationPlugin)k.Instance);

        public IEnumerable<ServicePlugin> ServiceInstances =>
            TaggedPlugins
                .Where(k => k.Instance.PluginType == PluginType.Service)
                .Select(k => (ServicePlugin)k.Instance);

        public IEnumerable<Plugin> Plugins =>
            TaggedPlugins
                .Select(k => k.Instance)
                .Distinct();

        protected List<TaggedClass<Type>> CachedCommands { get; } = new List<TaggedClass<Type>>();
        protected List<TaggedClass<Plugin>> TaggedPlugins { get; } = new List<TaggedClass<Plugin>>();
        protected List<TaggedClass<Assembly>> Assemblies { get; } = new List<TaggedClass<Assembly>>();

        protected static readonly string BackendDirectory = Domain.PluginPath;
        protected static readonly string ExtendedDirectory = Domain.ExtendedPluginPath;

        public bool ContainsPlugin(string command)
        {
            return Commands.Any(k => k.Tag == command);
        }

        public CommandPlugin GetPlugin(string command)
        {
            return Commands.FirstOrDefault(k => k.Tag == command).Instance;
        }

        public T GetNewInstance<T>() where T : Plugin
        {
            return Activator.CreateInstance(typeof(T)) as T;
        }
        public T GetNewInstance<T>(Type pluginType) where T : Plugin
        {
            return Activator.CreateInstance(pluginType) as T;
        }

        public Type GetPluginType(string command)
        {
            return CachedCommands.FirstOrDefault(k => k.Tag == command).Instance;
        }

        public T GetPlugin<T>() where T : Plugin
        {
            return (T)TaggedPlugins.FirstOrDefault(k => k.Instance.GetType() == typeof(T)).Instance;
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
                if (typeof(T) != item.Instance.GetType()) continue;
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
                        Assemblies.Add(new TaggedClass<Assembly>(fi.Name, asm));
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
                        if (cmdPlugin.Commands != null && cmdPlugin.Commands.Length > 0)
                        {
                            foreach (var cmd in cmdPlugin.Commands)
                            {
                                TaggedPlugins.Add(new TaggedClass<Plugin>(cmd, cmdPlugin));
                                CachedCommands.Add(new TaggedClass<Type>(cmd, type));
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
                        TaggedPlugins.Add(new TaggedClass<Plugin>(null, plugin));
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
    }

}
