namespace KF_2_2023.Models
{
    public class ConsumptionDataModel
    {
        public int TweetsConsumed { get; set; }
        public IEnumerable<string> TopHashtags { get; set; } = Enumerable.Empty<string>();
        public int TweetsPossiblySensitive { get; set; }
        //public IEnumerable<string> UniqueSources { get; set; }
    }
}
