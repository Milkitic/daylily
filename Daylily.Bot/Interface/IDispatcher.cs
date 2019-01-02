namespace Daylily.Bot.Interface
{
    public interface IDispatcher : IConfigurable
    {
        void Message_Received(object sender, MessageReceivedEventArgs args);
    }
}
