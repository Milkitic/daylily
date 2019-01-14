using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public interface IParamDivider
    {
        bool TryDivide(string fullCmd, out ICommand command);
    }
}
