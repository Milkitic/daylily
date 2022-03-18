using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Plugins.Basic;

[PluginIdentifier("3512e3d9-2d29-4f0c-bc84-d68cfd3bdc6f", "今天吃什么", Scope = "mjbot",
    Authors = "milkiticyf, mjyn")]
public class WhatToEat : BasicPlugin
{
    private readonly ILogger<WhatToEat> _logger;
    private readonly BotOptions _botOptions;
    private readonly WhatToEatConfig _config;

    public WhatToEat(IConfiguration<WhatToEatConfig> configuration, ILogger<WhatToEat> logger, BotOptions botOptions)
    {
        _logger = logger;
        _botOptions = botOptions;
        _config = configuration.Instance;
    }

    protected override async Task OnInitialized()
    {
        if (_config.Sessions == null)
        {
            _config.Sessions = new ConcurrentDictionary<MessageIdentity, WhatToEatConfig.GroupMeals?>();
            await _config.SaveAsync();
        }
    }

    [CommandHandler("what2eat")]
    public async Task<IResponse> What2EatCore(MessageContext context)
    {
        var (groupMeals, response) = await AutoGetGroupMeal(context);
        if (response != null) return response;

        var @lock = groupMeals!.CollectionLock;
        TimeSpan elapsed = DateTime.Now - groupMeals.Cooldown;
        if (elapsed.TotalSeconds < 5)
            return Reply("冷却中。");

        using var _ = WannaRead(@lock);
        if (groupMeals.Meals.Count == 0)
            return Reply("条目列表为空。");

        groupMeals.Cooldown = DateTime.Now;

        var randomIndex = Random.Shared.Next(0, groupMeals.Meals.Count);
        return Reply($"吃 {groupMeals.Meals[randomIndex]} 怎么样？");
    }

    [CommandHandler("what2eat10")]
    public async Task<IResponse> What2Eat10Core(MessageContext context)
    {
        var (groupMeals, response) = await AutoGetGroupMeal(context);
        if (response != null) return response;

        var @lock = groupMeals!.CollectionLock;
        TimeSpan elapsed = DateTime.Now - groupMeals.Cooldown;
        if (elapsed.TotalSeconds < 30)
            return Reply("冷却中。");

        var sb = new StringBuilder();
        using var _ = WannaRead(@lock);
        if (groupMeals.Meals.Count == 0)
            return Reply("条目列表为空。");

        groupMeals.Cooldown = DateTime.Now;
        for (int i = 0; i < Math.Min(11, groupMeals.Meals.Count); i++)
        {
            int randomIndex = Random.Shared.Next(0, groupMeals.Meals.Count);
            sb.AppendLine($"第 {i + 1} 道菜：{groupMeals.Meals[randomIndex]}");
        }

        var result = sb.ToString().TrimEnd('\n', '\r');
        return Reply(result);
    }

    [CommandHandler("addmeal")]
    public async Task<IResponse> AddMeal(MessageContext context,
        [Description("菜肴"), Argument] string? mealName = null)
    {
        var messageIdentity = context.MessageIdentity!;

        var excludeMeals = FixConfiguration(messageIdentity);
        var groupMeals = GetGroupMeal(messageIdentity, excludeMeals);
        var mealList = groupMeals.Meals;

        var @lock = groupMeals.CollectionLock;
        using (WannaRead(@lock))
        {
            if (string.IsNullOrWhiteSpace(mealName))
                return Reply("参数为空。");
            if (mealName.Length > 70)
                return Reply("条目过长。");
            if (mealList.Contains(mealName))
                return Reply("该条目已存在。");
        }

        using (WannaWrite(@lock))
        {
            mealList.Add(mealName);
            await _config.SaveAsync();
        }

        int count;
        using (WannaRead(@lock)) count = mealList.Count;

        _logger.LogInformation($"目前共有 {count} 个条目。");
        return Reply("命令成功完成。");
    }

    [CommandHandler("removemeal")]
    public async Task<IResponse> RemoveMeal(MessageContext context,
        [Description("菜肴"), Argument] string? mealName = null)
    {
        var messageIdentity = context.MessageIdentity!;

        var excludeMeals = FixConfiguration(messageIdentity);
        var groupMeals = GetGroupMeal(messageIdentity, excludeMeals);
        var mealList = groupMeals.Meals;

        var @lock = groupMeals.CollectionLock;
        using (WannaRead(@lock))
        {
            if (string.IsNullOrWhiteSpace(mealName))
                return Reply("参数为空。");
            if (!mealList.Contains(mealName))
                return Reply("该条目不存在。");
        }

        using (WannaWrite(@lock))
        {
            mealList.Remove(mealName);
            await _config.SaveAsync();
        }

        int count;
        using (WannaRead(@lock)) count = mealList.Count;

        _logger.LogInformation($"目前共有 {count} 个条目。");
        return Reply("命令成功完成。");
    }

