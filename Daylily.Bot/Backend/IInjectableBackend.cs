namespace Daylily.Bot.Backend
{
    public interface IInjectableBackend : ICommandBackend
    {
        ParameterCollection Parameters { get; }
        void OnCommandBindingFailed(BindingFailedEventArgs args);
    }
}
