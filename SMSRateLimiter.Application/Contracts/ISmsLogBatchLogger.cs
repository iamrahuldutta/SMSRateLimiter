using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Contracts
{
    public interface ISmsLogBatchLogger
    {
        void Enqueue(int accountId, string phoneNumber, DateTime timestamp);
    }
}