    [CommandHandler("wtemode")]
    public async Task<IResponse> WteMode(MessageContext context)
    {
        var messageIdentity = context.MessageIdentity!;
        if (messageIdentity.SubId == null)
            return Reply("不是频道会话，无需切换模式");

        var excludeMeals = FixConfiguration(messageIdentity);
        var groupMeals = GetGroupMeal(messageIdentity, excludeMeals);
        groupMeals.IsSubChannelOnly = !groupMeals.IsSubChannelOnly;
        var str = groupMeals.IsSubChannelOnly ? "子频道独享" : "频道共享";
        await _config.SaveAsync();
        return Reply("已切换至模式：" + str);
    }

    private async Task<(WhatToEatConfig.GroupMeals? groupMeals, IResponse? response)> AutoGetGroupMeal(MessageContext context)
    {
        var userId = context.MessageUserIdentity!.UserId;
        var messageIdentity = context.MessageIdentity!;

        var excludeMeals = FixConfiguration(messageIdentity);
        var response = await RateLimit(context, messageIdentity, excludeMeals, userId);
        if (response != null)
        {
            return (null, response);
        }

        var groupMeals = GetGroupMeal(messageIdentity, excludeMeals);
        return (groupMeals, null);
    }

    private WhatToEatConfig.GroupMeals GetGroupMeal(MessageIdentity messageIdentity,
        WhatToEatConfig.GroupMeals excludeMeals)
    {
        if (messageIdentity.SubId == null)
            return excludeMeals;
        if (excludeMeals.IsSubChannelOnly)
            return excludeMeals;

        var excludes = _config.Sessions
            .Where(k => k.Value.IsSubChannelOnly)
            .Select(k => k.Key)
            .ToHashSet();

        var allDbs = _config.Sessions
            .Where(k => !excludes.Contains(k.Key) &&
                        k.Key.Id == messageIdentity.Id &&
                        k.Key.MessageType == messageIdentity.MessageType)
            .ToList();

        var sharedMeals = new WhatToEatConfig.GroupMeals
        {
            Cooldown = excludeMeals.Cooldown,
            Meals = allDbs.SelectMany(k => k.Value.Meals).Distinct().ToList()
        };

        return sharedMeals;
    }

    private async Task<IResponse?> RateLimit(MessageContext context,
        MessageIdentity messageIdentity,
        WhatToEatConfig.GroupMeals excludeMeals,
        string userId)
    {
        if (messageIdentity.MessageType == MessageType.Private || context.Authority >= MessageAuthority.Admin)
            return null;

        // rate limit
        var limit = 5;
        var list = excludeMeals.UserCoolDownList[userId]
            .Where(k => k >= DateTime.Now.AddMinutes(-10) && k < DateTime.Now)
            .ToList();
        if (list.Count > limit)
        {
            excludeMeals.UserCoolDownList[userId] = list;
            await _config.SaveAsync();
            return Reply("您说话太快了，请休息一下再发言。");
        }

        excludeMeals.UserCoolDownList[userId].Add(DateTime.Now);
        await _config.SaveAsync();
        return null;
    }

    private WhatToEatConfig.GroupMeals FixConfiguration(MessageIdentity messageIdentity)
    {
        var excludeMeals = _config.Sessions.GetOrAdd(messageIdentity, _ => new WhatToEatConfig.GroupMeals())!;
        excludeMeals.UserCoolDownList ??= new Dictionary<string, List<DateTime>>();
        return excludeMeals;
    }

    private static AutoExitRwLock WannaRead(ReaderWriterLockSlim @lock)
    {
        return new AutoExitRwLock(@lock, true);
    }

    private static AutoExitRwLock WannaWrite(ReaderWriterLockSlim @lock)
    {
        return new AutoExitRwLock(@lock, false);
    }

    private class AutoExitRwLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly bool _readOrWrite;

        public AutoExitRwLock(ReaderWriterLockSlim @lock, bool readOrWrite)
        {
            _lock = @lock;
            _readOrWrite = readOrWrite;
            if (readOrWrite)
                @lock.EnterReadLock();
            else
                @lock.EnterWriteLock();
        }

        public void Dispose()
        {
            if (_readOrWrite)
                _lock.ExitReadLock();
            else
                _lock.ExitWriteLock();
        }
    }
}