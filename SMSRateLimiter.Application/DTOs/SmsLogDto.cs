using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.DTOs
{
    public class SmsLogDto
    {
        public int AccountId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
