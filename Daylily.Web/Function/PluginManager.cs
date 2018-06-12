using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Web.Function.Application;
using Daylily.Web.Function.Application.Command;

namespace Daylily.Web.Function
{
    public static class PluginManager
    {
        public static Dictionary<string, AppConstruct> CommandMap { get; set; } =
            new Dictionary<string, AppConstruct>();

        public static List<AppConstruct> ServiceList { get; set; } = new List<AppConstruct>();
        public static List<AppConstruct> ApplicationList { get; set; } = new List<AppConstruct>();

        public static Dictionary<string, Assembly> AssemblyList { get; set; } = new Dictionary<string, Assembly>();

        private static readonly string PluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        public static void LoadAllPlugins(string[] args)
        {
            Type[] iType =
            {
                typeof(CheckCqAt),
                //typeof(DragonDetectorAlpha),
                typeof(PandaDetectorAlpha),
                typeof(PornDetector),
                typeof(Repeat),

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

            foreach (var item in Directory.GetFiles(PluginDir, "*.dll"))
            {
                bool isValid = false;
                FileInfo fi = new FileInfo(item);
                try
                {
                    Logger.PrimaryLine("已发现" + fi.Name);

                    //Assembly asm = Assembly.LoadFile(fi.FullName);
                    Assembly asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(fi.FullName);
                    // 目前无Unload

                    Type[] t = asm.GetExportedTypes();
                    foreach (Type type in t)
                    {
                        string typeName = "";
                        try
                        {
                            if (type.BaseType != typeof(AppConstruct)) continue;

                            typeName = type.Name ?? "";
                            InsertPlugin(type, args);

                            isValid = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.DangerLine(typeName + " occurred an unexpected error.");
                            Logger.WriteException(ex);
                        }
                    }

                    if (isValid)
                        AssemblyList.Add(fi.Name, asm);
                }
                catch (Exception ex)
                {
                    Logger.DangerLine(ex.Message);
                }

                if (!isValid)
                    Logger.WarningLine($"\"{fi.Name}\" 不是合法的插件扩展。");

            }

            //throw new NotImplementedException();
        }

        public static void RemovePlugin<T>()
        {
            foreach (var item in CommandMap)
            {
                if (typeof(T) != item.Value.GetType()) continue;
                CommandMap.Remove(item.Key);
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
            AppConstruct plugin = Activator.CreateInstance(type) as AppConstruct;
            InsertPlugin(plugin, args);
        }

        private static void InsertPlugin(AppConstruct plugin, string[] args)
        {
            plugin.OnLoad(args);
            switch (plugin.AppType)
            {
                case AppType.Command:

                    if (plugin.Command == null)
                        Logger.WarningLine($"\"{plugin.Name}\" 没有设置命令！！");
                    else
                    {
                        string[] cmds = plugin.Command.Split(',');
                        foreach (var cmd in cmds)
                            CommandMap.Add(cmd, plugin);
                    }

                    Logger.WriteLine($"命令 \"{plugin.Name}\" ({plugin.Command}) 已经加载完毕。");
                    break;
                case AppType.Application:
                    ApplicationList.Add(plugin);
                    Logger.WriteLine($"应用 \"{plugin.Name}\" 已经加载完毕。");
                    break;
                default:
                    ServiceList.Add(plugin);
                    Logger.WriteLine($"服务 \"{plugin.Name}\" 已经加载完毕。");
                    break;
            }
        }
    }
}
