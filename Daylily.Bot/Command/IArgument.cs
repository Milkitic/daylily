using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Command
{
    public interface IArgument
    {
        string ArgString { get; }
        Dictionary<string, string> Args { get; }
        List<string> FreeArgs { get; }
        List<string> Switches { get; }
        List<string> SimpleArgs { get; }
    }
}
