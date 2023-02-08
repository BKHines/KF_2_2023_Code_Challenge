using KF_2_2023.DataAccess;
using KF_2_2023.Managers;
using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;

namespace KF_2_2023.Manager
{
    public class ServiceControllerManager
    {
        private CustomLogger _logger { get; set; }
        private SettingsModel _options { get; set; }
        private IConnectionMultiplexer _redis { get; set; }
        private SampleStreamManager _streamMgr { get; set; }

        public ServiceControllerManager(CustomLogger logger, IConnectionMultiplexer redis, SettingsModel options, SampleStreamManager streamMgr)
        {
            _redis = redis;
            _logger = logger;
            _options = options;
            _streamMgr = streamMgr;
        }

        public bool StartService(string ip)
        {
            var _svcCmd = new ServiceCommandModel("start", ip);

            _streamMgr.Start();
            _svcCmd.success = _streamMgr.isrunning;

            if (_options.PersistTweets)
            {
                var _da = new ServicesDataAccess(_logger, _redis);
                _da.AddCommand(_svcCmd);
            }

            return _svcCmd.success;
        }

        public bool StopService(string ip)
        {
            var _svcCmd = new ServiceCommandModel("stop", ip);
            _streamMgr.Stop();
            _svcCmd.success = !_streamMgr.isrunning;

            if (_options.PersistTweets)
            {
                var _da = new ServicesDataAccess(_logger, _redis);
                _da.AddCommand(_svcCmd);
            }

            return _svcCmd.success;
        }
    }
}
