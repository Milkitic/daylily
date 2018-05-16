using Daylily.Common.Assist;
using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Web.Function.Application
{
    public class CheckCqAt : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            if (message.MessageType != MessageType.Private)
            {
                if (message.Group != null && message.GroupId == "133605766" &&
                    DateTime.Now.Hour < 22 && DateTime.Now.Hour > 6)
                    return null;
                string[] ids = CQCode.GetAt(message.Message);
                if (ids != null && (ids.Contains("2181697779")|| ids.Contains("3421735167")))
                {
                    return new CommonMessageResponse("", message, true);
                }
            }
            return null;
        }
    }
}
