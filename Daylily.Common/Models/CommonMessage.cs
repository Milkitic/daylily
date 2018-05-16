using Daylily.Common.Models.CQResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Common.Models
{
    public class CommonMessage
    {
        public string Message { get; private set; }

        public string FullCommand { get; set; }
        public string Command { get; set; }
        public string Parameter { get; set; }

        public MessageType MessageType { get; private set; }
        public PermissionLevel PermissionLevel { get; set; }

        public string UserId { get; private set; }
        public string DiscussId { get; private set; }
        public string GroupId { get; private set; }
        public long MessageId { get; private set; }

        public PrivateMsg Private { get; private set; }
        public DiscussMsg Discuss { get; private set; }
        public GroupMsg Group { get; private set; }

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
    }
}
