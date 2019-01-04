using Daylily.Bot.Backend;
using Daylily.Bot.Backend.Plugins;
using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Daylily.CoolQ
{
    public class CoolQPluginManager
    {
      

        //private void LoadExtend()
        //{
        //    if (!Directory.Exists(ExtendedDirectory))
        //        Directory.CreateDirectory(ExtendedDirectory);

        //    foreach (var dir in Directory.GetDirectories(ExtendedDirectory))
        //    {
        //        string metaFile = Path.Combine(dir, "metadata.json");
        //        if (!File.Exists(metaFile))
        //        {
        //            Logger.Error(dir + "内未包含metadata.json");
        //            continue;
        //        }
        //        Logger.Info("已发现 " + new DirectoryInfo(dir).Name);

        //        string json = ConcurrentFile.ReadAllText(metaFile);
        //        ExtendMeta extendMeta = JsonConvert.DeserializeObject<ExtendMeta>(json);

        //        ExtendPlugin extendPlugin = new ExtendPlugin()
        //        {
        //            Program = extendMeta.Program,
        //            File = new FileInfo(Path.Combine(dir, extendMeta.File)).FullName,
        //            Name = extendMeta.Name,
        //            Author = extendMeta.Author,
        //            Major = extendMeta.Major,
        //            Minor = extendMeta.Minor,
        //            Patch = extendMeta.Patch,
        //            State = extendMeta.State,
        //            Helps = extendMeta.Help,
        //            Commands = extendMeta.Command,
        //        };

        //        if (extendPlugin.Commands.Length != 0)
        //        {
        //            foreach (var item in extendPlugin.Commands)
        //            {
        //                CommandMap.TryAdd(item, extendPlugin.GetType());
        //                CommandMapStatic.TryAdd(item, extendPlugin);
        //            }
        //            Logger.Origin($"命令 \"{extendMeta.Name}\" ({string.Join(',', extendPlugin.Commands)}) 已经加载完毕。");

        //        }
        //        else
        //        {
        //            Logger.Warn($"\"{extendMeta.Name}\"尚未设置命令，因此无法被用户激活。");
        //            Logger.Origin($"命令 \"{extendMeta.Name}\" 已经加载完毕。");
        //        }
        //    }
        //}
        

      
    }
}
