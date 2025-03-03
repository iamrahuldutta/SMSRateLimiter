using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Implementations.Services
{
    public class SmsRateLimiterAppService(ISmsRateLimiter smsRateLimiter) : ISmsRateLimiterAppService
    {
        private readonly ISmsRateLimiter _smsRateLimiter = smsRateLimiter;

        public async Task<bool> CanSendMessage(string phoneNumber)
        {
            return await _smsRateLimiter.CanSendMessage(phoneNumber);
        }

        public async Task<int> GetGlobalMessageCount()
        {
            return await _smsRateLimiter.GetGlobalMessageCount();
        }

        public async Task<int> GetMessageCountForNumber(string phoneNumber)
        {
            return await _smsRateLimiter.GetMessageCountForNumber(phoneNumber);
        }
    }
}
