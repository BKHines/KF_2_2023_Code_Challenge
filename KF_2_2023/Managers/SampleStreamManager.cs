using KF_2_2023.DataAccess;
using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming;
using Tweetinvi.Streaming.V2;

namespace KF_2_2023.Managers
{
    public sealed class SampleStreamManager
    {
        private static readonly object threadlock = new object();

        private SettingsModel _settings;
        private IConnectionMultiplexer _redis;

        private TwitterClient appClient;
        private ISampleStreamV2 sampleStream;
        private List<TweetModel> tweets { get; set; }
        private Dictionary<string, int> dyHashtags { get; set; }

        public bool isrunning { get; set; }

        public SampleStreamManager(IConnectionMultiplexer redis, SettingsModel settings)
        {
            _settings = settings;
            _redis = redis;

            appClient = new TwitterClient(settings.TwitterAPIKey, settings.TwitterAPIKeySecret, settings.TwitterBearerToken);
            tweets = new List<TweetModel>();
            dyHashtags = new Dictionary<string, int>();
        }

        public async void Start()
        {
            sampleStream = appClient.StreamsV2.CreateSampleStream();
            sampleStream.TweetReceived += (sender, eventArgs) =>
            {
                if (!isrunning) isrunning = true;
                SampleTweetReceived(eventArgs.Tweet);
            };
            await sampleStream.StartAsync();
        }

        public void Stop()
        {
            if (sampleStream != null)
            {
                sampleStream.StopStream();
                isrunning = false;
            }
        }

        public int GetConsumptionCount()
        {
            return tweets.Count;
        }

        public IEnumerable<string> GetTopHashtags(int count)
        {
            return dyHashtags != null ? dyHashtags.OrderByDescending(a => a.Value).Take(count).Select(a => a.Key) : new List<String>();
        }

        private void SampleTweetReceived(TweetV2 tweet)
        {
            var _tweet = _settings.PersistTweets ? new TweetModel(tweet, _redis) : new TweetModel(tweet);

            lock (threadlock)
            {
                tweets.Add(_tweet);

                if (_tweet.hashtags != null)
                {
                    foreach (var ht in _tweet.hashtags)
                    {
                        if (dyHashtags.ContainsKey(ht.Tag))
                        {
                            dyHashtags[ht.Tag] += 1;
                        }
                        else
                        {
                            dyHashtags.Add(ht.Tag, 1);
                        }
                    }
                }
            }

            if (_settings.PersistTweets)
            {
                // save to REDIS
                _tweet.PersistTweet();
            }
        }
    }
}
