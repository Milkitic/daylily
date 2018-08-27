using System;
using System.Collections.Generic;
using Daylily.Bot.Enum;
using Daylily.CoolQ.Models.CqResponse;
using Newtonsoft.Json;

namespace Daylily.Bot.Models
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
        public PermissionLevel PermissionLevel { get; set; }
        public MessageType MessageType { get; set; }

        public Identity Identity
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Private:
                        return new Identity(Convert.ToInt64(UserId), MessageType.Private);
                    case MessageType.Discuss:
                        return new Identity(Convert.ToInt64(DiscussId), MessageType.Discuss);
                    case MessageType.Group:
                    default:
                        return new Identity(Convert.ToInt64(GroupId), MessageType.Group);
                }
            }
        }

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

        public CommonMessage() { }

        public CommonMessage(Msg msg, PermissionLevel level = PermissionLevel.Public)
        {
            Message = msg.Message;
            UserId = msg.UserId.ToString();
            MessageId = msg.MessageId;
            PermissionLevel = level;

            switch (msg)
            {
                case PrivateMsg privateMsg:
                    MessageType = MessageType.Private;
                    Private = privateMsg;
                    break;
                case DiscussMsg discussMsg:
                    MessageType = MessageType.Discuss;
                    Discuss = discussMsg;
                    DiscussId = discussMsg.DiscussId.ToString();
                    break;
                case GroupMsg groupMsg:
                    MessageType = MessageType.Group;
                    Group = groupMsg;
                    GroupId = groupMsg.GroupId.ToString();
                    break;
            }
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
