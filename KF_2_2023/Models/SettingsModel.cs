namespace KF_2_2023.Models
{
    public class SettingsModel
    {
        public bool PersistTweets { get; set; }
        public string RedisEndpoint { get; set; } = string.Empty;
        public int MinThreadPools { get; set; } = 0;
        public string APIUrl { get; set; } = string.Empty;
        public string TwitterAPIKey { get; set; } = string.Empty;
        public string TwitterAPIKeySecret { get; set; } = string.Empty;
        public string TwitterBearerToken { get; set; } = string.Empty;
    }
}
