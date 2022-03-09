using System.ComponentModel;
using daylily.Moebooru;
using MilkiBotFramework.Messaging;
using MilkiBotFramework.Messaging.RichMessages;
using MilkiBotFramework.Plugining;
using MilkiBotFramework.Plugining.Attributes;

namespace daylily.Plugins;

[PluginIdentifier("a6cbd411-499f-4dde-bdb0-fc17431cb6c9", "konachan", Authors = "bleatingsheep")]
[Description("设了")]
public class Konachan : BasicPlugin
{
    [CommandHandler("konachan")]
    public async Task<IResponse?> OnKonachan()
    {
        return await GetResponse("https://konachan.net");
    }

    [CommandHandler("yandere")]
    public async Task<IResponse?> OnYandere()
    {
        return await GetResponse("https://yande.re");
    }

    private static async Task<IResponse?> GetResponse(string domain)
    {
        var k = new Api(domain);
        var result = await k.PopularRecentAsync();
        var post = result?.FirstOrDefault();
        return post == null ? null : Reply(new LinkImage(post.JpegUrl));
    }
}