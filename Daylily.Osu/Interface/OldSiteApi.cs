using System;
using System.Collections.Generic;
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

        public static int GetUser(string osuId, out OsuUser user)
        {
            OsuUser[] list = GetUserList(osuId);
            user = list[0];
            return list.Length;
        }
    }
}
