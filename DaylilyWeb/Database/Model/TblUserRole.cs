using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Database.Model
{
    public class TblUserRole
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string Role { get; set; }
        public long QQ { get; set; }
        public string LegacyUname { get; set; }
        public string CurrentUname { get; set; }
        public bool IsBanned { get; set; }
        public long RepeatCount { get; set; }
        public long SpeakingCount { get; set; }
        public int Mode { get; set; }
    }
}
