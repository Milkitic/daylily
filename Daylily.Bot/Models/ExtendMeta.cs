using Daylily.Bot.Backend;
using Daylily.Bot.Enum;
using Newtonsoft.Json;

namespace Daylily.Bot.Models
{
    public class ExtendMeta
    {
        public string Program { get; set; }
        public string File { get; set; }
        public string Name { get; set; }
        public string[] Author { get; set; }
        public string Version { get; set; }
        public PluginVersion State { get; set; }
        public string[] Help { get; set; }
        public string[] Command { get; set; }

        [JsonIgnore]
        public int Major => int.Parse(Version.Split('.')[0]);
        [JsonIgnore]
        public int Minor => int.Parse(Version.Split('.')[1]);
        [JsonIgnore]
        public int Patch => int.Parse(Version.Split('.')[2]);
    }
}
