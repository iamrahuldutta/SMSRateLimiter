using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Application.DTOs;
using SMSRateLimiter.Domain.Contracts.Services;
using SMSRateLimiter.Domain.Implementations.Services;

namespace SMSRateLimiter.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SmsController(ISmsRateLimiterAppService smsService, ISmsLogBatchLogger smsLogBatchLogger, IClock clock) : ControllerBase
    {
        private readonly ISmsRateLimiterAppService _smsService = smsService;
        private readonly ISmsLogBatchLogger _smsLogBatchLogger = smsLogBatchLogger;
        private readonly IClock _clock = clock;

        // POST: api/v1/sms/send
        [HttpPost("send")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequestDto smsRequest)
        {
            var timestamp = _clock.UtcNow;

            var canSend = await _smsService.CanSendMessageAsync(
                smsRequest.AccountId,
                smsRequest.PhoneNumber,
                timestamp
            );

            if (!canSend)
            {
                return BadRequest("Rate limit exceeded. Try again later.");
            }

            _smsLogBatchLogger.Enqueue(smsRequest.AccountId, smsRequest.PhoneNumber, _clock.UtcNow);

            // For this sample, we return a success response.
            return Ok(new { smsRequest.PhoneNumber, Sent = true });
        }
    }
}

