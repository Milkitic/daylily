using System.Collections.Concurrent;
using System.Collections.Generic;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models
{
    public class CommonMessage
    {
        public string Message { get; set; }

        public string FullCommand { get; set; }
        public string Command { get; set; }
        public string Parameter { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, string> Switches { get; set; }
        public List<string> SimpleParams { get; set; }

        public MessageType MessageType { get; set; }
        public PermissionLevel PermissionLevel { get; set; }

        public string UserId { get; set; }
        public string DiscussId { get; set; }
        public string GroupId { get; set; }
        public long MessageId { get; set; }

        public PrivateMsg Private { get; set; }
        public DiscussMsg Discuss { get; set; }
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
    }
}
