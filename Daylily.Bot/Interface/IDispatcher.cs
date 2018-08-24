using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface IDispatcher
    {
        void Message_Received(object sender, MessageReceivedEventArgs args);
    }
}
