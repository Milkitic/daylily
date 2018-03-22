using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQResponse
{
    public class GroupList
    {
        public List<GroupInfo> ListInfo { get; set; }
       
    }
    public class GroupInfo
    {
        public long group_id { get; set; }
        public string group_name { get; set; }
    }
}
