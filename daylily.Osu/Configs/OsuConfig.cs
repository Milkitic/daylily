using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Osu.Configs;

public sealed class OsuConfig : ConfigurationBase
{
    public string QQAesKey { get; set; } = "aeskey";
    public string QQAesIV { get; set; } = "aesiv";
    public int ClientId { get; set; } = 114514;
    public string ClientSecret { get; set; } = "ClientSecret";
    public string ServerRedirectUri { get; set; } = "ServerRedirectUri";
    public int ServerRedirectPort { get; set; } = 23333;

}