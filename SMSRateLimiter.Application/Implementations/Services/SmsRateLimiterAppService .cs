using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Implementations.Services
{
    public class SmsRateLimiterAppService : ISmsRateLimiterAppService
    {
        private readonly ISmsRateLimiter _smsRateLimiter;

        public SmsRateLimiterAppService(ISmsRateLimiter smsRateLimiter)
        {
            _smsRateLimiter = smsRateLimiter;
        }

        public bool CanSendMessage(string phoneNumber)
        {
            return _smsRateLimiter.CanSendMessage(phoneNumber);
        }

        public int GetGlobalMessageCount()
        {
            return _smsRateLimiter.GetGlobalMessageCount();
        }

        public int GetMessageCountForNumber(string phoneNumber)
        {
            return _smsRateLimiter.GetMessageCountForNumber(phoneNumber);
        }
    }
}
