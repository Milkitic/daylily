using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models;

namespace Daylily.Common.Function.Command
{
    public class CommandAnalyzer
    {
        public string CommandName { get; private set; }
        public string Parameter { get; private set; }

        public Dictionary<string, string> Parameters { get; private set; }
        public Dictionary<string, string> Switches { get; set; }
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
                Parameters = _divider.Parameters;
                Switches = _divider.Switches;
            }
            else
            {
                Parameters = new Dictionary<string, string>();
                Switches = new Dictionary<string, string>();
            }
            CommandName = _divider.CommandName;
            Parameter = _divider.Parameter;
            SimpleParams = _divider.SimpleParams;
        }

        public void Analyze(string fullCmd, CommonMessage commonMessage)
        {
            //outCm = (CommonMessage)inCm.Clone();
            if (_divider.TryDivide(fullCmd))
            {
                commonMessage.Parameters = _divider.Parameters;
                commonMessage.Switches = _divider.Switches;
            }
            else
            {
                commonMessage.Parameters = new Dictionary<string, string>();
                commonMessage.Switches = new Dictionary<string, string>();
            }
            commonMessage.Command = _divider.CommandName;
            commonMessage.Parameter = _divider.Parameter;
            commonMessage.SimpleParams = _divider.SimpleParams;
        }
    }
}
