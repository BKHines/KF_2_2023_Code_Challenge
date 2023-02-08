using KF_2_2023.Managers;
using KF_2_2023.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;
using System.Net;

namespace KF_2_2023.Controllers
{
    [Produces("application/json")]
    [EnableCors("KF22023CORS")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly SettingsModel _settings;
        private readonly CustomLogger _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly SampleStreamManager _streamMgr;

        public StatisticsController(IOptions<SettingsModel> options, ILogger<ServiceController> logger, IConnectionMultiplexer redis, SampleStreamManager streamMgr)
        {
            _settings = options.Value;
            _logger = new CustomLogger(logger);
            _redis = redis;
            _streamMgr = streamMgr;
        }

        /// <summary>
        /// Gets the up to date data of the active consumption service
        /// If no service is running, returns default values
        /// </summary>
        /// <param name="hashtagcount">The count of hashtags to return in the top hashtag list, between 0 and 20</param>
        /// <returns></returns>
        [HttpGet("GetConsumptionData", Name = "GetConsumptionData")]
        [ProducesResponseType(typeof(ConsumptionDataModel), (int)HttpStatusCode.OK)]
        public IActionResult GetConsumptionData(int hashtagcount)
        {
            if (_streamMgr.isrunning)
            {
                var _scm = new StatisticsControllerManager(_logger, _redis, _settings, _streamMgr);
                if (hashtagcount < 0 || hashtagcount > 20) hashtagcount = 10;
                var cdm = _scm.GetConsumptionData(hashtagcount);
                return Ok(cdm);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.PartialContent, new ConsumptionDataModel() { TopHashtags = new List<string>(), TweetsConsumed = 0 });
            }
        }
    }
}
