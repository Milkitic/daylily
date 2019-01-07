using System.Collections.Generic;
using System.Linq;

namespace Daylily.Plugin.Osu.M4M
{
    public static class M4MTool
    {
        public static int Have(this bool[] b1, bool[] b2)
        {
            if (b1 == null || b2 == null)
                return -1;
            if (b1.Length != b2.Length)
                return -1;
  
            return b1.Where((t, i) => t && b2[i]).Any() ? 1 : 0;
        }

        public static int Contains(this bool[] b1, bool[] b2)
        {
            if (b1 == null || b2 == null)
                return -1;
            if (b1.Length != b2.Length)
                return -1;
            return b1.Where((t, i) => !t && b2[i]).Any() ? 0 : 1;
        }

        public static int GetPreferanceTotalLength(this MatchInfo myInfo, MatchInfo otherInfo)
        {
            List<string> mods = new List<string>();
            for (var i = 0; i < myInfo.Preference.Length; i++)
            {
                var b = myInfo.Preference[i];
                if (!b) continue;
                switch (i)
                {
                    case 0:
                        mods.Add("osu");
                        break;
                    case 1:
                        mods.Add("taiko");
                        break;
                    case 2:
                        mods.Add("fruits");
                        break;
                    case 3:
                        mods.Add("mania");
                        break;
                }
            }

            LightMaps[] maps = otherInfo.Set.Beatmaps.Where(map => mods.Contains(map.Mode)).ToArray();
            return maps.Sum(map => map.TotalLength);
        }
    }
}
