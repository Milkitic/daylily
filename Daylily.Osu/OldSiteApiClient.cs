using CSharpOsu;
using CSharpOsu.Module;
using System.Collections.Generic;
using System.Linq;

namespace Daylily.Osu
{
    public static class OldSiteApiClient
    {
        public static OsuUser[] GetUserList(string osuId)
        {
            OsuClient osu = new OsuClient(OsuApiConfig.ApiKey);
            return osu.GetUser(osuId);
        }

        public static string GetUidByUsername(string username)
        {
            OsuUser userObj = GetUserList(username).FirstOrDefault(k => k.username == username);
            return userObj?.user_id;
        }

        public static string GetUserNameByUid(string uid)
        {
            OsuUser userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid);
            return userObj == null ? uid : userObj.username;
        }

        public static IEnumerable<string> GetUserNameByUid(IEnumerable<string> uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid)
            select userObj == null ? uid : userObj.username;

        public static IEnumerable<string> GetUserNameByUid(params string[] uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid)
            select userObj == null ? uid : userObj.username;

        public static int GetUser(string osuId, out OsuUser user)
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
    }
}
