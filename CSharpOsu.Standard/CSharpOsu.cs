using CSharpOsu.Standard.Module;
using CSharpOsu.Standard.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

[assembly: CLSCompliant(true)]
namespace CSharpOsu.Standard
{
    public class OsuClient
    {
        Strings str = new Strings();

        /// <summary>
        /// Osu API Key
        /// </summary>
        /// <param name="key">API Key</param>
        public OsuClient(string key)
        {
            str.Key = key;
        }

        /// <summary>
        /// Get JSON to be pharsed.
        /// </summary>
        /// <param name="url">Url of the JSON.</param>
        /// <returns>Fetch JSON.</returns>
        string GetUrl(string url)
        { return new WebClient().DownloadString(url); }

        /// <summary>
        /// Get information about beatmaps.
        /// </summary>
        /// <param name="_id">Specify a beatmapset or beatmap id.</param>
        /// <param name="_isSet">Logical switch that determine if the request is a single beatmap or a beatmapset</param>
        /// <param name="_since">Return all beatmaps ranked since this date. Must be a MySQL date.</param>
        /// <param name="_u">Specify a user or a username to return metadata from.</param>
        /// <param name="_m">Mode (0 = osu!, 1 = Taiko, 2 = CtB, 3 = osu!mania). Optional, maps of all modes are returned by default.</param>
        /// <param name="_a">Specify whether converted beatmaps are included (0 = not included, 1 = included). Only has an effect if m is chosen and not 0. Converted maps show their converted difficulty rating. Optional, default is 0.</param>
        /// <param name="_h">The beatmap hash. It can be used, for instance, if you're trying to get what beatmap has a replay played in, as .osr replays only provide beatmap hashes (example of hash: a5b99395a42bd55bc5eb1d2411cbdf8b). Optional, by default all beatmaps are returned independently from the hash.</param>
        /// <param name="_limit">Amount of results. Optional, default and maximum are 500.</param>
        /// <returns>Fetch Beatmap.</returns>
        public OsuBeatmap[] GetBeatmap(int? _id = null, bool _isSet = true, string _since = null, string _u = null, mode? _m = null, conv? _a = null, string _h = null, int? _limit = null)
        {
            OsuBeatmap[] obj;
            string beatmap = str.Beatmap();
            if (_since != null)
            {
                beatmap = beatmap + "&since=" + _since;
            }
            else if (_u != null)
            {
                beatmap = beatmap + "&u=" + _u;
            }
            else if (_m != null)
            {
                beatmap = beatmap + "&m=" + _m;
            }
            else if (_a != null)
            {
                beatmap = beatmap + "&a=" + _a;
            }
            else if (_h != null)
            {
                beatmap = beatmap + "&h=" + _h;
            }
            else if (_limit != null)
            {
                beatmap = beatmap + "&limit=" + _limit;
            }
            else if (_id != null)
            {
                beatmap = (_isSet) ? beatmap + "&s=" + _id : beatmap + "&b=" + _id;
            }

            string html = GetUrl(beatmap);

            obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(html);
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].thumbnail = "https://b.ppy.sh/thumb/" + obj[i].beatmapset_id + "l.jpg";
                if (_id == null)
                { obj[i].beatmapset_url = "https://osu.ppy.sh/s/" + obj[i].beatmapset_id; obj[i].beatmap_url = "https://osu.ppy.sh/b/" + obj[i].beatmap_id; }
                else
                {
                    if (_isSet)
                    { obj[i].beatmapset_url = "https://osu.ppy.sh/s/" + obj[i].beatmapset_id; obj[i].beatmap_url = "NULL"; }
                    else
                    { obj[i].beatmapset_url = "https://osu.ppy.sh/s/" + obj[i].beatmapset_id; obj[i].beatmap_url = "https://osu.ppy.sh/b/" + obj[i].beatmap_id; }
                }
                obj[i].download = str.osuDowload + obj[i].beatmapset_id;
                obj[i].download_no_video = str.osuDowload + obj[i].beatmapset_id + "n";
                obj[i].osu_direct = str.osuDirect + obj[i].beatmapset_id;
                obj[i].bloodcat = str.Bloodcat + obj[i].beatmapset_id;
                switch (obj[i].approved)
                {
                    case "-2":
                        obj[i].approved_string = "Graveyard";
                        break;
                    case "-1":
                        obj[i].approved_string = "Pending";
                        break;
                    case "1":
                        obj[i].approved_string = "Ranked";
                        break;
                    case "2":
                        obj[i].approved_string = "Approved";
                        break;
                    case "3":
                        obj[i].approved_string = "Qualified";
                        break;
                    case "4":
                        obj[i].approved_string = "Loved";
                        break;
                    default:
                        obj[i].approved_string = "NULL";
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Get informations about a user.
        /// </summary>
        /// <param name="id">Specify a user id or a username to return metadata from (required).</param>
        /// <param name="_m">Mode (0 = osu!, 1 = Taiko, 2 = CtB, 3 = osu!mania). Optional, maps of all modes are returned by default.</param>
        /// <param name="_event_days">Max number of days between now and last event date. Range of 1-31. Optional, default value is 1.</param>
        /// <returns>Fetch User.</returns>
        public OsuUser[] GetUser(string id, mode? _m = null, int? _event_days = null)
        {
            string user = str.User(id);
            if (_m != null)
            {
                user = user + "&m=" + _m;
            }
            else if (_event_days != null)
            {
                user = user + "&u=" + _event_days;
            }
            OsuUser[] obj;
            string html = GetUrl(user);
            obj = JsonConvert.DeserializeObject<OsuUser[]>(html);
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].url = "https://osu.ppy.sh/u/" + obj[i].user_id;
                obj[i].image = "https://a.ppy.sh/" + obj[i].user_id;
                obj[i].flag = "https://new.ppy.sh/images/flags/" + obj[i].country + ".png";
                obj[i].flag_old = "https://s.ppy.sh/images/flags/" + obj[i].country.ToLower() + ".gif";
                obj[i].osutrack = "https://ameobea.me/osutrack/user/" + obj[i].UserName;
                obj[i].osustats = "https://osustats.ppy.sh/u/" + obj[i].UserName;
                obj[i].osuskills = "http://osuskills.tk/user/" + obj[i].UserName;
                obj[i].osuchan = "https://syrin.me/osuchan/u/" + obj[i].user_id;
                obj[i].spectateUser = "osu://spectate/" + obj[i].user_id;
            }
            return obj;
        }


