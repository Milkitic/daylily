namespace CSharpOsu.Standard.Util
{
    internal class Strings
    {
        public string Key { get; internal set; }

        public string osuApiUrl = "https://osu.ppy.sh/api/";
        public string osuDowload = "https://osu.ppy.sh/d/";
        public string Bloodcat = "https://bloodcat.com/osu/s/";
        public string osuDirect = "osu://s/";

        public string osuBeatmap = "get_beatmaps?";
        public string osuScores = "get_scores?";
        public string osuUser = "get_user?";
        public string osuUserBest = "get_user_best?";
        public string osuUserRecent = "get_user_recent?";
        public string osuMatch = "get_match?";
        public string osuReplay = "get_replay?";

        public string Beatmap() { return osuApiUrl + osuBeatmap + "k=" + Key; }
        public string User(string id) { return osuApiUrl + osuUser + "k=" + Key + "&u=" + id; }
        public string Score(int beatmap) { return osuApiUrl + osuScores + "k=" + Key + "&b=" + beatmap; }
        public string User_Best(string user) { return osuApiUrl + osuUserBest + "k=" + Key + "&u=" + user; }
        public string User_Recent(string user) { return osuApiUrl + osuUserRecent + "k=" + Key + "&u=" + user; }
        public string Match(int match) { return osuApiUrl + osuMatch + "k=" + Key + "&mp=" + match; }
        public string Replay(mode mode, int beatmap, string user) { return osuApiUrl + osuReplay + "k=" + Key + "&m=" + mode + "&b=" + beatmap + "&u=" + user; }
    }
}
