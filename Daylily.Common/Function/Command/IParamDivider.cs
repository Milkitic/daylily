using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common.Function.Command
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
