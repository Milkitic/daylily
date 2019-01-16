using CSharpOsu.Standard;
using CSharpOsu.Standard.Module;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Osu
{
    public class OldSiteApiClient
    {
        private static OsuClient _client;

        public OldSiteApiClient()
        {
            if (_client == null)
                _client = new OsuClient(OsuApiConfig.ApiKey);
        }

        #region Beatmap

        public OsuBeatmap[] GetBeatmaps()
        {
            return _client.GetBeatmap();
        }

        public OsuBeatmap[] GetBeatmapByBid(int bId)
        {
            return _client.GetBeatmap(
                _id: bId,
                _isSet: false
            );
        }

        public OsuBeatmap[] GetBeatmapsBySid(int sId)
        {
            return _client.GetBeatmap(
                _id: sId,
                _isSet: true
            );
        }

        public IGrouping<string, OsuBeatmap>[] GetBeatmapsetsByCreator(string creator)
        {
            return _client.GetBeatmap(
                _u: creator
            ).GroupBy(k=>k.beatmapset_id).ToArray();
        }

        #endregion Beatmap

        #region User
        public OsuUser[] GetUserList(string osuId)
        {
            return _client.GetUser(osuId);
        }

        public string GetUidByUsername(string username)
        {
            OsuUser userObj = GetUserList(username).FirstOrDefault(k => k.UserName == username);
            return userObj?.user_id;
        }

        public string GetUserNameByUid(string uid)
        {
            OsuUser userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid);
            return userObj == null ? uid : userObj.UserName;
        }

        public IEnumerable<string> GetUserNameByUid(IEnumerable<string> uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid)
            select userObj == null ? uid : userObj.UserName;

        public IEnumerable<string> GetUserNameByUid(params string[] uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid)
            select userObj == null ? uid : userObj.UserName;

        public int GetUser(string osuId, out OsuUser user)
        {
            OsuUser[] list = GetUserList(osuId);
            if (list.Length == 0)
            {
                user = null;
                return 0;
            }
            user = list[0];
            return list.Length;
        }

        #endregion User
    }
}
