using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Application.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Implementations.Services
{
    public class SmsLogBatchLogger : ISmsLogBatchLogger
    {
        private readonly ConcurrentQueue<SmsLogDto> _queue;

        public SmsLogBatchLogger(ConcurrentQueue<SmsLogDto> queue)
        {
            _queue = queue;
        }

        public void Enqueue(int accountId, string phoneNumber, DateTime timestamp)
        {
            _queue.Enqueue(new SmsLogDto
            {
                AccountId = accountId,
                PhoneNumber = phoneNumber,
                Timestamp = timestamp
            });
        }
    }
}
