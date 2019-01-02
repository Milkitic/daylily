using Daylily.Bot.Models;

namespace Daylily.Bot.Interface
{
    public interface IMiddleware
    {
        MiddlewareConfig MiddlewareConfig { get; }
    }
}
