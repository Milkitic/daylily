using Daylily.Bot.Dispatcher;

namespace Daylily.Bot.Frontend
{
    public interface IFrontend : IMiddleware
    {
        event MessageEventHandler MessageReceived;
    
        bool RawObject_Received(object rawObject);
    }
}
