using KF_2_2023.Manager;
using KF_2_2023.Managers;
using KF_2_2023.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneTimeBuckAPI.Core;
using StackExchange.Redis;
using System.Net;
using System.Net.Http;

namespace KF_2_2023.Controllers
{
    [Produces("application/json")]
    [EnableCors("KF22023CORS")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly SettingsModel _settings;
        private readonly CustomLogger _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConnectionMultiplexer _redis;
        private SampleStreamManager _streamMgr;

        public ServiceController(IOptions<SettingsModel> options, ILogger<ServiceController> logger, IHttpContextAccessor httpContext, IConnectionMultiplexer redis, SampleStreamManager streamMgr) 
        {
            _settings = options.Value;
            _logger = new CustomLogger(logger);
            _httpContext = httpContext;
            _redis = redis;
            _streamMgr = streamMgr;
        }

        /// <summary>
        /// Starts the consumption service on the server
        /// </summary>
        /// <returns></returns>
        [HttpGet("StartConsumption", Name = "StartConsumption")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult StartConsumption()
        {
            var _svcCmdMgr = new ServiceControllerManager(_logger, _redis, _settings, _streamMgr);
            try
            {
                if (_httpContext == null || _httpContext.HttpContext == null) 
                {
                    _logger.LogMessage(LogType.error, new CustomLog() { title = "Start Consumption", messages = new[] { "Http Context null when starting consumption" } });
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var _rip = _httpContext.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
                if (_rip == null)
                {
                    _logger.LogMessage(LogType.error, new CustomLog() { title = "Start Consumption", messages = new[] { "Remote IP address was null" } });
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var _ip = _rip.ToString();
                var started = _svcCmdMgr.StartService(_ip);
                return Ok(started);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(LogType.error, new CustomLog() { title = "Start Consumption", messages = new[] { "General Start Concumption Error" } }, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Stops the consumption service on the server
        /// </summary>
        /// <returns></returns>
        [HttpGet("StopConsumption", Name = "StopConsumption")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult StopConsumption()
        {
            var _svcCmdMgr = new ServiceControllerManager(_logger, _redis, _settings, _streamMgr);
            try
            {
                if (_httpContext == null || _httpContext.HttpContext == null)
                {
                    _logger.LogMessage(LogType.error, new CustomLog() { title = "Stop Consumption", messages = new[] { "Http Context null when stopping consumption" } });
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var _rip = _httpContext.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
                if (_rip == null)
                {
                    _logger.LogMessage(LogType.error, new CustomLog() { title = "Stop Consumption", messages = new[] { "Remote IP address was null" } });
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var _ip = _rip.ToString();
                var started = _svcCmdMgr.StopService(_ip);
                return Ok(started);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(LogType.error, new CustomLog() { title = "Stop Consumption", messages = new[] { "General Stop Concumption Error" } }, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
