using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.AspNetCore
{
    public static class ServiceCollectionExtension
    {
        public static void AddDaylily(this IServiceCollection services, Bot.Config config)
        {
            var bot = new Bot.Core(config);
            bot.ConfigDispatcher(config.Dispatcher, obj => { });
            foreach (var frontend in config.Frontends)
            {
                bot.AddFrontend(frontend, obj => { });
            }
            services.AddSingleton(bot);
        }
    }
}
