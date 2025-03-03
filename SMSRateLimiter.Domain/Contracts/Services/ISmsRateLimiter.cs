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
        Task<bool> CanSendMessage(string phoneNumber);

        Task<int> GetGlobalMessageCount();
        Task<int> GetMessageCountForNumber(string phoneNumber);
    }
}
