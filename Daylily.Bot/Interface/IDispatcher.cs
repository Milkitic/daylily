namespace Daylily.Bot.Interface
{
    public interface IDispatcher : IConfigurable
    {
        bool Message_Received(object sender, MessageReceivedEventArgs args);
    }
}
