using daylily.ThirdParty.Tuling;
using daylily.Utils;
using Microsoft.Extensions.DependencyInjection;
using MilkiBotFramework.Aspnetcore;
using MilkiBotFramework.Platforms.GoCqHttp;
using NLog.Extensions.Logging;

var key = "Template@Key";
var iv = "Template@Iv";
var nowTicks = DateTime.Now.Ticks;
var state = EncryptUtil.EncryptAes256UseMd5(114514 + "|" + nowTicks,
    key, iv);
var state2 = EncryptUtil.DecryptAes256UseMd5(state,
    key, iv);

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