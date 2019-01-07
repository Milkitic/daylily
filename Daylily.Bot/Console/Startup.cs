using Daylily.Common.Logging;
using Daylily.Common.Web.Socket;

namespace Daylily.Bot.Console
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
