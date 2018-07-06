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

        public ConcurrentDictionary<string, string> Parameters { get; private set; }
        public ConcurrentDictionary<string, string> Switches { get; set; }
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
            CommandName = _divider.CommandName;
            Parameter = _divider.Parameter;
            SimpleParams = _divider.SimpleParams;
        }
    }
}
