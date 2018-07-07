using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models.CQResponse;
using Daylily.Common.Models.MessageList;

namespace Daylily.Common.Function.Dispatch
{
    class Dispatcher
    {
        public Dispatcher(List<IMessageList> messageLists)
        {
            _messageLists = messageLists;
        }

        private readonly List<IMessageList> _messageLists;

        public void SendToBack(PrivateMsg msg)
        {
            switch (msg)
            {
                case GroupMsg groupMsg:
                    break;
                case DiscussMsg discussMsg:
                    break;
                case PrivateMsg privateMsg:
                    break;
            }
            
        }
    }
}
