using Daylily.Bot.Attributes;

namespace Daylily.Plugin.ShaDiao.Command
{
    [Command("yandere")]
    public class Yandere : Konachan
    {
        protected override string Website => "https://yande.re";
    }
}
