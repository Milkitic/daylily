using System.Collections.Generic;
using Daylily.Bot.Models.MessageList;
using Daylily.CoolQ.Models.CqResponse;

namespace Daylily.Bot
{
    class Dispatcher
    {
        public Dispatcher(List<IMessageList> messageLists)
        {
            _messageLists = messageLists;
        }

        private readonly List<IMessageList> _messageLists;

        public void SendToBack(Msg msg)
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
