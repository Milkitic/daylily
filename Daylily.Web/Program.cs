using Daylily.Bot;
using Daylily.Common.Utils.LoggerUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Daylily.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application;
            args = new[] { $"{app.ApplicationName.Split('.')[0]} {app.ApplicationVersion.Remove(app.ApplicationVersion.Length - 2)} based on {app.RuntimeFramework}" };
            Logger.Raw(@".__       . .   
|  \ _.  .|*|  .
|__/(_]\_||||\_|
       ._|   ._|");
            Logger.Raw($"{app.ApplicationName.Split('.')[0]} {app.ApplicationVersion} based on {app.RuntimeFramework}");
            Core.InitCore(new CoolQJsonDeserializer(), new CoolQDispatcher());
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:23333");
    }
}
