using Daylily.Bot.Command;
using Daylily.Bot.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Daylily.Bot.Session;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;

namespace Daylily.CoolQ.Message
{
    public sealed class CoolQRouteMessage : RouteMessage, ICloneable
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

        public bool EnableAt { get; set; }

        [JsonIgnore]
        public CoolQPrivateMessageApi Private { get; set; }
        [JsonIgnore]
        public CoolQDiscussMessageApi Discuss { get; set; }
        [JsonIgnore]
        public CoolQGroupMessageApi Group { get; set; }

        public CoolQRouteMessage() { }

        public CoolQRouteMessage(string message, CqIdentity cqIdentity, string atId = null) :
            this(new CoolQMessage(new Text(message)), cqIdentity, atId)
        {
        }

        public CoolQRouteMessage(CoolQMessage message, CqIdentity cqIdentity, string atId = null)
        {
            Message = message;
            switch (cqIdentity.Type)
            {
                case MessageType.Private:
                    MessageType = MessageType.Private;
                    UserId = cqIdentity.Id.ToString();
                    break;
                case MessageType.Discuss:
                    MessageType = MessageType.Discuss;
                    DiscussId = cqIdentity.Id.ToString();
                    break;
                case MessageType.Group:
                    MessageType = MessageType.Group;
                    GroupId = cqIdentity.Id.ToString();
                    break;
            }

            if (atId == null) return;
            EnableAt = true;
            UserId = atId;
        }

        public CoolQRouteMessage ToSource(string message, bool enableAt = false)
        {
            return ToSource(new Text(message), enableAt);
        }
        public CoolQRouteMessage ToSource(CoolQCode message, bool enableAt = false)
        {
            return ToSource(new CoolQMessage(message), enableAt);
        }

        public CoolQRouteMessage ToSource(CoolQMessage message, bool enableAt = false)
        {
            return new CoolQRouteMessage
            {
                UserId = UserId,
                DiscussId = DiscussId,
                GroupId = GroupId,
                MessageType = MessageType,
                Message = message,
                EnableAt = enableAt,
                Handled = Handled
            };
        }

        public static CoolQRouteMessage Parse(CoolQMessageApi coolQMessageApi, Authority level = Authority.Public)
        {
            var coolQMessage = new CoolQRouteMessage
            {
                Message = new CoolQMessage
                {
                    RawMessage = coolQMessageApi.Message
                },
                UserId = coolQMessageApi.UserId.ToString(),
                MessageId = coolQMessageApi.MessageId,
                Authority = level
            };
            switch (coolQMessageApi)
            {
                case CoolQPrivateMessageApi privateMsg:
                    coolQMessage.MessageType = MessageType.Private;
                    coolQMessage.Private = privateMsg;
                    break;
                case CoolQDiscussMessageApi discussMsg:
                    coolQMessage.MessageType = MessageType.Discuss;
                    coolQMessage.Discuss = discussMsg;
                    coolQMessage.DiscussId = discussMsg.DiscussId.ToString();
                    break;
                case CoolQGroupMessageApi groupMsg:
                    coolQMessage.MessageType = MessageType.Group;
                    coolQMessage.Group = groupMsg;
                    coolQMessage.GroupId = groupMsg.GroupId.ToString();
                    break;
            }

            return coolQMessage;
        }

        public CoolQRouteMessage Handle()
        {
            Handled = true;
            return this;
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
