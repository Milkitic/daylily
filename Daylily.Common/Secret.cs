namespace Daylily.Common
{
    public class Secret
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public OsuSettings OsuSettings { get; set; }
        public CosSettings CosSettings { get; set; }
        public BotSettings BotSettings { get; set; }
        public TuLingSettings TuLingSettings { get; set; }
    }

    public struct ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string MyConnection { get; set; }
    }

    public struct OsuSettings
    {
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public struct CosSettings
    {
        public int AppId { get; set; }
        public string SecretId { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
    }
    public struct TuLingSettings
    {
        public string ApiKey { get; set; }
    }

    public struct BotSettings
    {
        public string PostUrl { get; set; }
        public string CqDir { get; set; }
        public string CommandFlag { get; set; }
    }
}
