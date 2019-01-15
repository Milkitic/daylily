using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot.Backend.Plugins;
using Daylily.Common;
using Daylily.Common.Logging;

namespace Daylily.Plugin.Kernel
{
    public class CacheManageSvc : ServicePlugin
    {
        public override Guid Guid => new Guid("0e4d58ea-4e4f-41ae-a377-47b2c56b2e10");

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
