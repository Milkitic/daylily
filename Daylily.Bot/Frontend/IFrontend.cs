using Daylily.Bot.Dispatcher;
using Daylily.Bot.Message;

namespace Daylily.Bot.Frontend
{
    public interface IFrontend : IMiddleware
    {
        event MessageEventHandler MessageReceived;
    
        bool RawObject_Received(object rawObject);
    }
}
