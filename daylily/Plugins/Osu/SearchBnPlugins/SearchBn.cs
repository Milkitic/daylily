using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using daylily.Plugins.Osu.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;
using MilkiBotFramework.Tasking;

namespace daylily.Plugins.Osu.SearchBnPlugins;

[PluginIdentifier("70d8ff56-a6c0-4ae0-8304-94d867acd143", "BN信息搜索", Scope = "osu!")]
[PluginLifetime(PluginLifetime.Singleton)]
public class SearchBn : BasicPlugin
{
    private readonly ApiService _apiService;
    private readonly LightHttpClient _httpClient;
    private readonly OsuDbContext _osuDbContext;
    private readonly BotTaskScheduler _taskScheduler;

    public SearchBn(ApiService apiService, LightHttpClient httpClient, OsuDbContext osuDbContext, BotTaskScheduler taskScheduler)
    {
        _apiService = apiService;
        _httpClient = httpClient;
        _osuDbContext = osuDbContext;
        _taskScheduler = taskScheduler;
    }

    protected override async Task OnInitialized()
    {
        _taskScheduler.AddTask("BN列表更新", builder => builder
            .EachDayAt(DateTime.Parse("04:24:00"))
            .Do(UpdateList));
    }

    private void UpdateList(TaskContext context, CancellationToken token)
    {
        var logger = context.Logger;
        try
        {
            UpdateByMpgApi();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MappersGuild api出错，将无法更新BN列表");
        }

        var allTracked = _osuDbContext.OsuUserInfos.ToList();
        foreach (var userInfo in allTracked)
        {
            var osuId = userInfo.Id.ToString();
            var userResponse = _apiService.TryAccessPublicApi(async client => await client.User.GetUser(osuId)).Result;
            if (!userResponse.Success)
            {
                logger.LogWarning("获取osu用户时出错: " + userResponse.Error);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                continue;
            }

            var setResponse = _apiService.TryAccessPublicApi(async client => await client.User.GetUserBeatmap(osuId, UserBeatmapType.Favourite)).Result;
            if (!setResponse.Success)
            {
                logger.LogWarning("获取osu用户的收藏Beatmapset时出错: " + userResponse.Error);
                Thread.Sleep(TimeSpan.FromSeconds(1));
                continue;
            }

            var user = userResponse.Result!;
            var beatmapsets = setResponse.Result!;

            var favoriteSets = beatmapsets.Length > 0
                ? "\n" + string.Join("\n", beatmapsets.Select(k =>
                    $"<p>- Favorite map: \"{k.ArtistUnicode} - {k.TitleUnicode}\" by {k.Creator}</p>"))
                : null;

            var parser = new HtmlParser();
            var pageHtml = user.Page.Html;
            if (favoriteSets != null)
            {
                pageHtml += favoriteSets;
            }

            var document = parser.ParseDocument(pageHtml);
            using (var writer = new StringWriter())
            {
                document.ToHtml(writer, new PrettyMarkupFormatter
                {
                    Indentation = "    ",
                    NewLine = "\n"
                });
                var indentedText = writer.ToString();
                userInfo.UserPageText = indentedText;
            }

            try
            {
                _osuDbContext.SaveChanges();
                logger.LogDebug($"更新osu用户{userInfo.Id}({userInfo.Username})的信息");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"更新osu用户{userInfo.Id}信息时出错");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }

    private void UpdateByMpgApi()
    {
        var relevantInfo = _httpClient.HttpGet<RelevantInfo>("https://bn.mappersguild.com/relevantInfo").Result;

        var storedAll = _osuDbContext.OsuUserInfos
            .AsNoTracking()
            .ToDictionary(k => k.Id, k => k);
        var newAll = new Dictionary<int, OsuUserInfo>();
        foreach (var allUsersByMode in relevantInfo.AllUsersByMode)
        {
            foreach (var osuUserInfo in allUsersByMode.Users)
            {
                if (newAll.TryGetValue(osuUserInfo.Id, out var exist))
                {
                    exist.ModeIds.Add(allUsersByMode.ModeId);
                }
                else
                {
                    osuUserInfo.ModeIds.Add(allUsersByMode.ModeId);
                    newAll.Add(osuUserInfo.Id, osuUserInfo);
                }
            }
        }

        var existUsers = newAll.Where(k => storedAll.ContainsKey(k.Key)).Select(k => k.Value);
        var newUsers = newAll.Where(k => !storedAll.ContainsKey(k.Key)).Select(k => k.Value);
        var notExistUsers = storedAll.Where(k => !newAll.ContainsKey(k.Key)).Select(k => k.Value);

        _osuDbContext.OsuUserInfos.UpdateRange(existUsers);
        _osuDbContext.OsuUserInfos.AddRange(newUsers);
        _osuDbContext.OsuUserInfos.RemoveRange(notExistUsers);

        _osuDbContext.SaveChanges();
    }
}