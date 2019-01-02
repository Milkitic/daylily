using Daylily.Bot.Enum;

namespace Daylily.Bot.Models
{
    public class CommonMessageResponse
    {
        public bool EnableAt { get; }
        public string Message { get; }
        public MessageType MessageType { get; }

        public Identity Identity
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Private:
                        return new Identity(long.Parse(UserId), MessageType.Private);
                    case MessageType.Discuss:
                        return new Identity(long.Parse(DiscussId), MessageType.Discuss);
                    case MessageType.Group:
                    default:
                        return new Identity(long.Parse(GroupId), MessageType.Group);
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
        /// <param name="identity"></param>
        /// <param name="atId"></param>
        public CommonMessageResponse(string message, Identity identity, string atId = null)
        {
            Message = message;
            switch (identity.Type)
            {
                case MessageType.Private:
                    MessageType = MessageType.Private;
                    UserId = identity.Id.ToString();
                    break;
                case MessageType.Discuss:
                    MessageType = MessageType.Discuss;
                    DiscussId = identity.Id.ToString();
                    break;
                case MessageType.Group:
                    MessageType = MessageType.Group;
                    GroupId = identity.Id.ToString();
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
