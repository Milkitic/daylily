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
            SubCommand.List => throw new NotImplementedException(),
            SubCommand.Add => Reply(await AddSubscribe(context, beatmapsetId)),
            SubCommand.Del => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(subCommand), subCommand, null)
        };
    }

    private async Task<string> AddSubscribe(MessageContext context, int? beatmapsetId)
    {
        const int userLimitCount = 8;
        if (beatmapsetId == null)
            return "请指定谱面sid..";

        var userId = context.MessageUserIdentity!.UserId;
        if (context.Authority != MessageAuthority.Root)
        {
            var all = await _dbContext.BeatmapSubscribes
                .CountAsync(k => k.ScribeUserId == userId);
            if (all >= userLimitCount) return $"您的订阅数量已达最大值（{userLimitCount}），请使用\"/stats.list\"查看订阅列表。";
        }

        var setId = beatmapsetId.Value;
        var first = await _dbContext.BeatmapScans.FirstOrDefaultAsync(k => k.BeatmapSetId == setId);
        if (first == null)
        {
            _dbContext.BeatmapScans.Add(new BeatmapScan
            {
                BeatmapSetId = beatmapsetId.Value,
                BeatmapSubscribes = new List<BeatmapSubscribe> { new() { ScribeUserId = userId, } }
            });
        }
        else
        {
            first.BeatmapSubscribes.Add(new BeatmapSubscribe { ScribeUserId = userId });
        }

        await _dbContext.SaveChangesAsync();
        return "命令成功完成。";
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

            var result = response.Result!;
            var beatmapStat = new BeatmapStat
            {
                FavoriteCount = result.FavouriteCount,
                PlayCount = result.PlayCount,
                Timestamp = DateTime.Now
            };
            beatmapScan.BeatmapStats.Add(beatmapStat);

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