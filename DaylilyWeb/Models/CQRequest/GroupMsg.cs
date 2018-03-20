using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQRequest
{
    public class GroupMsg
    {
        public long time { get; set; }
        public string post_type { get; set; }
        public string message_type { get; set; }
        public string sub_type { get; set; }
        public long message_id { get; set; }
        public long group_id { get; set; }
        public long user_id { get; set; }
        public string anonymous { get; set; }
        public string anonymous_flag { get; set; }
        public string message { get; set; }
        public long font { get; set; }
        public string self_id { get; set; }
    }
}
