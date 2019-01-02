using Daylily.Bot.Models;

namespace Daylily.Bot.Interface
{
    public interface IConfigurable
    {
        MiddlewareConfig MiddlewareConfig { get; set; }
    }
}
