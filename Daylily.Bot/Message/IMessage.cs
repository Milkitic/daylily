using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Message
{
    public interface IMessage
    {
        string RawMessage { get; }
    }
}
