using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Models
{
    public class RateLimitOptions
    {
        public int MaxPerAccountPerSecond { get; set; }
        public int MaxPerNumberPerSecond { get; set; }
    }
}
