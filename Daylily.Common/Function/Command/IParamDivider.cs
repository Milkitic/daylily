using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Function.Command
{
    public interface IParamDivider
    {
        string CommandName { get; }
        string Parameter { get; }
        ConcurrentDictionary<string, string> Parameters { get; }
        ConcurrentDictionary<string, string> Switches { get; }
        List<string> SimpleParams { get; set; }
        bool TryDivide(string fullCmd);
    }
}
