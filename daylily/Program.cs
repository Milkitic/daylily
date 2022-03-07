using Microsoft.Extensions.Logging;
using MilkiBotFramework.Aspnetcore;
using MilkiBotFramework.Platforms.GoCqHttp;

return await new AspnetcoreBotBuilder(args, "http://0.0.0.0:23333")
    //.UseGoCqHttp("ws://127.0.0.1:6700")
    .UseGoCqHttp(GoCqConnection.ReverseWebSocket("/connector/reversews"))
    //.UseGoCqHttp(GoCqConnection.WebSocket("ws://127.0.0.1:6700"))
    .ConfigureLogger(k => k
        .AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            //options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss.ffzz ";
        })
        .AddFilter((ns, level) =>
        {
            if (ns.StartsWith("Microsoft") && level < LogLevel.Warning)
                return false;
            if (level < LogLevel.Information)
                return false;
            return true;

            //if (ns.StartsWith("Microsoft") && level < LogLevel.Information)
            //    return false;
            //return true;
        })
    )
    .Build()
    .RunAsync();