using Daylily.Bot.Backend;
using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public interface ICommand : IArgument
    {
        string FullCommand { get; }
        string CommandName { get; }
    }

    public interface IWritableCommand : ICommand
    {
        new string FullCommand { get; set; }
        new string CommandName { get; set; }
        new string ArgString { get; set; }
        new Dictionary<string, string> Args { get; set; }
        new List<string> FreeArgs { get; set; }
        new List<string> Switches { get; set; }
        new List<string> SimpleArgs { get; set; }
    }
}
