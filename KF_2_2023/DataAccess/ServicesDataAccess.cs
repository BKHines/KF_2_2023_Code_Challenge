using KF_2_2023.Models;
using OneTimeBuckAPI.Core;
using Redis.OM;
using StackExchange.Redis;

namespace KF_2_2023.DataAccess
{
    public class ServicesDataAccess
    {
        private CustomLogger _logger { get; set; }
        private IConnectionMultiplexer _redis { get; set; }
        private RedisConnectionProvider _provider { get; set; }
        private IDatabase _db { get; set; }

        public ServicesDataAccess(CustomLogger logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _db = _redis.GetDatabase();
            _provider = new RedisConnectionProvider(_redis);
            _provider.Connection.CreateIndex(typeof(ServiceCommandModel));
        }

        public bool AddCommand(ServiceCommandModel servicesCommand)
        {
            var services = _provider.RedisCollection<ServiceCommandModel>();
            services.InsertAsync(servicesCommand);
            services.SaveAsync();
            return true;
        }
    }
}
