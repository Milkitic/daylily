using System.Windows;
using daylily;
using daylily.ThirdParty.Tuling;
using Microsoft.Extensions.DependencyInjection;
using MilkiBotFramework.Aspnetcore;
using MilkiBotFramework.Imaging.Wpf.Internal;
using MilkiBotFramework.Platforms.GoCqHttp;
using NLog.Extensions.Logging;

UiThreadHelper.GetApplicationFunc = () =>
{
    var application = new App();
    var resourceLocater = new Uri("/daylily;component/app.xaml", UriKind.Relative);
    Application.LoadComponent(application, resourceLocater);
    return application;
};

return await new AspnetcoreBotBuilder()
    .ConfigureServices(k => k.AddSingleton<TulingClient>())
    .ConfigureLogger(k => k
#if DEBUG
            .AddNLog("NLog.debug.config")
#else
            .AddNLog("NLog.config")
#endif

    )
    .UseGoCqHttp()
    .UseCommandLineAnalyzer<MyCommandLineAnalyzer>(new GoCqParameterConverter())
    .Build()
    .RunAsync();