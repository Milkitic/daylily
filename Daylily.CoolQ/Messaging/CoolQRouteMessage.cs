using Daylily.Bot.Messaging;
using Daylily.Bot.Session;
using Daylily.CoolQ.CoolQHttp.ResponseModel.Report;
using Newtonsoft.Json;
using System;

namespace Daylily.CoolQ.Messaging
{
    public sealed class CoolQRouteMessage : RouteMessage, ICloneable
    {
        public Authority CurrentAuthority { get; set; }
        public MessageType MessageType { get; set; }

        public CoolQIdentity CoolQIdentity => (CoolQIdentity)Identity;
        public override ISessionIdentity Identity
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Private:
                        return new CoolQIdentity(Convert.ToInt64(UserId), MessageType.Private);
                    case MessageType.Discuss:
                        return new CoolQIdentity(Convert.ToInt64(DiscussId), MessageType.Discuss);
                    case MessageType.Group:
                    default:
                        return new CoolQIdentity(Convert.ToInt64(GroupId), MessageType.Group);
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
        [JsonIgnore]
        public ReportBase ReportMeta { get; set; }

        public CoolQRouteMessage()
        {
        }

        public CoolQRouteMessage(string message, CoolQIdentity cqIdentity, string atId = null) :
            this(new Text(message), cqIdentity, atId)
        {
        }
        public CoolQRouteMessage(CoolQCode message, CoolQIdentity cqIdentity, string atId = null) :
            this(new CoolQMessage(message), cqIdentity, atId)
        {
        }

        public CoolQRouteMessage(CoolQMessage message, CoolQIdentity cqIdentity, string atId = null)
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
                Handled = Handled,
                Canceled = false
            };
        }

        public static CoolQRouteMessage Parse(CoolQMessageApi coolQMessageApi, Authority level = Authority.Public)
        {
            var coolQMessage = new CoolQRouteMessage
            {
                Message = CoolQMessage.Parse(coolQMessageApi.Message),
                UserId = coolQMessageApi.UserId.ToString(),
                MessageId = coolQMessageApi.MessageId,
                CurrentAuthority = level,
                ReportMeta = coolQMessageApi
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

        public CoolQRouteMessage Cancel()
        {
            Canceled = true;
            return this;
        }

        public CoolQRouteMessage Delay(TimeSpan delay)
        {
            DelayTime = delay;
            return this;
        }

        public CoolQRouteMessage ForceToSend()
        {
            IsForced = true;
            return this;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
