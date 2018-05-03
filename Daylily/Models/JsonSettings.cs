using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Models
{
    public class JsonSettings
    {
        public _KeySettings ConnectionStrings { get; set; } = new _KeySettings();
    }
    public class _KeySettings
    {
        public string CQDir { get; set; }
        public string DefaultConnection { get; set; }
        public string MyConnection { get; set; }
        public string PostUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
