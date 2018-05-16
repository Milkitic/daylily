using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin.Command
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
