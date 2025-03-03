using Microsoft.AspNetCore.Mvc;
using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MetricsController : ControllerBase
    {
        private readonly ISmsLogRepository _smsLogRepository;

        public MetricsController(ISmsLogRepository smsLogRepository)
        {
            _smsLogRepository = smsLogRepository;
        }

        [HttpGet("get-logs")]
        public async Task<IActionResult> GetLogs(
           [Required][FromQuery] int accountId,
           [FromQuery] string? phoneNumber,
           [FromQuery] DateTime? from,
           [FromQuery] DateTime? to)
        {
            if (accountId <= 0)
                return BadRequest("AccountId is required.");

            var logs = await _smsLogRepository.GetLogsAsync(accountId, phoneNumber, from, to);

            return Ok(logs);
        }
    }
}
