using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Bot;
using Microsoft.Extensions.Hosting;

namespace Daylily.AspNetCore
{
    public static class ServiceCollectionExtension
    {
        public static void AddDaylily(this IServiceCollection services, StartupConfig startupConfig)
        {
            var bot = new DaylilyCore(startupConfig);
            bot.ConfigDispatcher(startupConfig.Dispatcher, startupConfig.GeneralDispatcherConfig);
            foreach (var frontend in startupConfig.Frontends)
            {
                bot.AddFrontend(frontend, startupConfig.GeneralFrontendsConfig);
            }
            services.AddSingleton(bot);
        }
    }
}
