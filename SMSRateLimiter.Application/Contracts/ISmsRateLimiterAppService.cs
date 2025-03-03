using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Contracts
{
    public interface ISmsRateLimiterAppService
    {
        Task<bool> CanSendMessage(string phoneNumber);
        Task<int> GetGlobalMessageCount();
        Task<int> GetMessageCountForNumber(string phoneNumber);
    }
}
