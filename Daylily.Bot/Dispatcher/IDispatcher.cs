using Daylily.Bot.Interface;

namespace Daylily.Bot.Dispatcher
{
    public interface IDispatcher : IMiddleware
    {
        bool Message_Received(object sender, MessageReceivedEventArgs args);
    }
}
