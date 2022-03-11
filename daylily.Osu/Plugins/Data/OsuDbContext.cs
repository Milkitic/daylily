using Coosu.Api.V2.ResponseModels;
using Microsoft.EntityFrameworkCore;
using MilkiBotFramework.Plugining.Database;

namespace daylily.Osu.Plugins.Data
{
    public class OsuDbContext : PluginDbContext
    {
        public DbSet<OsuToken> Tokens { get; set; }

        public async Task<long?> GetUserIdByQQ(long qq)
        {
            var result = (await Tokens.FindAsync(qq))?.UserId;
            return result;
        }

        public async Task AddOrUpdateToken(long qq, long userId, TokenBase token)
        {
            var osuToken = new OsuToken
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                CreateTime = token.CreateTime,
                ExpiresIn = token.ExpiresIn,
                QQ = qq,
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

            var exist = await Tokens.FindAsync(qq);
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