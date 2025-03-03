using SMSRateLimiter.Domain.Contracts.Caching;

namespace SMSRateLimiter.Domain.Tests.Mocks
{
    public class MockRateLimitCache : IRateLimitCache
    {
        private readonly Dictionary<string, int> _store = [];

        public int Increment(string key, TimeSpan expiration)
        {
            if (!_store.ContainsKey(key))
                _store[key] = 0;
            _store[key]++;
            return _store[key];
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_store.TryGetValue(key, out int intValue))
            {
                if (typeof(T) == typeof(int))
                {
                    value = (T)(object)intValue;
                    return true;
                }
            }
            value = default;
            return false;
        }
    }
}
