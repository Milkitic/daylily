using Daylily.Bot.Message;

namespace Daylily.CoolQ.Message
{
    public class CommonMessageResponse : IResponse
    {
        public bool EnableAt { get; }
        public string Message { get; }
        public MessageType MessageType { get; }
        public bool Cancel { get; set; } = false;
        public bool Handled { get; set; } = false;
        public object Tag { get; set; }

        public CqIdentity CqIdentity
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Private:
                        return new CqIdentity(long.Parse(UserId), MessageType.Private);
                    case MessageType.Discuss:
                        return new CqIdentity(long.Parse(DiscussId), MessageType.Discuss);
                    case MessageType.Group:
                    default:
                        return new CqIdentity(long.Parse(GroupId), MessageType.Group);
                }
            }
        }

        public string UserId { get; }
        public string DiscussId { get; }
        public string GroupId { get; }

        public CommonMessageResponse()
        {
            Cancel = true;
        }

        /// <summary>
        /// 此为指定一个特定的群时再使用
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cqIdentity"></param>
        /// <param name="atId"></param>
        public CommonMessageResponse(string message, CqIdentity cqIdentity, string atId = null)
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
            UserId = atId.ToString();
        }

        public CommonMessageResponse(string message, CoolQNavigableMessage coolQNavigableMessage, bool enableAt = false)
        {
            Message = message;
            UserId = coolQNavigableMessage.UserId;
            MessageType = coolQNavigableMessage.MessageType;
            EnableAt = enableAt;
            switch (coolQNavigableMessage.MessageType)
            {
                case MessageType.Discuss:
                    DiscussId = coolQNavigableMessage.DiscussId;
                    break;
                case MessageType.Group:
                    GroupId = coolQNavigableMessage.GroupId;
                    break;
            }
        }
    }
}
