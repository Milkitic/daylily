using System.Collections.Generic;

namespace Daylily.Common.Models
{
    public class JsonSettings
    {
        public readonly Dictionary<string, Dictionary<string, string>> Plugins = new Dictionary<string, Dictionary<string, string>>();
    }
}
