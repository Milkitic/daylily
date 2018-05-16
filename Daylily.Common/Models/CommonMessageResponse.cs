using Daylily.Common.Models.CQRequest.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Common.Models
{
    public class CommonMessageResponse
    {
        public bool EnableAt { get; private set; }
        public string Message { get; private set; }
        public MessageType MessageType { get; private set; }

        public string UserId { get; private set; }
        public string DiscussId { get; private set; }
        public string GroupId { get; private set; }

        public SendPrivateMsg Private { get; private set; }
        public SendDiscussMsg Discuss { get; private set; }
        public SendGroupMsg Group { get; private set; }

        /// <summary>
        /// 此为指定一个特定的群时再使用
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="enableAt"></param>
        public CommonMessageResponse(string message, string userId = null, bool enableAt = false)
        {
            UserId = userId;
            Message = message;
            EnableAt = enableAt;
        }
        public CommonMessageResponse(string message, CommonMessage commonMessage, bool enableAt = false)
        {
            Message = message;
            UserId = commonMessage.UserId;
            MessageType = commonMessage.MessageType;
            EnableAt = enableAt;
            switch (commonMessage.MessageType)
            {
                case MessageType.Private:
                    Private = new SendPrivateMsg(commonMessage.Private.UserId.ToString(), commonMessage.Private.Message);
                    break;
                case MessageType.Discuss:
                    Discuss = new SendDiscussMsg(commonMessage.Discuss.DiscussId.ToString(), commonMessage.Discuss.Message);
                    DiscussId = commonMessage.DiscussId;
                    break;
                case MessageType.Group:
                    Group = new SendGroupMsg(commonMessage.Group.GroupId.ToString(), commonMessage.Group.Message);
                    GroupId = commonMessage.GroupId;
                    break;
            }
        }

        public CommonMessageResponse(SendPrivateMsg privateMsg, string userId, bool enableAt = false)
        {
            UserId = privateMsg.UserId.ToString();

            MessageType = MessageType.Private;
            Private = privateMsg;
        }

        public CommonMessageResponse(SendDiscussMsg discussMsg, string userId, bool enableAt = false)
        {
            UserId = userId;
            DiscussId = discussMsg.DiscussId.ToString();

            MessageType = MessageType.Discuss;
            Discuss = discussMsg;
        }

        public CommonMessageResponse(SendGroupMsg groupMsg, string userId, bool enableAt = false)
        {
            UserId = userId;
            DiscussId = groupMsg.GroupId.ToString();

            MessageType = MessageType.Group;
            Group = groupMsg;
        }
    }
}
