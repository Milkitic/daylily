using System.Net.Http;
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using daylily.Plugins.Osu.Data;
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
    public class ApiResult<T>
    {
        public ApiResult(string error)
        {
            Error = error;
        }

        public ApiResult(T? result)
        {
            Result = result;
            Success = true;
        }

        public bool Success { get; set; }
        public T? Result { get; set; }
        public string? Error { get; set; }
    }

    private readonly BotTaskScheduler _taskScheduler;
    private readonly OsuConfig _config;
    private readonly TaskCompletionSource _initialWait;

    internal string UnbindMessage => "你还没有绑定账号，请私聊我 \"/setid.osu\" 完成绑定。直接复制引号中的内容即可。";

    public ApiService(IConfiguration<OsuConfig> configuration, BotTaskScheduler taskScheduler)
    {
        _taskScheduler = taskScheduler;
        _config = configuration.Instance;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        _initialWait = new TaskCompletionSource();
        cts.Token.Register(() =>
        {
            _initialWait.TrySetResult();
            cts.Dispose();
        });
    }

    public UserToken? PublicToken { get; private set; }

    internal async Task<ApiResult<T>> TryAccessPublicApi<T>(Func<OsuClientV2, Task<T>> func)
    {
        try
        {
            if (PublicToken == null)
                await _initialWait.Task;
            if (PublicToken == null)
                return new ApiResult<T>("PublicToken未初始化");
            return new ApiResult<T>(await func.Invoke(new OsuClientV2(PublicToken)));
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains("401"))
            {
                return new ApiResult<T>("osu授权出错，请重新绑定osu ID");
            }

            if (ex.Message.Contains("404"))
            {
                return new ApiResult<T>("无结果，请检查参数");
            }

            return new ApiResult<T>("osu!api访问出错：" + ex.Message);
        }
    }

    internal async Task<(bool, TokenBase? token, IResponse? unbindMessage)> TryGetToken(
        MessageContext messageContext, OsuDbContext dbContext)
    {
        var osuToken = await dbContext.Tokens.FindAsync(long.Parse(messageContext.MessageUserIdentity.UserId));
        if (osuToken == null)
        {
            return (false, null, Reply(UnbindMessage));
        }

        return (true, ConvertToken(osuToken), null);
    }

    internal async Task<(bool, long? osuId, IResponse? message)> TryGetUserId(
        MessageContext messageContext, OsuDbContext dbContext)
    {
        var osuId = dbContext.GetUserIdBySourceId(messageContext.MessageUserIdentity.UserId).Result;
        if (osuId == null)
        {
            return (false, null, Reply(UnbindMessage));
        }

        return (false, osuId, null);
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
        try
        {
            var result = authClientPub.GetPublicToken(_config.ClientId, _config.ClientSecret).Result;
            this.PublicToken = result;
            context.Logger.LogInformation($"已刷新Osu!PublicToken，将在{DateTime.Now.AddSeconds(result.ExpiresIn)}过期");
        }
        finally
        {
            _initialWait.TrySetResult();
        }
    }

    private static TokenBase ConvertToken(OsuToken osuToken)
    {
        if (osuToken.IsPublic)
            return new PublicToken
            {
                AccessToken = osuToken.AccessToken,
                CreateTime = osuToken.CreateTime,
                ExpiresIn = osuToken.ExpiresIn,
                TokenType = osuToken.TokenType
            };
        return new UserToken
        {
            AccessToken = osuToken.AccessToken,
            CreateTime = osuToken.CreateTime,
            ExpiresIn = osuToken.ExpiresIn,
            TokenType = osuToken.TokenType,
            RefreshToken = osuToken.RefreshToken
        };
    }
}