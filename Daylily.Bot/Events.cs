using System;
using Daylily.CoolQ.Models.CqResponse;

namespace Daylily.Bot
{
    public delegate void JsonReceivedEventHandler(object sender, JsonReceivedEventArgs args);

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs args);

    public class MessageReceivedEventArgs : EventArgs
    {
        public Msg MessageObj { get; set; }
    }

    public class JsonReceivedEventArgs : EventArgs
    {
        public string JsonString { get; set; }
    }
}
