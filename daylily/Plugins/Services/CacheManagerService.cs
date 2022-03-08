using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Services
{
    [PluginIdentifier("689efebb-9642-4ba1-bad8-8e74e0f91200", "缓存定时清理")]
    public sealed class CacheManagerService : ServicePlugin
    {
        private readonly ILogger<CacheManagerService> _logger;
        private readonly BotTaskScheduler _scheduler;
        private readonly BotOptions _botOptions;

        public CacheManagerService(ILogger<CacheManagerService> logger, BotOptions botOptions, BotTaskScheduler scheduler)
        {
            _logger = logger;
            _botOptions = botOptions;
            _scheduler = scheduler;
        }

        protected override async Task OnInitialized()
        {
            _scheduler.AddTask("清理图片缓存", k => k
                .AtStartup()
                .ByInterval(TimeSpan.FromHours(0.5))
                .Do(DoCleanTask)
            );
        }

        private void DoCleanTask(TaskContext context, CancellationToken token)
        {
            _logger.LogInformation("开始清理图片缓存……");
            ClearCache(_botOptions.CacheImageDir, _logger);
            _logger.LogInformation("清理图片缓存完毕，下次清理时间：" + context.NextTriggerTimes.First());
        }

        private static void ClearCache(string path, ILogger logger)
        {
            var folder = new DirectoryInfo(path);
            if (!folder.Exists) return;
            var files = folder.EnumerateFiles();
            foreach (var file in files)
            {
                try
                {
                    if (DateTime.Now - file.LastWriteTime > TimeSpan.FromMinutes(30))
                        file.Delete();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "清理文件时出错");
                }
            }

            var folders = folder.EnumerateDirectories();
            foreach (var f in folders)
            {
                ClearCache(f.FullName, logger);
                try
                {
                    if (DateTime.Now - f.LastWriteTime > TimeSpan.FromMinutes(30))
                        f.Delete();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "清理目录时出错");
                }
            }
        }
    }
}
