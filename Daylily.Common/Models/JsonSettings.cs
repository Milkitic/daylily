using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Common.Models
{
    public class JsonSettings
    {
        public Dictionary<string, Dictionary<string, string>> Plugins = new Dictionary<string, Dictionary<string, string>>();
    }
}
