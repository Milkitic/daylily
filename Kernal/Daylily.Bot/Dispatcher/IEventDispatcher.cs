namespace Daylily.Bot.Dispatcher
{
    public interface IEventDispatcher : IDispatcher
    {
        bool Event_Received(object sender, EventEventArgs args);

    }
}
