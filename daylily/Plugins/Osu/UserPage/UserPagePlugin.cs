using System.ComponentModel;
using Coosu.Api.V2;
using daylily.Plugins.Osu.Data;
using MilkiBotFramework.Imaging;
using MilkiBotFramework.Imaging.Wpf;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins.Osu.UserPage;

[PluginIdentifier("20e4039e-41fd-4dee-bcaa-12b43dc7e0a6", "个人介绍页面", Scope = "osu!")]
[Description("自动生成个人介绍页面截图")]
public class UserPagePlugin : BasicPlugin
{
    private readonly ApiService _apiService;
    private readonly OsuDbContext _dbContext;

    public UserPagePlugin(ApiService apiService, OsuDbContext dbContext)
    {
        _apiService = apiService;
        _dbContext = dbContext;
    }

    [CommandHandler("userpage")]
    public async Task<IResponse> UserPageHandler(MessageContext context,
        [Argument, Description("用户名或用户UID")] string? user = null)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            var osuId = await _dbContext.GetUserIdBySourceId(context.MessageUserIdentity!.UserId);
            if (osuId == null) return Reply(_apiService.UnbindMessage);
            user = osuId.ToString();
        }

        var response =
            await _apiService.TryAccessPublicApi(async client => await client.User.GetUser(user!, GameMode.Osu));
        if (!response.Success)
            return Reply(response.Error!);

        var result = response.Result!;
        if (string.IsNullOrWhiteSpace(result.Page?.Raw))
            return Reply("该用户没有UserPage");

        var renderer = new WpfDrawingProcessor<UserPageVm, UserPageControl>((vm, image1) =>
            new UserPageControl(PluginHome, vm, image1));

        var helpViewModel = new UserPageVm(result.Id!.Value, result.Page.Html);
        var image = await renderer.ProcessAsync(helpViewModel);
        return Reply(new MemoryImage(image, ImageType.Png));
    }
}