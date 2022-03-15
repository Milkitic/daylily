using System.ComponentModel;
using System.Text.Json;
using daylily.Plugins.Osu.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Osu;

[PluginIdentifier("c95a929f-8b4c-42e2-bdec-80498b2c55d5", "地图数据订阅")]
[PluginLifetime(PluginLifetime.Singleton)]
public class BeatmapStatistics : BasicPlugin
{
    public enum SubCommand
    {
        Map, List, Add, Del
    }

    private readonly ApiService _apiService;
    private readonly BotTaskScheduler _taskScheduler;
    private readonly OsuDbContext _dbContext;

    public BeatmapStatistics(ApiService apiService,
        BotTaskScheduler taskScheduler,
        OsuDbContext dbContext)
    {
        _apiService = apiService;
        _taskScheduler = taskScheduler;
        _dbContext = dbContext;
    }

    [CommandHandler("stats")]
    public async Task<IResponse> Stats(MessageContext context,
        [Argument] SubCommand subCommand,
        [Argument, Description("谱面sid")] int? beatmapsetId = null,
        [Option("time"), Description("指定最近的N小时内数据")]
        int? hours = null)
    {
        return subCommand switch
        {
            SubCommand.Map => Reply(await GetMapGraph(context, beatmapsetId, hours)),
            SubCommand.List => Reply(await ListSubscribe(context)),
            SubCommand.Add => Reply(await AddSubscribe(context, beatmapsetId)),
            SubCommand.Del => Reply(await DelSubscribe(context, beatmapsetId)),
            _ => throw new ArgumentOutOfRangeException(nameof(subCommand), subCommand, null)
        };
    }

    private async Task<string> ListSubscribe(MessageContext context)
    {
        var userId = context.MessageUserIdentity!.UserId;
        var all = await _dbContext.BeatmapSubscribes
            .Include(k => k.BeatmapScan)
            .Where(k => k.ScribeUserId == userId)
            .ToListAsync();
        if (all.Count == 0)
            return "你未订阅任何谱面..";

        var stringInfos = all.Select(k =>
            $"{k.BeatmapScan.BeatmapSetId}: {k.BeatmapScan.Artist} - {k.BeatmapScan.Title} by {k.BeatmapScan.Mapper}");
        return "订阅列表：\r\n" + string.Join("\r\n", stringInfos) +
               "\r\n使用\"/stats.del {sid}\" 移除订阅。";
    }

    private async Task<string> AddSubscribe(MessageContext context, int? setId)
    {
        const int userLimitCount = 8;
        if (setId == null)
            return "请指定谱面sid..";

        var userId = context.MessageUserIdentity!.UserId;
        if (context.Authority != MessageAuthority.Root)
        {
            var all = await _dbContext.BeatmapSubscribes
                .CountAsync(k => k.ScribeUserId == userId);
            if (all >= userLimitCount) return $"你的订阅数量已达最大值（{userLimitCount}），请使用\"/stats.list\"查看订阅列表。";
        }

        var beatmapSetId = setId.Value;
        var scan = await _dbContext.BeatmapScans
            .Include(k => k.BeatmapSubscribes)
            .FirstOrDefaultAsync(k => k.BeatmapSetId == beatmapSetId);

        var response = await _apiService.TryAccessPublicApi(async client => await client.Beatmap.GetBeatmapset(setId.Value));

        if (!response.Success)
        {
            return response.Error!;
        }

        var setInfo = response.Result!;

        if (scan == null)
        {
            _dbContext.BeatmapScans.Add(scan = new BeatmapScan
            {
                BeatmapSetId = setId.Value,
                Artist = setInfo.ArtistUnicode ?? setInfo.Artist,
                Title = setInfo.TitleUnicode ?? setInfo.Title,
                Mapper = setInfo.Creator,
                BeatmapSubscribes = new List<BeatmapSubscribe> { new() { ScribeUserId = userId, } }
            });
        }
        else
        {
            if (scan.BeatmapSubscribes.Any(k => k.ScribeUserId == userId))
                return "你已过订阅该谱面..";
            scan.BeatmapSubscribes.Add(new BeatmapSubscribe { ScribeUserId = userId });
        }

        await _dbContext.SaveChangesAsync();
        var description = $"{scan.BeatmapSetId}: {scan.Artist} - {scan.Title} by {scan.Mapper}";
        return "已成功订阅：" + description;
    }

    private async Task<string> DelSubscribe(MessageContext context, int? setId)
    {
        if (setId == null)
            return "请指定谱面sid..";

        var userId = context.MessageUserIdentity!.UserId;
        var subscribe = await _dbContext.BeatmapSubscribes
            .FirstOrDefaultAsync(k => k.ScribeUserId == userId && k.BeatmapScan.BeatmapSetId == setId);

        if (subscribe == null)
        {
            return "你尚未订阅该谱面..";
        }

        var scan = subscribe.BeatmapScan;
        var description = $"{scan.BeatmapSetId}: {scan.Artist} - {scan.Title} by {scan.Mapper}";
        _dbContext.BeatmapSubscribes.Remove(subscribe);
        await _dbContext.SaveChangesAsync();
        return "已成功取消订阅：" + description;
    }

    private async Task<IRichMessage> GetMapGraph(MessageContext context, int? beatmapsetId, int? hours)
    {
        if (beatmapsetId == null)
            return new Text("请指定谱面sid.");
        throw new NotImplementedException();
    }

    protected override async Task OnInitialized()
    {
        _taskScheduler.AddTask("更新地图订阅数据", builder => builder
            .AtStartup()
            .ByInterval(TimeSpan.FromMinutes(3))
            .Do(UpdateSubscriptionTask)
        );
    }

    private void UpdateSubscriptionTask(TaskContext context, CancellationToken token)
    {
        var existScans = _dbContext
            .BeatmapScans
            .Include(k => k.BeatmapSubscribes)
            .Include(k => k.BeatmapStats)
            .AsEnumerable()
            .Where(k => k.BeatmapSubscribes.Any());

        foreach (var beatmapScan in existScans)
        {
            var response = _apiService.TryAccessPublicApi(async client =>
            {
                return await client.Beatmap.GetBeatmapset(beatmapScan.BeatmapSetId);
            }).Result;

            if (!response.Success)
            {
                context.Logger.LogWarning("Error while scanning beatmap stats: " + response.Error);
                continue;
            }

            var setInfo = response.Result!;
            var beatmapStat = new BeatmapStat
            {
                FavoriteCount = setInfo.FavouriteCount,
                PlayCount = setInfo.PlayCount,
                Timestamp = DateTime.Now
            };
            beatmapScan.BeatmapStats.Add(beatmapStat);

            beatmapScan.Artist = setInfo.ArtistUnicode ?? setInfo.Artist;
            beatmapScan.Title = setInfo.TitleUnicode ?? setInfo.Title;
            beatmapScan.Mapper = setInfo.Creator;

            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                context.Logger.LogWarning("Error while saving beatmap stats: " + ex.Message);
            }

            context.Logger.LogDebug("Saved beatmapsets stats. " +
                                    "beatmapsets: " + beatmapScan.BeatmapSetId +
                                    "; data: " + JsonSerializer.Serialize(beatmapStat));
        }
    }
}