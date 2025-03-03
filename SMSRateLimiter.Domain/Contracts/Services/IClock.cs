using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Contracts.Services
{
    public interface IClock
    {
        DateTime UtcNow { get; }    
    }
}
