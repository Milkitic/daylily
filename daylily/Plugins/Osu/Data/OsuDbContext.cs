using Coosu.Api.V2.ResponseModels;
using Microsoft.EntityFrameworkCore;
using MilkiBotFramework.Plugining.Database;

namespace daylily.Plugins.Osu.Data
{
    public class OsuDbContext : PluginDbContext
    {
        public DbSet<OsuToken> Tokens { get; set; }
        public DbSet<BeatmapScan> BeatmapScans { get; set; }
        public DbSet<BeatmapStat> BeatmapStats { get; set; }
        public DbSet<BeatmapSubscribe> BeatmapSubscribes { get; set; }

        public async Task<long?> GetUserIdBySourceId(string qq)
        {
            var result = (await Tokens.FindAsync(qq))?.UserId;
            return result;
        }

        public async Task AddOrUpdateToken(string sourceId, long userId, TokenBase token)
        {
            var osuToken = new OsuToken
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                CreateTime = token.CreateTime,
                ExpiresIn = token.ExpiresIn,
                SourceId = sourceId,
                UserId = userId
            };

            if (token is UserToken userToken)
            {
                osuToken.RefreshToken = userToken.RefreshToken;
            }
            else
            {
                osuToken.IsPublic = true;
            }

            var exist = await Tokens.FindAsync(sourceId);
            if (exist == null)
            {
                Tokens.Add(osuToken);
            }
            else
            {
                exist.TokenType = token.TokenType;
                exist.AccessToken = token.AccessToken;
                exist.CreateTime = token.CreateTime;
                exist.ExpiresIn = token.ExpiresIn;
                exist.IsPublic = osuToken.IsPublic;
                exist.RefreshToken = exist.RefreshToken;
                exist.UserId = userId;
            }

            await SaveChangesAsync();
        }
    }
}