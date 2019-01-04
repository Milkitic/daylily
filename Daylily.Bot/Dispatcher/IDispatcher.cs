using Daylily.Bot.Message;

namespace Daylily.Bot.Dispatcher
{
    public interface IDispatcher : IMiddleware
    {
        bool Message_Received(object sender, MessageEventArgs args);
        void SendMessage(RouteMessage message);
    }
}
