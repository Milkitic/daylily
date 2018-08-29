using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Daylily.Bot.Interface;
using Daylily.Common;
using Daylily.Common.Utils.LoggerUtils;
using Daylily.CoolQ.Models.CqResponse;

namespace Daylily.Bot
{
    public static class Core
    {
        public static event JsonReceivedEventHandler JsonReceived;
        public static event MessageReceivedEventHandler MessageReceived;

        private static IJsonDeserializer _jsonDeserializer;
        private static IDispatcher _dispatcher;

        static Core()
        {
            Type t = typeof(Domain);
            var infos = t.GetProperties();
            foreach (var item in infos)
            {
                string path = item.GetValue(null, null) as string;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static void InitCore(IJsonDeserializer jsonDeserializer, IDispatcher dispatcher)
        {
            _jsonDeserializer = jsonDeserializer;
            _dispatcher = dispatcher;
        }

        public static void ReceiveJson(string json)
        {
            if (JsonReceived == null)
            {
                Logger.Error("未配置json解析");
                return;
            }

            JsonReceived.Invoke(null, new JsonReceivedEventArgs
            {
                JsonString = json
            });
        }

        public static void ReceiveMessage(Msg msg)
        {
            if (MessageReceived == null)
            {
                Logger.Error("未配置message解析");
                return;
            }

            MessageReceived.Invoke(null, new MessageReceivedEventArgs
            {
                MessageObj = msg
            });
        }
    }
}
