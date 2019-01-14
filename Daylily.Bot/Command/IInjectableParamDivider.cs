using Daylily.Bot.Backend;

namespace Daylily.Bot.Command
{
    public interface IInjectableParamDivider : IParamDivider
    {
        bool TryDivide(string fullCmd, IInjectableBackend backend, out ICommand command);
    }
}