using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Implementations.Services
{
    public class SmsRateLimiter(IRateLimitCache cache, IClock clock, int maxPerNumber, int maxGlobal) : ISmsRateLimiter
    {
        private readonly IRateLimitCache _cache = cache;
        private readonly IClock _clock = clock;
        private readonly int _maxPerNumber = maxPerNumber;
        private readonly int _maxGlobal = maxGlobal;
        private const string GlobalCounterKey = "GlobalCounter";

        public async Task<bool> CanSendMessage(string phoneNumber)
        {
            // Use current UTC second to scope counters for a 1-second window
            var currentSecond = _clock.UtcNow.ToString("yyyyMMddHHmmss");
            var numberKey = $"{phoneNumber}-{currentSecond}";
            var globalKey = $"{GlobalCounterKey}-{currentSecond}";

            // Atomically increment the counters using the cache abstraction
            int numberCount = await _cache.IncrementAsync(numberKey, TimeSpan.FromSeconds(1));
            int globalCount = await _cache.IncrementAsync(globalKey, TimeSpan.FromSeconds(1));

            // Return true only if both limits are not exceeded.
            return numberCount <= _maxPerNumber && globalCount <= _maxGlobal;
        }

        public async Task<int> GetGlobalMessageCount()
        {
            var currentSecond = _clock.UtcNow.ToString("yyyyMMddHHmmss");
            var globalKey = $"{GlobalCounterKey}-{currentSecond}";
            var (Found, Value) = await _cache.TryGetValueAsync<int>(globalKey);
            return Found ? Value : 0;
        }

        public async Task<int> GetMessageCountForNumber(string phoneNumber)
        {
            var currentSecond = _clock.UtcNow.ToString("yyyyMMddHHmmss");
            var numberKey = $"{phoneNumber}-{currentSecond}";
            var (Found, Value) = await _cache.TryGetValueAsync<int>(numberKey);
            return Found ? Value : 0;
        }
    }
}
