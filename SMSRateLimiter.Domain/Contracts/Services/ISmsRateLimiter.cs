using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Contracts.Services
{
    public interface ISmsRateLimiter
    {
        /// <summary>
        /// Determines if an SMS message can be sent from the given phone number
        /// without exceeding the per-number and global limits.
        /// </summary>
        Task<bool> CanSendMessageAsync(int accountId, string phoneNumber, DateTime timestamp);

        Task<int> GetGlobalMessageCountAsync(int accountId, DateTime timestamp);
        Task<int> GetMessageCountForNumberAsync(int accountId, string phoneNumber, DateTime timestamp);
    }
}
