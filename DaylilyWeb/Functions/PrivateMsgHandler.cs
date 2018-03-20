﻿using DaylilyWeb.Assist;
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
        HttpApi CQApi = new HttpApi();

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
                    string result;
                    var query = message.Substring(command.Length, message.Length - command.Length).Split(' ');
                    if (!int.TryParse(query[0], out int a))
                    {
                        Task<string> msgText = CQApi.SendPrivateMessageAsync(user, Roll.Next().ToString());
                        Log.WriteLine(msgText.Result, ToString());
                        return;
                    }
                    else if (query.Length == 1)
                    {
                        result = Roll.Next(int.Parse(query[0])).ToString();
                    }
                    else if (query.Length == 2)
                    {
                        result = Roll.Next(int.Parse(query[0]), int.Parse(query[1])).ToString();
                    }
                    else if (query.Length == 3)
                    {
                        result = Roll.Next(int.Parse(query[0]), int.Parse(query[1]), int.Parse(query[2])).ToString();
                    }
                    else throw new ArgumentException();
                    Task<string> mes = CQApi.SendPrivateMessageAsync(user, result);
                    Log.WriteLine(mes.Result, ToString());
                    return;
                }
                else if (message.IndexOf("roll") == 1)
                {
                    string result = Roll.Next().ToString();
                    Task<string> mes = CQApi.SendPrivateMessageAsync(user, result);
                    Log.WriteLine(mes.Result, ToString());
                    return;
                }
                throw new NotImplementedException();
            }
        }
    }
}
