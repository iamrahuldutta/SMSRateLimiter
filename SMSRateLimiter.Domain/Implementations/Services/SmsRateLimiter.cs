using Microsoft.Extensions.Options;
using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Domain.Contracts.Services;
using SMSRateLimiter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Implementations.Services
{
    public class SmsRateLimiter(IRateLimitCache cache, IOptions<RateLimitOptions> options) : ISmsRateLimiter
    {
        private readonly IRateLimitCache _cache = cache;
       private const string GlobalCounterKey = "GlobalCounter";
        private readonly RateLimitOptions _options = options.Value;

        public async Task<bool> CanSendMessageAsync(int accountId, string phoneNumber, DateTime timestamp)
        {
            var accountKey = $"account:{accountId}:{timestamp:yyyyMMddHHmmss}";
            var numberKey = $"number:{accountId}:{phoneNumber}:{timestamp:yyyyMMddHHmmss}";

            var accountCount = await _cache.IncrementAsync(accountKey, TimeSpan.FromSeconds(1));
            var numberCount = await _cache.IncrementAsync(numberKey, TimeSpan.FromSeconds(1));

            if (accountCount >= _options.MaxPerAccountPerSecond || numberCount >= _options.MaxPerNumberPerSecond)
                return false;

            return true;
        }

        public async Task<int> GetGlobalMessageCountAsync(int accountId, DateTime timestamp)
        {
            var accountKey = $"account:{accountId}:{timestamp:yyyyMMddHHmmss}";
            var (found, value) = await _cache.TryGetValueAsync<int>(accountKey);
            return found ? value : 0;
        }

        public async Task<int> GetMessageCountForNumberAsync(int accountId, string phoneNumber, DateTime timestamp)
        {
            var numberKey = $"number:{accountId}:{phoneNumber}:{timestamp:yyyyMMddHHmmss}";
            var (found, value) = await _cache.TryGetValueAsync<int>(numberKey);
            return found ? value : 0;
        }
    }
}
