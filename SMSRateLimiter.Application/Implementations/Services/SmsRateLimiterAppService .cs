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

        public async Task<bool> CanSendMessageAsync(int accountId, string phoneNumber, DateTime timestamp)
        {
            return await _smsRateLimiter.CanSendMessageAsync(accountId, phoneNumber, timestamp);
        }

        public async Task<int> GetGlobalMessageCountAsync(int accountId, DateTime timestamp)
        {
            return await _smsRateLimiter.GetGlobalMessageCountAsync(accountId, timestamp);
        }

        public async Task<int> GetMessageCountForNumberAsync(int accountId, string phoneNumber, DateTime timestamp)
        {
            return await _smsRateLimiter.GetMessageCountForNumberAsync(accountId, phoneNumber, timestamp);
        }
    }
}
