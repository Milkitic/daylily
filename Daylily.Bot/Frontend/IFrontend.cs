using Daylily.Bot.Message;

namespace Daylily.Bot.Frontend
{
    public interface IFrontend : IMiddleware
    {
        event MessageEventHandler MessageReceived;
        event ExceptionEventHandler ErrorOccured;

        bool RawObject_Received(object rawObject);
    }
}
