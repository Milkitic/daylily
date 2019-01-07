using Daylily.Bot.Message;

namespace Daylily.Bot.Dispatcher
{
    public interface IMessageDispatcher : IDispatcher
    {
        bool Message_Received(object sender, MessageEventArgs args);
        void SendMessage(RouteMessage message);
    }
}
