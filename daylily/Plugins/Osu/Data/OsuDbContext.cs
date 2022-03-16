using Coosu.Api.V2.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MilkiBotFramework.Plugining.Database;

namespace daylily.Plugins.Osu.Data;

public class OsuDbContext : PluginDbContext
{
    public DbSet<OsuToken> Tokens { get; set; }
    public DbSet<OsuUserInfo> OsuUserInfos { get; set; }
    public DbSet<BeatmapScan> BeatmapScans { get; set; }
    public DbSet<BeatmapStat> BeatmapStats { get; set; }
    public DbSet<BeatmapSubscribe> BeatmapSubscribes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OsuUserInfo>(entity =>
        {
            entity.Property(k => k.RequestStatus)
                .HasConversion(
                    v => (v == null || v.Length == 0) ? default : string.Join(',', v),
                    v => v == default
                        ? Array.Empty<RequestStatus>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(Enum.Parse<RequestStatus>)
                            .ToArray(),
                    new ValueComparer<RequestStatus[]?>(true)
                );
            entity.Property(k => k.ModeIds)
                .HasConversion(
                    v => v.Count == 0 ? default : string.Join(',', v),
                    v => v == default
                        ? new HashSet<ModeId>()
                        : new HashSet<ModeId>(v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(Enum.Parse<ModeId>)),
                    new ValueComparer<HashSet<ModeId>>(true)
                );
            entity.Property(k => k.Id)
                .HasConversion(v => v, v => v);
        });
    }

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