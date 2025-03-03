using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Implementations.Services
{
    public class SmsRateLimiter(IRateLimitCache cache, int maxPerNumber, int maxGlobal) : ISmsRateLimiter
    {
        private readonly IRateLimitCache _cache = cache;
        private readonly int _maxPerNumber = maxPerNumber;
        private readonly int _maxGlobal = maxGlobal;
        private const string GlobalCounterKey = "GlobalCounter";

        public bool CanSendMessage(string phoneNumber)
        {
            // Use current UTC second to scope counters for a 1-second window
            var currentSecond = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var numberKey = $"{phoneNumber}-{currentSecond}";
            var globalKey = $"{GlobalCounterKey}-{currentSecond}";

            // Atomically increment the counters using the cache abstraction
            int numberCount = _cache.Increment(numberKey, TimeSpan.FromSeconds(1));
            int globalCount = _cache.Increment(globalKey, TimeSpan.FromSeconds(1));

            // Return true only if both limits are not exceeded.
            return numberCount <= _maxPerNumber && globalCount <= _maxGlobal;
        }

        public int GetGlobalMessageCount()
        {
            var currentSecond = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var globalKey = $"{GlobalCounterKey}-{currentSecond}";
            return _cache.TryGetValue<int>(globalKey, out var count) ? count : 0;
        }

        public int GetMessageCountForNumber(string phoneNumber)
        {
            var currentSecond = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var numberKey = $"{phoneNumber}-{currentSecond}";
            return _cache.TryGetValue<int>(numberKey, out var count) ? count : 0;
        }
    }
}
