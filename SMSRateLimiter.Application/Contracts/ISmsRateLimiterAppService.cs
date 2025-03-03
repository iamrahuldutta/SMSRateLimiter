using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Contracts
{
    public interface ISmsRateLimiterAppService
    {
        Task<bool> CanSendMessageAsync(int accountId, string phoneNumber, DateTime timestamp);
        Task<int> GetGlobalMessageCountAsync(int accountId, DateTime timestamp);
        Task<int> GetMessageCountForNumberAsync(int accountId, string phoneNumber, DateTime timestamp);
    }
}
