using DaylilyWeb.Functions.Applications;
using DaylilyWeb.Interface.CQHttp;
using DaylilyWeb.Models;
using DaylilyWeb.Models.CQRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions
{
    public class PrivateMsgHandler : IMsgHandler
    {
        private PrivateMsg CurrentMessageInfo { get; set; }
        public PrivateMsgHandler(PrivateMsg parsed_obj)
        {
            CurrentMessageInfo = parsed_obj;
        }

        public void HandleMessage()
        {
            string message = CurrentMessageInfo.message.Trim().Trim('\n').Trim('\r');
            string user = CurrentMessageInfo.user_id.ToString();
            if (message.Substring(0, 1) == "!")
            {
                if (message.IndexOf("roll ") == 1)
                {
                    string command = "!roll ";
                    message = message.Substring(command.Length, message.Length - command.Length);
                }
                else if (message.IndexOf("roll") == 1)
                {
                    HttpApi api = new HttpApi();
                    var result = Roll.Next().ToString();
                    Task<string> mes = api.SendPrivateMessageAsync(user, result);
                    Assist.Log.WriteLine(mes.Result, ToString());
                    return;
                }
            }
            throw new NotImplementedException();
        }
    }
}
