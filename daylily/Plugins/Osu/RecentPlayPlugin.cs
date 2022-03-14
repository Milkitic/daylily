using System.ComponentModel;
using Coosu.Api.V2;
using daylily.Data;
using daylily.Plugins.Osu;
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
            else
            {
                var id = await _dbContext.GetUserIdByQQ(Convert.ToInt64(context.MessageUserIdentity.UserId));
                if (id == null) return Reply(_apiService.UnbindMessage);
                osuId = id.Value.ToString();
            }
        }

        var result = await _apiService.TryAccessPublicApi(async client =>
        {
            return await client.User.GetUser(osuId, GameMode.Osu);
        });
        if (!result.Success)
        {
            return Reply(result.Error!);
        }

        return Reply(result.Result!.Username + "!.jpg");
    }
}