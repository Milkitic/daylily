using Daylily.Bot.Backend;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Command
{
    public class Command : ICommand
    {
        internal Command(string commandName, Dictionary<string, string> args, List<string> freeArgs,
            List<string> switches, string fullCommand, string argString, List<string> simpleArgs)
        {
            ArgString = argString;
            Args = args;
            FreeArgs = freeArgs;
            Switches = switches;
            SimpleArgs = simpleArgs;
            FullCommand = fullCommand;
            CommandName = commandName;
        }

        public string ArgString { get; }
        public Dictionary<string, string> Args { get; }
        public List<string> FreeArgs { get; }
        public List<string> Switches { get; }
        public List<string> SimpleArgs { get; }
        public ParameterCollection Parameters { get; set; }
        public string FullCommand { get; set; }
        public string CommandName { get; }
    }
}
