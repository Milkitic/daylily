using System.Collections.Generic;

namespace CSharpOsu.Standard.Module
{
    public class Match
    {
        public string match_id { get; set; }
        public string name { get; set; }
        public string start_time { get; set; }
        public object end_time { get; set; }
    }

    public class Game
    {
        public string game_id { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string beatmap_id { get; set; }
        public string play_mode { get; set; }
        public string match_type { get; set; }
        public string scoring_type { get; set; }
        public string team_type { get; set; }
        public string mods { get; set; }
        public List<object> scores { get; set; }
    }

    public class OsuMatch
    {
        public Match match { get; set; }
        public List<Game> games { get; set; }
    }
}
