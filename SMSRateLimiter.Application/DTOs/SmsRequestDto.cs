﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.DTOs
{
    public class SmsRequestDto
    {
        public int AccountId { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
