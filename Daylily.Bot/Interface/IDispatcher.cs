namespace Daylily.Bot.Interface
{
    public interface IDispatcher : IMiddleware
    {
        bool Message_Received(object sender, MessageReceivedEventArgs args);
    }
}
