namespace daylily.Plugins.Osu.UserPage;

public sealed class UserPageVm
{
    public UserPageVm(long userId, string rawHtml)
    {
        UserId = userId;
        RawHtml = rawHtml;
    }

    public long UserId { get; }
    public string RawHtml { get; }
}