        /// <summary>
        /// Get informations about scores from a beatmap.
        /// </summary>
        /// <param name="_b">Specify a beatmap_id to return score information from.</param>
        /// <param name="_u">Specify a user_id or a username to return score information for.</param>
        /// <param name="_m">Mode (0 = osu!, 1 = Taiko, 2 = CtB, 3 = osu!mania). Optional, default value is 0.</param>
        /// <param name="_mods">Specify a mod or mod combination (See https://github.com/ppy/osu-api/wiki#mods )</param>
        /// <param name="_limit">Amount of results from the top (range between 1 and 100 - defaults to 50).</param>
        /// <returns>Fetch Scores.</returns>
        public OsuScore[] GetScore(int _b, string _u = null, mode? _m = null, int? _mods = null, int? _limit = null)
        {
            string score = str.Score(_b);
            if (_u != null)
            {
                score = score + "&u=" + _u;
            }
            else if (_m != null)
            {
                score = score + "&m=" + _m;
            }
            else if (_mods != null)
            {
                score = score + "&mods=" + _mods;
            }
            else if (_limit != null)
            {
                score = score + "&limit=" + _limit;
            }
            OsuScore[] obj;
            string html = GetUrl(score);
            obj = JsonConvert.DeserializeObject<OsuScore[]>(html);

            return obj;
        }

