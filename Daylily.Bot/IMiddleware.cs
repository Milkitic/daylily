namespace Daylily.Bot
{
    public interface IMiddleware
    {
        event ExceptionEventHandler ErrorOccured;
        MiddlewareConfig MiddlewareConfig { get; }
    }
}
