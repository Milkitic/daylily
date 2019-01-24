using Daylily.Bot.Messaging;

namespace Daylily.Bot.Dispatcher
{
    public interface IMessageDispatcher : IDispatcher
    {
        bool Message_Received(object sender, MessageEventArgs args);
        void SendMessage(RouteMessage message);
    }
}
