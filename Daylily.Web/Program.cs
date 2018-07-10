using System;
using Daylily.Common.Function;
using Daylily.Common.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Daylily.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application;
            Logger.Raw(@".__       . .   
|  \ _.  .|*|  .
|__/(_]\_||||\_|
       ._|   ._|");
            Logger.Raw($"{app.ApplicationName.Split('.')[0]} {app.ApplicationVersion} based on {app.RuntimeFramework}");
            PluginManager.LoadAllPlugins(args);
            BuildWebHost(args).Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:23333")
                .Build();
    }
}
