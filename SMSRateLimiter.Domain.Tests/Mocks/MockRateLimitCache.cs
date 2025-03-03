using SMSRateLimiter.Domain.Contracts.Caching;
using System.Collections.Concurrent;

namespace SMSRateLimiter.Domain.Tests.Mocks
{
    public class MockRateLimitCache : IRateLimitCache
    {
        // Using a thread-safe dictionary to simulate cache storage.
        private readonly ConcurrentDictionary<string, int> _store = new ConcurrentDictionary<string, int>();

        public Task<int> IncrementAsync(string key, TimeSpan expiration)
        {
            // Atomically increment the value associated with the key.
            int newValue = _store.AddOrUpdate(key, 1, (k, current) => current + 1);
            return Task.FromResult(newValue);
        }

        public Task<(bool Found, T Value)> TryGetValueAsync<T>(string key)
        {
            if (_store.TryGetValue(key, out int intValue))
            {
                if (typeof(T) == typeof(int))
                {
                    return Task.FromResult<(bool, T)>((true, (T)(object)intValue));
                }
            }
            return Task.FromResult<(bool, T)>((false, default(T)!));
        }
    }
}
