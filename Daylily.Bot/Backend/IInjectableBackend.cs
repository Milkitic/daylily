namespace Daylily.Bot.Backend
{
    public interface IInjectableBackend : ICommandBackend
    {
        void OnCommandBindingFailed(BindingFailedEventArgs args);
    }
}
