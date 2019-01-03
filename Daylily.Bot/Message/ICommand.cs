using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface ICommand
    {
        string FullCommand { get; set; }
        string Command { get; set; }
        string ArgString { get; set; }
        Dictionary<string, string> Args { get; set; }
        List<string> FreeArgs { get; set; }
        Dictionary<string, string> Switches { get; set; }
        List<string> SimpleArgs { get; set; }
    }
}
