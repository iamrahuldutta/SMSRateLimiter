using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Entities
{
    public class SmsLog
    {
        // Primary key for the table.
        public int Id { get; set; }

        public int AccountId { get; set; }

        // The phone number that was allowed.
        public string PhoneNumber { get; set; } = string.Empty;

        // The UTC timestamp when the allowed SMS send occurred.
        public DateTime Timestamp { get; set; }
    }
}