        /// <summary>
        /// Get informations about a user best scores.
        /// </summary>
        /// <param name="_u">Specify a user_id or a username to return score information for.</param>
        /// <param name="_m">Mode (0 = osu!, 1 = Taiko, 2 = CtB, 3 = osu!mania). Optional, default value is 0.</param>
        /// <param name="_limit">Amount of results from the top (range between 1 and 100 - defaults to 50).</param>
        /// <returns>Fetch user best scores.</returns>
        public OsuUserBest[] GetUserBest(string _u, mode? _m = null, int? _limit = null)
        {
            string userbest = str.User_Best(_u);
            if (_m != null)
            {
                userbest = userbest + "&m=" + _m;
            }
            else if (_limit != null)
            {
                userbest = userbest + "&limit=" + _limit;
            }
            OsuUserBest[] obj;
            string html = GetUrl(userbest);
            obj = JsonConvert.DeserializeObject<OsuUserBest[]>(html);

            for (int i = 0; i < obj.Length; i++)
            {
                // Get Beatmap for the mode
                var currentObj = obj[i];
                var bt = GetBeatmap(Convert.ToInt32(currentObj.beatmap_id), _isSet: false);

                // Variables
                float accuracy = 0;
                var mapMode = (mode)Convert.ToInt32(bt[0].mode);
                float totalPointsOfHits;
                float totalNumberOfHits;

                // Logical switch for every mode
                if (mapMode == mode.osu)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50) * 50 + Convert.ToInt32(currentObj.count100) * 100 + Convert.ToInt32(currentObj.count300) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                else if (mapMode == mode.Taiko)
                {
                    totalPointsOfHits = (Convert.ToInt32(currentObj.count100) * 0.5f + Convert.ToInt32(currentObj.count300) * 1) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                else if (mapMode == mode.CtB)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50 + currentObj.count100 + currentObj.count300);
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300) + Convert.ToInt32(currentObj.countkatu);
                    accuracy = totalPointsOfHits / totalNumberOfHits;
                }
                else if (mapMode == mode.osuMania)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50) * 50 + Convert.ToInt32(currentObj.count100) * 100 + Convert.ToInt32(currentObj.countkatu) * 200 + Convert.ToInt32(currentObj.count300 + currentObj.countgeki) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.countkatu) + Convert.ToInt32(currentObj.count300) + Convert.ToInt32(currentObj.countgeki);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                currentObj.accuracy = (accuracy * 100).ToString();
            }
            return obj;
        }

        /// <summary>
        /// Get informations about a user recent scores.
        /// </summary>
        /// <param name="_u">Specify a user_id or a username to return score information for.</param>
        /// <param name="_m">Mode (0 = osu!, 1 = Taiko, 2 = CtB, 3 = osu!mania). Optional, default value is 0.</param>
        /// <param name="_limit">Amount of results from the top (range between 1 and 100 - defaults to 50).</param>
        /// <returns>Fetch user recent scores.</returns>
        public OsuUserRecent[] GetUserRecent(string _u, mode? _m = null, int? _limit = null)
        {
            string userbest = str.User_Recent(_u);
            if (_m != null)
            {
                userbest = userbest + "&m=" + _m;
            }
            else if (_limit != null)
            {
                userbest = userbest + "&limit=" + _limit;
            }
            OsuUserRecent[] obj;
            string html = GetUrl(userbest);
            obj = JsonConvert.DeserializeObject<OsuUserRecent[]>(html);

            for (int i = 0; i < obj.Length; i++)
            {
                // Get Beatmap for the mode
                var currentObj = obj[i];
                var bt = GetBeatmap(Convert.ToInt32(currentObj.beatmap_id), _isSet: false);

                // Variables
                float accuracy = 0;
                var mapMode = (mode)Convert.ToInt32(bt[0].mode);
                float totalPointsOfHits;
                float totalNumberOfHits;

                // Logical switch for every mode
                if (mapMode == mode.osu)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50) * 50 + Convert.ToInt32(currentObj.count100) * 100 + Convert.ToInt32(currentObj.count300) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                else if (mapMode == mode.Taiko)
                {
                    totalPointsOfHits = (Convert.ToInt32(currentObj.count100) * 0.5f + Convert.ToInt32(currentObj.count300) * 1) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                else if (mapMode == mode.CtB)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50 + currentObj.count100 + currentObj.count300);
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.count300) + Convert.ToInt32(currentObj.countkatu);
                    accuracy = totalPointsOfHits / totalNumberOfHits;
                }
                else if (mapMode == mode.osuMania)
                {
                    totalPointsOfHits = Convert.ToInt32(currentObj.count50) * 50 + Convert.ToInt32(currentObj.count100) * 100 + Convert.ToInt32(currentObj.countkatu) * 200 + Convert.ToInt32(currentObj.count300 + currentObj.countgeki) * 300;
                    totalNumberOfHits = Convert.ToInt32(currentObj.countmiss) + Convert.ToInt32(currentObj.count50) + Convert.ToInt32(currentObj.count100) + Convert.ToInt32(currentObj.countkatu) + Convert.ToInt32(currentObj.count300) + Convert.ToInt32(currentObj.countgeki);
                    accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
                }
                currentObj.accuracy = (accuracy * 100).ToString();
            }
            return obj;
        }

        /// <summary>
        /// Get informations about a multiplayer lobby.
        /// </summary>
        /// <param name="_mp">Match id to get information from.</param>
        /// <returns>Fetch multiplayer lobby.</returns>
        public OsuMatch GetMatch(int _mp)
        {
            OsuMatch obj;
            string match = str.Match(_mp);
            string html = GetUrl(match);
            obj = JsonConvert.DeserializeObject<OsuMatch>(html);

            return obj;
        }

        /// <summary>
        /// Get informations about a replay.
        /// </summary>
        /// <param name="_m">The mode the score was played in.</param>
        /// <param name="_b">The beatmap ID (not beatmap set ID!) in which the replay was played.</param>
        /// <param name="_u">The user that has played the beatmap.</param>
        /// <returns>Fetch replay data.</returns>
        public OsuReplay GetReplay(mode _m, int _b, string _u)
        {
            OsuReplay obj;
            string replay = str.Replay(_m, _b, _u);
            string html = GetUrl(replay);
            obj = JsonConvert.DeserializeObject<OsuReplay>(html);
            if (obj.error == null)
            {
                obj.error = "none";
            }
            else
            {
                throw new Exception("Replay data not available: " + obj.error);
            }

            return obj;
            // Note that the binary data you get when you decode above base64-string, is not the contents of an.osr-file.It is the LZMA stream referred to by the osu-wiki here:
            // The remaining data contains information about mouse movement and key presses in an wikipedia:LZMA stream(https://osu.ppy.sh/wiki/Osr_(file_format)#Format)
        }

        /// <summary>
        /// Get all the bytes to create a .osr file.
        /// </summary>
        /// <param name="_m">The mode the score was played in.</param>
        /// <param name="_b">The beatmap ID (not beatmap set ID!) in which the replay was played.</param>
        /// <param name="_u">The user that has played the beatmap.</param>
        /// <param name="t">Change the function to the one that returns bytes.</param>
        /// <param name="_mods">Specify a mod or mod combination (See https://github.com/ppy/osu-api/wiki#mods )</param>
        /// <param name="_count">This will specify which score to select.</param>
        /// <returns>.osr file bytes.</returns>
        public byte[] GetReplay(mode _m, string _u, int _b, int _mods = 0, int _count = 0)
        {
            var replay = GetReplay(_m, _b, _u);
            var sc = GetScore(_b, _u, _m, _mods);
            var bt = GetBeatmap(_b, _isSet: false);

            var score = sc[_count];
            var beatmap = bt[0];

            var bin = new BinHandler();
            var binWriter = new BinaryWriter(new MemoryStream());
            var binReader = new BinaryReader(binWriter.BaseStream);

            var replayHashData = bin.MD5Hash(score.maxcombo + "osu" + score.username + beatmap.file_md5 + score.score + score.rank);
            var content = Convert.FromBase64String(replay.content);
            var mode = Convert.ToInt32(_m).ToString();

            // Begin
            bin.writeByte(binWriter, mode);                                      // Write osu mode.
            bin.writeInteger(binWriter, 0.ToString());                           // Write osu version. (Unknown)
            bin.writeString(binWriter, beatmap.file_md5);                        // Write beatmap MD5.
            bin.writeString(binWriter, score.username);                          // Write username.
            bin.writeString(binWriter, replayHashData);                          // Write replay MD5.
            bin.writeShort(binWriter, score.count300);                           // Write 300s count.
            bin.writeShort(binWriter, score.count100);                           // Write 100s count.
            bin.writeShort(binWriter, score.count50);                            // Write 50s count.
            bin.writeShort(binWriter, score.countgeki);                          // Write geki count.
            bin.writeShort(binWriter, score.countkatu);                          // Write katu count.
            bin.writeShort(binWriter, score.countmiss);                          // Write miss count.
            bin.writeInteger(binWriter, score.score);                            // Write score.
            bin.writeShort(binWriter, score.maxcombo);                           // Write maxcombo.
            bin.writeByte(binWriter, score.perfect);                             // Write if the score is perfect or not.
            bin.writeInteger(binWriter, score.enabled_mods);                     // Write which mods where enabled.
            bin.writeString(binWriter, "");                                      // Write lifebar hp. (Unknown)
            bin.writeDate(binWriter, score.date);                                // Write replay timestamp.
            bin.writeInteger(binWriter, content.Length.ToString());              // Write replay content lenght.

            // Content
            binWriter.Write(content);                                            // Write replay content.


            // Final
            binWriter.Write(Convert.ToInt64(score.score_id));                    // Write score id.
            binWriter.Write(BitConverter.GetBytes(Convert.ToUInt32(0)), 4, 0);   // Write null byte.


            binReader.BaseStream.Position = 0;
            int streamLenght = (int)binReader.BaseStream.Length;


            // [WARNING!]
            // The Get Replay Data from osu!api dosen't have a parameter to retrieve a certain replay.
            // It is possible that the movement of the cursor to be wrong because of that.
            // There is no way to fix it until such parameter is added.

            return binReader.ReadBytes(streamLenght);

        }

    }
}
