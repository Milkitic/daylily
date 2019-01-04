using Daylily.Bot.Backend.Plugins;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daylily.Plugin.Core.Service
{
    public class CacheManageService : ServicePlugin
    {
        public override void OnInitialized(string[] args)
        {
            base.OnInitialized(args);

            Task.Run(() =>
            {
                while (true)
                {
                    ClearCache(Domain.CacheImagePath);
                    Thread.Sleep(1000 * 60 * 30);
                }
            });
        }

        private static void ClearCache(string path)
        {
            var folder = new DirectoryInfo(path);
            var files = folder.EnumerateFiles();
            foreach (var file in files)
            {
                try
                {
                    if (DateTime.Now - file.CreationTime > new TimeSpan(0, 30, 0))
                        file.Delete();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            var folders = folder.EnumerateDirectories();
            foreach (var f in folders)
            {
                ClearCache(f.FullName);
                try
                {
                    if (DateTime.Now - f.CreationTime > new TimeSpan(0, 30, 0))
                        f.Delete();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }
    }
}
