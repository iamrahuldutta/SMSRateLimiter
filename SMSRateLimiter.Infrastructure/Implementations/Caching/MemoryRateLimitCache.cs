using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SMSRateLimiter.Domain.Contracts.Caching;

namespace SMSRateLimiter.Infrastructure.Implementations.Caching
{
    public class MemoryRateLimitCache(IMemoryCache memoryCache) : IRateLimitCache
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly object _lock = new();

        public Task<int> IncrementAsync(string key, TimeSpan expiration)
        {
            // Ensures that only one thread can perform these operations at a time, preventing race conditions.
            lock (_lock)
            {
                int current = 0;
                if (!_memoryCache.TryGetValue(key, out current))
                {
                    current = 0;
                }
                current++;
                _memoryCache.Set(key, current, expiration);
                return Task.FromResult(current);
            }
        }

        public Task<(bool Found, T Value)> TryGetValueAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out object? cachedValue) && cachedValue is T value)
            {
                return Task.FromResult((true, value));
            }
            else
            {
                return Task.FromResult<(bool, T)>((false, default(T)!));
            }
        }
    }
}
