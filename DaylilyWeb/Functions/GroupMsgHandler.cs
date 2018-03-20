using DaylilyWeb.Models;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions
{
    public class GroupMsgHandler : IMsgHandler
    {
        public GroupMsg CurrentMessageInfo { get; set; }
        public GroupMsgHandler(GroupMsg parsed_obj)
        {
            CurrentMessageInfo = parsed_obj;
        }

        public void HandleMessage()
        {
            throw new NotImplementedException();
        }
    }
}
