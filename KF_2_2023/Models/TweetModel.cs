using KF_2_2023.DataAccess;
using OneTimeBuckAPI.Core;
using Redis.OM.Modeling;
using StackExchange.Redis;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Tweetinvi.Models.V2;

namespace KF_2_2023.Models
{
    [Document(StorageType = StorageType.Json)]
    public class TweetModel
    {
        [Indexed]
        public string id { get; set; }
        public string text { get; set; } = string.Empty;
        public string authorId { get; set; }
        public bool possiblySensitive { get; set; }
        public DateTimeOffset createdAt { get; set; } = DateTime.Now;
        public HashtagV2[] hashtags { get; set; }
        public string source { get; set; } = string.Empty;

        public TweetDataAcces _da { get; set; }

        public TweetModel(TweetV2 tweet)
        {
            id = tweet.Id;
            text = tweet.Text;
            createdAt = tweet.CreatedAt;
            authorId = tweet.AuthorId;
            possiblySensitive = tweet.PossiblySensitive;
            hashtags = tweet.Entities.Hashtags;
            source = tweet.Source;
        }

        public TweetModel(TweetV2 tweet, IConnectionMultiplexer redis) : this(tweet)
        {
            _da = new TweetDataAcces(redis);
        }

        public bool PersistTweet()
        {
            if (_da != null)
            {
                return _da.AddTweet(this);
            }

            return false;
        }
    }
}
