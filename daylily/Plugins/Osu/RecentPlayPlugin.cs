using System.ComponentModel;
using Coosu.Api.V2;
using daylily.Plugins.Osu;
using daylily.Plugins.Osu.Data;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace Daylily.Plugins.Osu;

[PluginIdentifier("611d6a61-1ccc-4c19-92f3-57976d927655", "成绩查询", Scope = "osu!")]
[Description("最近成绩查询")]
public class RecentPlayPlugin : BasicPlugin
{
    private readonly ILogger<MePlugin> _logger;
    private readonly ApiService _apiService;
    private readonly OsuDbContext _dbContext;

    public RecentPlayPlugin(ILogger<MePlugin> logger, ApiService apiService, OsuDbContext dbContext)
    {
        _logger = logger;
        _apiService = apiService;
        _dbContext = dbContext;
    }

    [CommandHandler("r")]
    public async Task<IResponse> Recent(MessageContext context,
        [Argument, Description("游戏模式. 0:Osu; 1:Taiko; 2:Fruits; 3:Mania")] GameMode? gameMode = null,
        [Option("all"), Description("包含Fail成绩")] bool all = false,
        [Option("qq", Authority = MessageAuthority.Root), Description("指定的qq号")] int? qq = null,
        [Option("id", Authority = MessageAuthority.Root), Description("指定的osu id号")] string? osuId = null,
        [Option("user", Authority = MessageAuthority.Root), Description("指定的用户名")] string? userName = null)
    {
        if (osuId == null)
        {
            if (qq != null)
            {
                var id = await _dbContext.GetUserIdByQQ(qq.Value);
                if (id == null) return Reply($"QQ {qq} 未绑定osu!账号。");
                osuId = id.Value.ToString();
            }
            else if (userName != null)
            {
                var response1 = await _apiService.TryAccessPublicApi(async client => await client.User.GetUser(userName));
                if (!response1.Success)
                {
                    return Reply(response1.Error!);
                }

                osuId = response1.Result!.Id?.ToString();
            }
            else
            {
                var id = await _dbContext.GetUserIdByQQ(Convert.ToInt64(context.MessageUserIdentity.UserId));
                if (id == null) return Reply(_apiService.UnbindMessage);
                osuId = id.Value.ToString();
            }
        }

        var response = await _apiService.TryAccessPublicApi(async client => await client.User.GetUserScores(osuId,
            ScoreType.Recent, all, gameMode, new Pagination
            {
                Offset = 1,
                Limit = 1
            }));
        if (!response.Success)
        {
            return Reply(response.Error!);
        }

        if (response.Result!.Length == 0) return Reply("没有成绩");
        var score = response.Result[0];
        var beatmapset = score.Beatmapset;
        var user = score.User;
        var reply = $"{user.Username} 在 \"{beatmapset.ArtistUnicode} - {beatmapset.TitleUnicode}\" 中以 {score.Accuracy:P2}、{score.Rank} Rank";
        if (score.Mods.Length > 0)
        {
            reply += "、+" + string.Join("", score.Mods);
        }

        if (score.Perfect)
            reply += $" 的成绩 FC 了. 🥳\r\n{user.Username}!.jpg";
        else if (score.Statistics.CountMiss == 0)
            reply += $" 的成绩 断滑条 了. 😏\r\n{user.Username}!.jpg";
        else if (score.Passed)
            reply += " 的成绩 PASS 了. 👏👏\r\n";
        else
            reply += " 的成绩 FAIL 了. 🤷‍♂️\r\n";
        reply += score.Beatmap.Url;
        return Reply(reply);
    }
}