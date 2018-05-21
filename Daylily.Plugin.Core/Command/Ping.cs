using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    public class Ping : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.GroupId != null) // 不给予群聊权限
                return null;
            return new CommonMessageResponse("pong", message);
        }
    }
}
