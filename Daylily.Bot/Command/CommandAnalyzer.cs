using Daylily.Bot.Models;
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

        public void Analyze(string fullCmd, CommonMessage commonMessage)
        {
            //outCm = (CommonMessage)inCm.Clone();
            if (_divider.TryDivide(fullCmd))
            {
                commonMessage.Args = _divider.Args;
                commonMessage.FreeArgs = _divider.FreeArgs;
                commonMessage.Switches = _divider.Switches;
            }
            else
            {
                commonMessage.Args = new Dictionary<string, string>();
                commonMessage.FreeArgs = new List<string>();
                commonMessage.Switches = new Dictionary<string, string>();
            }
            commonMessage.Command = _divider.CommandName;
            commonMessage.ArgString = _divider.ArgString;
            commonMessage.SimpleArgs = _divider.SimpleArgs;
        }
    }
}
