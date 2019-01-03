using System;
using Daylily.Common.Utils.LoggerUtils;

namespace Daylily.Common.Utils.SocketUtils
{
    public delegate void WsMessageReceivedEventHandler(LogList sender, WsMessageReceivedEventArgs args);
    public delegate void WsMessageSendEventHandler(LogList sender, WsMessageSendEventArgs args);

    public class WsMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    public class WsMessageSendEventArgs : EventArgs
    {
        public string FriendlyName { get; set; }
        public string Source { get; set; }
        public string Info { get; set; }
    }
}
