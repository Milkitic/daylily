using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface IFrontend
    {
        event MessageEventHandler MessageReceived;
        event ExceptionEventHandler ErrorOccured;

        void RawObject_Received(object rawObject);
    }
}
