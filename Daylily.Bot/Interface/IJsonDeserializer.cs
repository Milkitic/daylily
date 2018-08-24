using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Bot.Interface
{
    public interface IJsonDeserializer
    {
        void Json_Received(object sender, JsonReceivedEventArgs args);
    }
}
