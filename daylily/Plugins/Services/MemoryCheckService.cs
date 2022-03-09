using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Services
{
    [PluginIdentifier("11CCA200-61EF-49DF-9E30-C48C3AD69BE5")]
    public class MemoryCheckService : ServicePlugin
    {
        private readonly Bot _bot;
        private readonly BotTaskScheduler _taskScheduler;

        public MemoryCheckService(Bot bot, BotTaskScheduler taskScheduler)
        {
            _bot = bot;
            _taskScheduler = taskScheduler;
        }

        protected override async Task OnInitialized()
        {
            var preSize = 0L;

            await Task.Delay(TimeSpan.FromSeconds(3));
            _taskScheduler.AddTask("内存溢出检测", k => k
                .AtStartup()
                .WithoutLogging()
                .ByInterval(TimeSpan.FromSeconds(5))
                .Do((context, token) =>
                {
                    using var proc = Process.GetCurrentProcess();
                    var logger = context.Logger;
                    var mem = proc.PrivateMemorySize64;
                    if (preSize != 0L)
                    {
                        var value = mem - preSize;
                        if (value > 256 * 1024)
                            logger.LogDebug($"Memory changed by {SizeSuffix(value)} to {SizeSuffix(mem)} in 3 sec");
                    }

                    preSize = mem;

                    var max = 1024L * 1024L * 1024L * 2;
                    if (mem > max)
                    {
                        logger.LogWarning($"Memory reaches max to {SizeSuffix(max)}, the application will shutdown.");
                        _bot.Stop(-114514);
                        //Environment.Exit(-114514);
                    }
                    else if (mem > max * 0.85d)
                    {
                        logger.LogWarning($"Memory reaches 85% max to {SizeSuffix(max)} by {SizeSuffix(mem)}!");
                    }
                })
            );
        }

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }
    }
}
