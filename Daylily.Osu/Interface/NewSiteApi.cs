using System;
using System.Collections.Generic;
using System.Text;
using Bleatingsheep.Osu.ApiV2b;
using Bleatingsheep.Osu.ApiV2b.Models;

namespace Daylily.Osu.Interface
{
    public static class NewSiteApi
    {
        public static Beatmapsets[] SearchBeatmaps(string keyword, BeatmapsetsSearchOptions options = null)
        {
            OsuApiV2Client client = new OsuApiV2Client(OsuApiKey.UserName, OsuApiKey.Password);
            return client.SearchBeatMapAsync(keyword, options).Result;
        }

        public static Beatmapsets[] SearchAllBeatmaps(string keyword, BeatmapsetsSearchOptions options = null)
        {
            OsuApiV2Client client = new OsuApiV2Client(OsuApiKey.UserName, OsuApiKey.Password);
            List<Beatmapsets> list = new List<Beatmapsets>();
            Beatmapsets[] tmpArray;
            int i = 1;
            do
            {
                //Console.WriteLine($"page {i}.");
                var newOptions = new BeatmapsetsSearchOptions
                {
                    Page = i
                };

                if (options != null)
                {
                    newOptions.Extra = options.Extra;
                    newOptions.Genre = options.Genre;
                    newOptions.Language = options.Language;
                    newOptions.Mode = options.Mode;
                    newOptions.Status = options.Status;
                }

                tmpArray = client.SearchBeatMapAsync(keyword, newOptions).Result;
                if (tmpArray.Length > 0)
                    list.AddRange(tmpArray);
                else
                {
                    //Console.WriteLine($"page {i} no result.");
                }

                i++;
            } while (tmpArray.Length > 0);

            return list.ToArray();
        }
    }
}
