using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CosResponse
{
    class CosAnalyzer
    {
        public CosObject[] result_list { get; set; }
    }
    class CosObject
    {
        public int code { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public CosData data { get; set; }
    }

    class CosData
    {
        public int result { get; set; }
        public int forbid_status { get; set; }
        public double confidence { get; set; }
        public double hot_score { get; set; }
        public double normal_score { get; set; }
        public double porn_score { get; set; }
    }
}
