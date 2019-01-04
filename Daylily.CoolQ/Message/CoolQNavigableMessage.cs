using Daylily.Bot.Command;
using Daylily.Bot.Message;
using Daylily.CoolQ.Models.CqResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Daylily.CoolQ.Message
{
    public class CoolQNavigableMessage : NavigableMessage, ICloneable
    {
        public Authority Authority { get; set; }
        public MessageType MessageType { get; set; }

        public override ISessionIdentity Identity
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Private:
                        return new CqIdentity(Convert.ToInt64(UserId), MessageType.Private);
                    case MessageType.Discuss:
                        return new CqIdentity(Convert.ToInt64(DiscussId), MessageType.Discuss);
                    case MessageType.Group:
                    default:
                        return new CqIdentity(Convert.ToInt64(GroupId), MessageType.Group);
                }
            }
        }
        
        public string DiscussId { get; set; }
        public string GroupId { get; set; }
        public long MessageId { get; set; }

        [JsonIgnore]
        public PrivateMsg Private { get; set; }
        [JsonIgnore]
        public DiscussMsg Discuss { get; set; }
        [JsonIgnore]
        public GroupMsg Group { get; set; }

        public CoolQNavigableMessage() { }

        public static CoolQNavigableMessage Parse(Msg msg, Authority level = Authority.Public)
        {
            var coolQMessage = new CoolQNavigableMessage
            {
                Message = new CoolQMessage
                {
                    RawMessage = msg.Message
                },
                UserId = msg.UserId.ToString(),
                MessageId = msg.MessageId,
                Authority = level
            };
            switch (msg)
            {
                case PrivateMsg privateMsg:
                    coolQMessage.MessageType = MessageType.Private;
                    coolQMessage.Private = privateMsg;
                    break;
                case DiscussMsg discussMsg:
                    coolQMessage.MessageType = MessageType.Discuss;
                    coolQMessage.Discuss = discussMsg;
                    coolQMessage.DiscussId = discussMsg.DiscussId.ToString();
                    break;
                case GroupMsg groupMsg:
                    coolQMessage.MessageType = MessageType.Group;
                    coolQMessage.Group = groupMsg;
                    coolQMessage.GroupId = groupMsg.GroupId.ToString();
                    break;
            }

            return coolQMessage;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Encode()
        {
            ((CoolQMessage)Message).RawMessage = Message.RawMessage.Replace("\"", "---");
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
