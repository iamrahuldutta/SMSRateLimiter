using Microsoft.AspNetCore.Mvc;
using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Application.DTOs;

namespace SMSRateLimiter.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SmsController(ISmsRateLimiterAppService smsService) : ControllerBase
    {
        private readonly ISmsRateLimiterAppService _smsService = smsService;
        
        // POST: api/v1/sms/send
        [HttpPost("send")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequestDto smsRequest)
        {
            // Model validation via FluentValidation & ModelValidationFilter
            bool canSend = await _smsService.CanSendMessage(smsRequest.PhoneNumber);
            if (!canSend)
            {
                return BadRequest("Rate limit exceeded. Try again later.");
            }

            // For this sample, we return a success response.
            return Ok(new { smsRequest.PhoneNumber, Sent = true });
        }
    }
}

