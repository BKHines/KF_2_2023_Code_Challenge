using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using Redis.OM;
using StackExchange.Redis;

namespace KF_2_2023.DataAccess
{
    public class TweetDataAcces
    {
        private IConnectionMultiplexer _redis { get; set; }
        private RedisConnectionProvider _provider { get; set; }
        private IDatabase _db { get; set; }

        public TweetDataAcces(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
            _provider = new RedisConnectionProvider(_redis);
            _provider.Connection.CreateIndex(typeof(TweetModel));
        }

        public bool AddTweet(TweetModel tweetModel)
        {
            var tweets = _provider.RedisCollection<TweetModel>();
            tweets.InsertAsync(tweetModel);
            tweets.SaveAsync();
            return true;
        }
    }
}
