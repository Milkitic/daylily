using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public class Ping : IApplication
    {
        public string Execute(string @params)
        {
            return "pong";
        }
    }
}
