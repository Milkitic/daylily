using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQResponse
{
    public class PrivateMsgResponse
    {
        public string reply { get; set; }
        public bool auto_escape { get; set; } = false;
    }
}
