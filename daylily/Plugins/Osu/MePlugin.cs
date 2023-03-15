using System.ComponentModel;
using System.Text;
using Coosu.Api.V2;
using daylily.Plugins.Osu.BeatmapStats;
using daylily.Plugins.Osu.Data;
using daylily.Plugins.Osu.Me;
using Microsoft.Extensions.Logging;
using MilkiBotFramework;
using MilkiBotFramework.Connecting;
using MilkiBotFramework.Imaging;
using MilkiBotFramework.Imaging.Wpf;
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
    private readonly LightHttpClient _lightHttpClient;
    private readonly OsuDbContext _dbContext;

    public MePlugin(ILogger<MePlugin> logger,
        ApiService apiService,
        LightHttpClient lightHttpClient,
        OsuDbContext dbContext)
    {
        _logger = logger;
        _apiService = apiService;
        _lightHttpClient = lightHttpClient;
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

        var renderer = new WpfDrawingProcessor<MeOsuControlVm, MeControl>((vm, image) =>
            new MeControl(_lightHttpClient, vm, image), true);
        var vm = new MeOsuControlVm
        {
            User = user
        };
        var image = await renderer.ProcessAsync(vm);
        return Reply(new MemoryImage(image, ImageType.Png));
    }
}