using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public interface IParamDivider
    {
        string CommandName { get; }
        string ArgString { get; }
        Dictionary<string, string> Args { get; }
        List<string> FreeArgs { get; }
        Dictionary<string, string> Switches { get; }
        List<string> SimpleArgs { get; set; }
        bool TryDivide(string fullCmd);
    }
}
