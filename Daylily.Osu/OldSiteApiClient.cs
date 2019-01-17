using CSharpOsu.V1;
using CSharpOsu.V1.Beatmap;
using CSharpOsu.V1.User;
using System;
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
            return _client.GetBeatmaps();
        }

        public OsuBeatmap GetBeatmapByBid(BeatmapId mapId)
        {
            return _client.GetBeatmap(mapId);
        }

        public OsuBeatmap[] GetBeatmapsBySid(BeatmapSetId setId)
        {
            return _client.GetBeatmaps(setId);
        }

        public OsuBeatmapSet GetBeatmapSet(BeatmapComponent id)
        {
            switch (id)
            {
                case BeatmapId bid:
                    return _client.GetBeatmapSet(bid);
                case BeatmapSetId sid:
                    return _client.GetBeatmapSet(sid);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public OsuBeatmapSet[] GetBeatmapSetsByCreator(UserComponent creator)
        {
            var grouping = _client
                .GetBeatmaps(creator)
                .GroupBy(k => k.BeatmapSetId);
            return grouping
                .Select(s => s.ToList())
                .Select(beatmaps => new OsuBeatmapSet(beatmaps))
                .ToArray();
        }

        #endregion Beatmap

        #region User
        public OsuUser[] GetUserList(UserComponent user)
        {
            return _client.GetUsers(user);
        }

        public string GetUidByUserName(UserName username)
        {
            OsuUser userObj = GetUserList(username).FirstOrDefault(k => k.UserName == username.IdOrName);
            return userObj?.UserId.ToString();
        }

        public string GetUserNameByUid(UserId userId)
        {
            OsuUser userObj = GetUserList(userId).FirstOrDefault(k => k.UserId.ToString() == userId.IdOrName);
            return userObj?.UserName;
        }

        public IEnumerable<string> GetUserNameByUid(IEnumerable<UserId> userIds) =>
            from uid in userIds
            let userObj = GetUserList(uid).FirstOrDefault(k => k.UserId.ToString() == uid.IdOrName)
            select userObj?.UserName;

        public IEnumerable<string> GetUserNameByUid(params UserId[] uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.UserId.ToString() == uid.IdOrName)
            select userObj?.UserName;

        public int GetUser(UserComponent nameOrId, out OsuUser user)
        {
            OsuUser[] list = GetUserList(nameOrId);
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
