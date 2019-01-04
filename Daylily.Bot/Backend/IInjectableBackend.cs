namespace Daylily.Bot.Backend
{
    public interface IInjectableBackend : IBackend
    {
        void OnCommandBindingFailed(BindingFailedEventArgs args);
    }
}
