using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQResponse
{
    public class GroupMsgResponse
    {
        public string reply { get; set; }
        public bool auto_escape { get; set; }
        public bool at_sender { get; set; }
        public bool delete { get; set; }
        public bool kick { get; set; }
        public bool ban { get; set; }
    }
}
