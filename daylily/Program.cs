using daylily.Tuling;
using Microsoft.Extensions.DependencyInjection;
using MilkiBotFramework.Aspnetcore;
using MilkiBotFramework.Platforms.GoCqHttp;
using NLog.Extensions.Logging;

return await new AspnetcoreBotBuilder()
    .UseGoCqHttp()
    .ConfigureServices(k => k.AddSingleton<TulingClient>())
    .ConfigureLogger(k => k
#if DEBUG
            .AddNLog("NLog.debug.config")
#else
            .AddNLog("NLog.config")
#endif

    )
    .Build()
    .RunAsync();