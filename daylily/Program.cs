using Microsoft.Extensions.Logging;
using MilkiBotFramework.Aspnetcore;
using MilkiBotFramework.Platforms.GoCqHttp;
using NLog.Extensions.Logging;

return await new AspnetcoreBotBuilder()
    .UseGoCqHttp()
    .ConfigureLogger(k => k
#if DEBUG
            .AddNLog("NLog.debug.config")
#else
            .AddNLog("NLog.config")
#endif

    )
    .Build()
    .RunAsync();