using SMSRateLimiter.Domain.Contracts.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Infrastructure.Implementations.Caching
{
    public class RedisRateLimitCache(IConnectionMultiplexer redis) : IRateLimitCache
    {
        private readonly IDatabase _database = redis.GetDatabase();

        public async Task<int> IncrementAsync(string key, TimeSpan expiration)
        {
            // Atomically increment the value asynchronously.
            long value = await _database.StringIncrementAsync(key);
            if (value == 1)
            {
                // Set expiration if key was just created.
                await _database.KeyExpireAsync(key, expiration);
            }
            return (int)value;
        }

        public async Task<(bool Found, T Value)> TryGetValueAsync<T>(string key)
        {
            RedisValue redisValue = await _database.StringGetAsync(key, CommandFlags.None);
            if (redisValue.IsNullOrEmpty)
            {
                return await Task.FromResult<(bool, T)>((false, default(T)!));
            }
            if (typeof(T) == typeof(int) && int.TryParse(redisValue, out int intValue))
            {
                return await Task.FromResult<(bool, T)>((true, (T)(object)intValue));
            }
            return await Task.FromResult<(bool, T)>((false, default(T)!));
        }
    }
}
