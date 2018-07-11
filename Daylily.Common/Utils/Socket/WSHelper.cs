using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daylily.Common.Utils.Socket;
using Newtonsoft.Json;

namespace Daylily.Common.Utils
{
    public static class WsHelper
    {
        public static WebSocket WebSocket { get; set; }
        public static int CutCount { get; set; } = 50;

        public static void Ws_MessageSend(LogList sender, WsMessageSendEventArgs args)
        {
            if (WebSocket == null) return;

            // 小部分数据也使用之前json格式传送
            Dictionary<string, NewLogList> keyValues = new Dictionary<string, NewLogList>();
            NewLogList logList = new NewLogList();
            logList.Add(args.Source, args.Info);
            keyValues.Add(args.FriendlyName, logList);
            string json = JsonConvert.SerializeObject(keyValues);

            byte[] byteArray = Encoding.Default.GetBytes(json);
            WebSocket.SendAsync(new ArraySegment<byte>(byteArray, 0, byteArray.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    
        public static async Task Response()
        {
            var buffer = new byte[1024 * 4];
            if (Logger.LogLists != null)
            {
                // 建立连接后传送本剪切后的完整数据
                string fullData = JsonConvert.SerializeObject(Logger.LogLists.Cut(CutCount));
                byte[] byteArray = Encoding.Default.GetBytes(fullData);
                await WebSocket.SendAsync(new ArraySegment<byte>(byteArray, 0, byteArray.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }

            WebSocketReceiveResult result =
                await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                string text = Encoding.Default.GetString(buffer).Trim('\0');
                // 循环接收数据
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Logger.Raw("> " + text); // Do things here.
                    Console.DaylilyConsole.Response(text);
                }
                //await WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                //    result.EndOfMessage, CancellationToken.None); // Send

                buffer = new byte[1024 * 4];
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
