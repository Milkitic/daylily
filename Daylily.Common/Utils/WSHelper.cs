using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Daylily.Common.Utils
{
    public static class WsHelper
    {
        public class NewLogList
        {
            public List<LogListObject> Data { get; set; } = new List<LogListObject>();

            public void Add(string source, string info) => Data.Add(new LogListObject { Source = source, Info = info });
        }

        public static void LogList_MessageReceived(LogList sender, WsMessageReceivedEventArgs args)
        {
            if (WebSocket == null) return;

            Dictionary<string, NewLogList> keyValues = new Dictionary<string, NewLogList>();
            NewLogList logList = new NewLogList();
            logList.Add(args.Source, args.Info);
            keyValues.Add(args.FriendlyName, logList);

            string data = JsonConvert.SerializeObject(keyValues);
            byte[] byteArray = Encoding.Default.GetBytes(data);
            WebSocket.SendAsync(new ArraySegment<byte>(byteArray, 0, byteArray.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public static WebSocket WebSocket { get; set; }

        public static async Task Response()
        {
            var buffer = new byte[1024 * 4];
            if (Logger.LogLists != null)
            {
                string fullData = JsonConvert.SerializeObject(Logger.LogLists.Cut(100));
                byte[] byteArray = Encoding.Default.GetBytes(fullData);
                await WebSocket.SendAsync(new ArraySegment<byte>(byteArray, 0, byteArray.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }

            // 以下接收部分均尚未实现
            WebSocketReceiveResult result =
                await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                 string text = Encoding.Default.GetString(buffer).Trim('\0');
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Logger.Raw("> " + text); // Do things here.
                    Console.DaylilyConsole.Response(text);
                }
                await WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                    result.EndOfMessage, CancellationToken.None); // Response

                buffer = new byte[1024 * 4];
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
