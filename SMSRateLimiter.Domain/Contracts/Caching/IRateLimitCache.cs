using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Domain.Contracts.Caching
{
    public interface IRateLimitCache
    {
        /// <summary>
        /// Atomically increments the counter stored at key. If the key does not exist, it is created.
        /// The expiration is applied for new keys.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="expiration">Expiration timespan</param>
        /// <returns>The updated counter value.</returns>
        int Increment(string key, TimeSpan expiration);

        bool TryGetValue<T>(string key, out T value);
    }
}
