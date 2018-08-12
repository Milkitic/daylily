using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExtendAppDemo
{
    class Program
    {
        static void Main(string[] args)
        {  
            // 由于一些特殊情况，传进来的json需要转义
            string sourceJson = args[0].Replace("+++", " ").Replace("---", "\\\"");

            // 反序列化成相应的类
            ReceivedJson received = JsonConvert.DeserializeObject<ReceivedJson>(sourceJson);

            // --开始逻辑处理
            var message = received.FreeArgs.Count != 0
                ? $"Hello {received.FreeArgs[0]}!"
                : "Hello World!";
            // --处理完成

            // 将message参数传入某个形如下面的类，并序列化为json
            SentJson sent = new SentJson
            {
                DiscussId = received.DiscussId,
                GroupId = received.GroupId,
                UserId = received.UserId,
                EnableAt = false,
                Message = message
            };

            // 最终将Json结果直接Print到CLI中
            Console.WriteLine(JsonConvert.SerializeObject(sent));
        }

        public class SentJson
        {
            public bool EnableAt { get; set; }
            public string Message { get; set; }
            public MessageType MessageType { get; set; }

            public string UserId { get; set; }
            public string DiscussId { get; set; }
            public string GroupId { get; set; }
        }

        // 收到的json包含以下所有的信息，可根据情况删减
        public class ReceivedJson
        {
            public string Message { get; set; }

            public string FullCommand { get; set; }
            public string Command { get; set; }
            public string ArgString { get; set; }
            public Dictionary<string, string> Args { get; set; }
            public List<string> FreeArgs { get; set; }
            public Dictionary<string, string> Switches { get; set; }
            public List<string> SimpleArgs { get; set; }
            public MessageType MessageType { get; set; }
            public PermissionLevel PermissionLevel { get; set; }

            public string UserId { get; set; }
            public string DiscussId { get; set; }
            public string GroupId { get; set; }
            public long MessageId { get; set; }
        }

        public enum MessageType
        {
            Private,
            Discuss,
            Group
        }

        public enum PermissionLevel
        {
            Public,
            Admin,
            Root
        }
    }
}
