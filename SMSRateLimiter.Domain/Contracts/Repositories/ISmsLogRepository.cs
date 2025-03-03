using SMSRateLimiter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Contracts.Repositories
{
    public interface ISmsLogRepository
    {
        Task<IEnumerable<SmsLog>> GetLogsAsync(int accountId, string? phoneNumber = null, DateTime? from = null, DateTime? to = null);
    }
}
