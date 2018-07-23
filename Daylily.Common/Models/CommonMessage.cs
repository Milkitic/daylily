using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;
using Newtonsoft.Json;

namespace Daylily.Common.Models
{
    public class CommonMessage : ICloneable
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

        [JsonIgnore]
        public PrivateMsg Private { get; set; }
        [JsonIgnore]
        public DiscussMsg Discuss { get; set; }
        [JsonIgnore]
        public GroupMsg Group { get; set; }

        public CommonMessage()
        {
        }

        public CommonMessage(PrivateMsg privateMsg, PermissionLevel level = PermissionLevel.Public)
        {
            Message = privateMsg.Message;
            UserId = privateMsg.UserId.ToString();
            MessageId = privateMsg.MessageId;

            MessageType = MessageType.Private;
            PermissionLevel = level;

            Private = privateMsg;
        }

        public CommonMessage(DiscussMsg discussMsg, PermissionLevel level = PermissionLevel.Public)
        {
            Message = discussMsg.Message;
            UserId = discussMsg.UserId.ToString();
            DiscussId = discussMsg.DiscussId.ToString();
            MessageId = discussMsg.MessageId;

            MessageType = MessageType.Discuss;
            PermissionLevel = level;

            Discuss = discussMsg;
        }

        public CommonMessage(GroupMsg groupMsg, PermissionLevel level = PermissionLevel.Public)
        {
            Message = groupMsg.Message;
            UserId = groupMsg.UserId.ToString();
            GroupId = groupMsg.GroupId.ToString();
            MessageId = groupMsg.MessageId;

            MessageType = MessageType.Group;
            PermissionLevel = level;

            Group = groupMsg;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Encode()
        {
            Message = Message.Replace("\"", "---");
            FullCommand = FullCommand.Replace("\"", "---");
            Command = Command.Replace("\"", "---");
            ArgString = ArgString.Replace("\"", "---");

            var args = new Dictionary<string, string>();
            foreach (var arg in Args)
                args.Add(arg.Key.Replace("\"", "---"), arg.Value.Replace("\"", "---"));
            Args = args;
            var switches = new Dictionary<string, string>();
            foreach (var switch1 in Switches)
                switches.Add(switch1.Key.Replace("\"", "---"), switch1.Value.Replace("\"", "---"));
            Switches = switches;

            for (var i = 0; i < FreeArgs.Count; i++)
                FreeArgs[i] = FreeArgs[i].Replace("\"", "---");

            for (var i = 0; i < SimpleArgs.Count; i++)
                SimpleArgs[i] = SimpleArgs[i].Replace("\"", "---");
        }

        public void Decode()
        {
            FullCommand = FullCommand.Replace("---", "\"");
            Command = Command.Replace("---", "\"");
            ArgString = ArgString.Replace("---", "\"");

            var args = new Dictionary<string, string>();
            foreach (var arg in Args)
                args.Add(arg.Key.Replace("---", "\""), arg.Value.Replace("---", "\""));
            Args = args;
            var switches = new Dictionary<string, string>();
            foreach (var switch1 in Switches)
                switches.Add(switch1.Key.Replace("---", "\""), switch1.Value.Replace("---", "\""));
            Switches = switches;

            for (var i = 0; i < FreeArgs.Count; i++)
                FreeArgs[i] = FreeArgs[i].Replace("---", "\"");

            for (var i = 0; i < SimpleArgs.Count; i++)
                SimpleArgs[i] = SimpleArgs[i].Replace("---", "\"");
        }
    }
}
