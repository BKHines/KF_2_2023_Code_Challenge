using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;

namespace KF_2_2023.Managers
{
    public class StatisticsControllerManager
    {
        private CustomLogger _logger { get; set; }
        private SettingsModel _options { get; set; }
        private IConnectionMultiplexer _redis { get; set; }
        private SampleStreamManager _streamMgr { get; set; }

        public StatisticsControllerManager(CustomLogger logger, IConnectionMultiplexer redis, SettingsModel options, SampleStreamManager streamMgr)
        {
            _redis = redis;
            _logger = logger;
            _options = options;
            _streamMgr = streamMgr;
        }

        public ConsumptionDataModel GetConsumptionData(int hashtagCount)
        {
            var cdm = _streamMgr.GetConsumptionData(hashtagCount);

            return cdm;
        }
    }
}
