using System.ComponentModel;
using System.Text;
using Coosu.Api.V2;
using daylily.Plugins.Osu.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Osu;

[PluginIdentifier("1d3dce2d-f52d-42ab-97a4-fcf31e11dfbc", "个人名片", Scope = "osu!")]
[Description("个人名片查询")]
public class MePlugin : BasicPlugin
{
    private readonly ILogger<MePlugin> _logger;
    private readonly ApiService _apiService;
    private readonly OsuDbContext _dbContext;

    public MePlugin(ILogger<MePlugin> logger, ApiService apiService, OsuDbContext dbContext)
    {
        _logger = logger;
        _apiService = apiService;
        _dbContext = dbContext;
    }

    [CommandHandler("me.osu")]
    public async Task<IResponse> MeOsu(MessageContext messageContext)
    {
        var osuId = await _dbContext.GetUserIdBySourceId(messageContext.MessageUserIdentity.UserId);
        if (osuId == null) return Reply(_apiService.UnbindMessage);
        var result = await _apiService.TryAccessPublicApi(async client =>
            await client.User.GetUser(osuId.ToString(), GameMode.Osu));
        if (!result.Success)
        {
            return Reply(result.Error!);
        }

        var user = result.Result;

        var sb = new StringBuilder();
        sb.AppendLine(user.Username + "  " +
                      (string.IsNullOrWhiteSpace(user.Location) ? "" : (user.Location + ", ")) +
                      user.Country?.Name);
        if (!string.IsNullOrWhiteSpace(user.Discord)) sb.AppendLine("Discord: " + user.Discord);
        if (!string.IsNullOrWhiteSpace(user.Title)) sb.AppendLine("职位: " + user.Title);
        sb.AppendLine("好友关注: " + user.FollowerCount);
        sb.AppendLine("Rank图: " + user.RankedAndApprovedBeatmapsetCount);
        sb.AppendLine("Pending图: " + user.UnrankedBeatmapsetCount);
        sb.AppendLine("坟图: " + user.GraveyardBeatmapsetCount);
        if (user.Badges is { Length: > 0 })
        {
            sb.AppendLine("狗牌: ");
            var richMessage = new RichMessage(new Text(sb.ToString()));
            foreach (var badge in user.Badges)
            {
                _logger.LogDebug(badge.ImageUrl);
                richMessage.RichMessages.Add(new LinkImage(badge.ImageUrl));
            }

            return Reply(richMessage);
        }
        else
        {
            return Reply(sb.ToString().Trim('\n', '\r'));
        }

    }
}