using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpOsu;
using CSharpOsu.Module;
using Daylily.Osu.Models;

namespace Daylily.Osu.Interface
{
    public static class OldSiteApi
    {
        public static OsuUser[] GetUserList(string osuId)
        {
            OsuClient osu = new OsuClient(OsuApiKey.ApiKey);
            return osu.GetUser(osuId);
        }

        public static string GetUidByUsername(string username)
        {
            OsuUser userObj = GetUserList(username).FirstOrDefault(k => k.username == username);
            return userObj?.user_id;
        }

        public static string GetUsernameByUid(string uid)
        {
            OsuUser userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid);
            return userObj == null ? uid : userObj.username;
        }

        public static IEnumerable<string> GetUsernameByUid(IEnumerable<string> uidList) =>
            from uid in uidList
            let userObj = GetUserList(uid).FirstOrDefault(k => k.user_id == uid)
            select userObj == null ? uid : userObj.username;

        public static IEnumerable<string> GetUsernameByUid(params string[] uidList) =>
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
