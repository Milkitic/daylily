using Daylily.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Utils.LogUtils;
using Daylily.Common.Utils.Socket;

namespace Daylily.Common.Console
{
    public static class Startup
    {
        private static readonly DaylilyConsole Console = new DaylilyConsole();
        public static void RunConsole()
        {
            SocketLogger.MessageReceived += Ws_MessageReceived;
        }

        private static void Ws_MessageReceived(LogList sender, WsMessageReceivedEventArgs args)
        {
            Console.WsCall(null, args.Message);
        }
    }
}
