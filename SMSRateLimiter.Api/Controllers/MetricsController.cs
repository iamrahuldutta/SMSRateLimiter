using Microsoft.AspNetCore.Mvc;
using SMSRateLimiter.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MetricsController(ISmsRateLimiterAppService smsService) : ControllerBase
    {
        private readonly ISmsRateLimiterAppService _smsService = smsService;

        // GET: api/v1/metrics/global
        [HttpGet("global")]
        public async Task<IActionResult> GlobalMetrics()
        {
            int globalCount = await _smsService.GetGlobalMessageCount();
            return Ok(new { timestamp = DateTime.UtcNow, messagesPerSecond = globalCount });
        }

        // GET: api/v1/metrics/per-number?phoneNumber=...
        [HttpGet("per-number")]
        public async Task<IActionResult> PerNumberMetrics([FromQuery] string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest("Phone number is required.");
            }
            int count = await _smsService.GetMessageCountForNumber(phoneNumber);
            return Ok(new { phoneNumber, timestamp = DateTime.UtcNow, messagesPerSecond = count });
        }
    }
}
