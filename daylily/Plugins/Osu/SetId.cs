using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Api.V2;
using daylily.Plugins.Osu.Data;
using daylily.Utils;
using MilkiBotFramework.Event;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Plugins.Osu;

[PluginIdentifier("f159fb01-de65-46e0-818c-b38fdc55eea3", "绑定osu!账号")]
public class SetId : BasicPlugin
{
    private readonly ApiService _apiService;
    private readonly EventBus _eventBus;
    private readonly OsuDbContext _dbContext;
    private readonly OsuConfig _config;

    public SetId(IConfiguration<OsuConfig> configuration, ApiService apiService, EventBus eventBus,
        OsuDbContext dbContext)
    {
        _config = configuration.Instance;
        _apiService = apiService;
        _eventBus = eventBus;
        _dbContext = dbContext;
    }

    [Description("将osu!账号绑定至QQ")]
    [CommandHandler("setid.osu", AllowedMessageType = MessageType.Private)]
    public async IAsyncEnumerable<IResponse> SetIdCore(MessageContext context)
    {
        var userId = context.MessageUserIdentity!.UserId;
        var sb = new AuthorizationLinkBuilder(_config.ClientId,
            new Uri(_config.ServerRedirectUri));

        var guid = Guid.NewGuid();
        var sessionToken = guid.ToString("N");
        var state = EncryptUtil.EncryptAes256UseMd5(userId + "|" + DateTime.Now.Ticks + "|" + sessionToken,
            _config.QQAesKey,
            _config.QQAesIV);

        var uri = sb.BuildAuthorizationLink(state, AuthorizationScope.Identify);
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var tcs = new TaskCompletionSource();
        cts.Token.Register(() => tcs.SetCanceled());
        OsuTokenReceivedEvent @event = null!;
        _eventBus.Subscribe<OsuTokenReceivedEvent>(e =>
        {
            if (e.SessionToken != sessionToken) return;
            @event = e;
            tcs.TrySetResult();
        }, guid);

        yield return Reply("请点击以下链接完成账号授权，5分钟内有效（请勿分享链接）：");
        await Task.Delay(300);
        yield return Reply(new UriText(uri.AbsoluteUri));
        bool timeout;
        try
        {
            await tcs.Task;
            timeout = false;
        }
        catch
        {
            timeout = true;
        }

        _eventBus.Unsubscribe<OsuTokenReceivedEvent>(guid);
        if (timeout)
        {
            //yield return Reply("超过5分钟未点击链接，操作已取消..");
            yield break;
        }

        if (!@event.IsSuccess)
        {
            yield return Reply(@event.FailReason);
            yield break;
        }

        await _dbContext.AddOrUpdateToken(@event.SourceId!, @event.User.Id.Value, @event.Token!);
        yield return Reply($"你已成功绑定osu!id: {@event.User.Username}");
    }
}