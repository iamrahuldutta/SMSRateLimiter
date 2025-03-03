using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SMSRateLimiter.Domain.Contracts.Caching;

namespace SMSRateLimiter.Infrastructure.Implementations.Caching
{
    public class MemoryRateLimitCache : IRateLimitCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly object _lock = new object();

        public MemoryRateLimitCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public int Increment(string key, TimeSpan expiration)
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
                return current;
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
