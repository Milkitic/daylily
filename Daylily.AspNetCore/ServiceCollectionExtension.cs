using Daylily.Bot;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Daylily.AspNetCore
{
    public static class ServiceCollectionExtension
    {
        public static void AddDaylily(this IServiceCollection services, StartupConfig startupConfig, Action configCallback = null)
        {
            var bot = new DaylilyCore(startupConfig, configCallback);
            bot.ConfigDispatcher(startupConfig.MessageDispatcher, startupConfig.GeneralDispatcherConfig);
            foreach (var frontend in startupConfig.Frontends)
            {
                bot.AddFrontend(frontend, startupConfig.GeneralFrontendsConfig);
            }
            services.AddSingleton(bot);
        }
    }
}
