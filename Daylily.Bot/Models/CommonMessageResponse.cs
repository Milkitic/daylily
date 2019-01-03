using Daylily.Bot.Enum;

namespace Daylily.Bot.Models
{
    public class CommonMessageResponse
    {
        public bool EnableAt { get; }
        public string Message { get; }
        public MessageType MessageType { get; }

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

        public CommonMessageResponse() { }

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

        public CommonMessageResponse(string message, CommonMessage commonMessage, bool enableAt = false)
        {
            Message = message;
            UserId = commonMessage.UserId;
            MessageType = commonMessage.MessageType;
            EnableAt = enableAt;
            switch (commonMessage.MessageType)
            {
                case MessageType.Discuss:
                    DiscussId = commonMessage.DiscussId;
                    break;
                case MessageType.Group:
                    GroupId = commonMessage.GroupId;
                    break;
            }
        }
    }
}
