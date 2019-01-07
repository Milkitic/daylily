using System.Collections.Generic;

namespace Daylily.Bot.Command
{
    public class CommandAnalyzer
    {
        public string CommandName { get; private set; }
        public string ArgString { get; private set; }

        public Dictionary<string, string> Args { get; private set; }
        public List<string> FreeArgs { get; private set; }
        public Dictionary<string, string> Switches { get; private set; }
        public List<string> SimpleParams { get; set; } = new List<string>();

        private readonly IParamDivider _divider;

        public CommandAnalyzer(IParamDivider divider)
        {
            this._divider = divider;
        }

        public void Analyze(string fullCmd)
        {
            if (_divider.TryDivide(fullCmd))
            {
                Args = _divider.Args;
                FreeArgs = _divider.FreeArgs;
                Switches = _divider.Switches;
            }
            else
            {
                Args = new Dictionary<string, string>();
                FreeArgs = new List<string>();
                Switches = new Dictionary<string, string>();
            }
            CommandName = _divider.CommandName;
            ArgString = _divider.ArgString;
            SimpleParams = _divider.SimpleArgs;
        }

        public void Analyze(string fullCmd, ICommand command)
        {
            if (_divider.TryDivide(fullCmd))
            {
                command.Args = _divider.Args;
                command.FreeArgs = _divider.FreeArgs;
                command.Switches = _divider.Switches;
            }
            else
            {
                command.Args = new Dictionary<string, string>();
                command.FreeArgs = new List<string>();
                command.Switches = new Dictionary<string, string>();
            }
            command.Command = _divider.CommandName;
            command.ArgString = _divider.ArgString;
            command.SimpleArgs = _divider.SimpleArgs;
        }
    }
}
