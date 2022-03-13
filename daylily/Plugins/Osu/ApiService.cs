using System.Diagnostics.CodeAnalysis;
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Osu;

[PluginIdentifier("a1332794-448f-4378-a8b4-209428204af1")]
public class ApiService : ServicePlugin
{
    private readonly BotTaskScheduler _taskScheduler;
    private readonly OsuConfig _config;

    public ApiService(IConfiguration<OsuConfig> configuration, BotTaskScheduler taskScheduler)
    {
        _taskScheduler = taskScheduler;
        _config = configuration.Instance;
    }

    public UserToken? PublicToken { get; private set; }

    internal bool TryAccessApi<T>(TokenBase token, Func<OsuClientV2, T> func,
        [NotNullWhen(true)] out T? result,
        [NotNullWhen(false)] out IResponse? accessErrorMessage)
    {
        try
        {
            result = func.Invoke(new OsuClientV2(token))!;
            accessErrorMessage = null;
            return true;
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains("401"))
            {
                accessErrorMessage = Reply("osu授权出错，请重新绑定osu ID");
            }
            else if (ex.Message.Contains("404"))
            {
                accessErrorMessage = Reply("无结果，请检查参数");
            }
            else
            {
                accessErrorMessage = Reply("osu接口访问出错：" + ex.Message);
            }

            result = default;
            return false;
        }
    }

    protected override async Task OnInitialized()
    {
        _taskScheduler.AddTask("RefreshOsuToken", k => k
             .ByInterval(TimeSpan.FromHours(12))
             .AtStartup()
             .WithoutLogging()
             .Do(RefreshTokenTask));
    }

    private void RefreshTokenTask(TaskContext context, CancellationToken token)
    {
        var authClientPub = new AuthorizationClient();
        var result = authClientPub.GetPublicToken(_config.ClientId, _config.ClientSecret).Result;
        this.PublicToken = result;
        context.Logger.LogInformation($"已刷新Osu!PublicToken，将在{DateTime.Now.AddSeconds(result.ExpiresIn)}过期");
    }
}