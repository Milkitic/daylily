using System.Collections.Concurrent;
using System.ComponentModel;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;
using MilkiBotFramework.Plugining.Loading;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Core;

[PluginIdentifier("32d3ca82-969e-4757-8575-7ab11371766a", "关机", AllowDisable = false, Index = -30)]
[Description("可将${BotNick}关闭一段时间，支持自定义时间")]
[PluginLifetime(PluginLifetime.Singleton)]
public class PowerOff : BasicPlugin
{
    private readonly IMessageApi _messageApi;
    private readonly PluginManager _pluginManager;
    private readonly BotTaskScheduler _taskScheduler;
    private readonly ShuntDownConfig _config;

    private Dictionary<Guid, PluginInfo> _plugins;

    public PowerOff(IConfiguration<ShuntDownConfig> configuration,
        IMessageApi messageApi,
        PluginManager pluginManager,
        BotTaskScheduler taskScheduler)
    {
        _config = configuration.Instance;
        _messageApi = messageApi;
        _pluginManager = pluginManager;
        _taskScheduler = taskScheduler;
    }

    protected override async Task OnInitialized()
    {
        _config.ExpireTimeDictionary ??= new ConcurrentDictionary<MessageIdentity, DateTimeOffset?>();
        _plugins = _pluginManager
            .GetAllPlugins()
            .Where(k => k.PluginType == PluginType.Basic && k.AllowDisable
                        || k.Type == typeof(PluginFilter)
                        )
            .ToDictionary(k => k.Metadata.Guid, k => k);
        await _config.SaveAsync();
        _taskScheduler.AddTask("PowerOffScan", k => k
            .ByInterval(TimeSpan.FromSeconds(10))
            .AtStartup()
            .WithoutLogging()
            .Do(PowerOffScan)
        );
    }

    public override async IAsyncEnumerable<IResponse> OnMessageReceived(MessageContext context)
    {
        var messageIdentity = context.MessageIdentity!;
        using (await _config.AsyncLock.LockAsync())
        {
            if (_config.ExpireTimeDictionary!.TryGetValue(messageIdentity, out var expireTime) &&
                expireTime > DateTimeOffset.Now)
            {
                foreach (var pluginInfo in _plugins.Values)
                {
                    context.NextPlugins.Remove(pluginInfo);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    [CommandHandler("poweroff"
#if !DEBUG
        , AllowedMessageType = MessageType.Channel
#endif
    )]
    public async Task<IResponse?> OnPowerOff(MessageContext context,
        [Argument, Description("关闭的持续时间，最大支持7天，最小支持1分钟（单位：分钟）")]
        int elapsingTime = 10)
    {
        using (await _config.AsyncLock.LockAsync())
        {
            var messageIdentity = context.MessageIdentity!;
            if (_config.ExpireTimeDictionary!.TryGetValue(messageIdentity, out var expireTime) &&
                expireTime > DateTimeOffset.Now)
            {
                return null;
            }

            if (context.Authority < MessageAuthority.Admin)
            {
                return Reply("此功能需要群内管理员权限..").AvoidRepeat();
            }

            var time = TimeSpan.FromMinutes(elapsingTime);
            if (time < TimeSpan.FromMinutes(1) || time > TimeSpan.FromDays(7))
            {
                return Reply("时间范围设置不对，最大支持7天，最小支持1分钟..").AvoidRepeat();
            }

            var newTime = DateTimeOffset.Now + time;
            _config.ExpireTimeDictionary.AddOrUpdate(messageIdentity, newTime, (_, _) => newTime);
            await _config.SaveAsync();
            return Reply($"我自闭去了。。等我{newTime:g}回来");
        }
    }

    [CommandHandler("poweron"
#if !DEBUG
        , AllowedMessageType = MessageType.Channel
#endif
    )]
    public async Task<IResponse?> OnPowerOn(MessageContext messageContext)
    {
        using (await _config.AsyncLock.LockAsync())
        {
            var messageIdentity = messageContext.MessageIdentity!;

            if (_config.ExpireTimeDictionary!.TryGetValue(messageIdentity, out var expireTime) &&
                expireTime > DateTimeOffset.Now)
            {
                if (messageContext.Authority < MessageAuthority.Admin)
                {
                    return Reply("若想开机，请联系管理员使用此命令..").AvoidRepeat();
                }

                _config.ExpireTimeDictionary.TryRemove(messageIdentity, out _);
                await _config.SaveAsync();
                return Reply("啊，活过来了").AvoidRepeat();
            }

            return Reply("我还活着呢，你想干什么压");
        }
    }

    private void PowerOffScan(TaskContext context, CancellationToken token)
    {
        using (_config.AsyncLock.Lock())
        {
            var now = DateTime.Now;
            bool modified = false;
            var expireTimeDictionary = _config.ExpireTimeDictionary;
            if (expireTimeDictionary == null) return;

            foreach (var (messageIdentity, expireTime) in expireTimeDictionary.ToArray())
            {
                if (now < expireTime) continue;
                modified = true;
                expireTimeDictionary.TryRemove(messageIdentity, out _);
                var channelId = messageIdentity.Id;
                var subChannelId = messageIdentity.SubId;

                if (messageIdentity.MessageType == MessageType.Private)
                {
                    _messageApi.SendPrivateMessageAsync(channelId, "啊，活过来了").Wait(token);
                }
                else if (messageIdentity.MessageType == MessageType.Channel)
                {
                    _messageApi.SendChannelMessageAsync(channelId, "啊，活过来了", subChannelId).Wait(token);
                }
            }

            if (modified)
            {
                _config.SaveAsync().Wait(token);
            }
        }
    }
}