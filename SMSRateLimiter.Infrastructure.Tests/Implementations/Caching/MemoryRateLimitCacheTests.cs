using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Infrastructure.Implementations.Caching;

namespace SMSRateLimiter.Infrastructure.Tests.Implementations.Caching
{
    [TestFixture]
    public class MemoryRateLimitCacheTests
    {
        private IRateLimitCache CreateCache()
        {
            // Create an instance of IMemoryCache
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            return new MemoryRateLimitCache(memoryCache);
        }

        [Test]
        public async Task Increment_ShouldReturn_IncrementedValue()
        {
            // Arrange
            var cache = CreateCache();
            string key = "testKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            int value1 = await cache.IncrementAsync(key, expiration);
            int value2 = await cache.IncrementAsync(key, expiration);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(value1, Is.EqualTo(1));
                Assert.That(value2, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task TryGetValue_ShouldReturn_CorrectValue()
        {
            // Arrange
            var cache = CreateCache();
            string key = "testKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            await cache.IncrementAsync(key, expiration);
            var result = await cache.TryGetValueAsync<int>(key);

            Assert.Multiple(() =>
             {
                 // Assert
                 Assert.That(result.Found, Is.True);
                 Assert.That(result.Value, Is.EqualTo(1));
             });
        }

        [Test]
        public async Task TryGetValue_ShouldReturnFalse_ForNonExistentKey()
        {
            // Arrange
            var cache = CreateCache();
            string key = "nonExistentKey";

            // Act
            var result = await cache.TryGetValueAsync<int>(key);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Found, Is.False);
                Assert.That(result.Value, Is.EqualTo(0)); // default(int) is 0
            });
        }

        [Test]
        public async Task Value_ShouldExpire_AfterExpirationTime()
        {
            // Arrange
            var cache = CreateCache();
            string key = "expiringKey";
            TimeSpan expiration = TimeSpan.FromMilliseconds(500);

            // Act
            await cache.IncrementAsync(key, expiration);
            var foundBeforeDelay = await cache.TryGetValueAsync<int>(key);
            await Task.Delay(600); // Wait longer than expiration time
            var foundAfterDelay = await cache.TryGetValueAsync<int>(key);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(foundBeforeDelay.Found, Is.True);
                Assert.That(foundAfterDelay.Found, Is.False);
            });
        }

        [Test]
        public async Task Increment_MultipleKeys_ShouldMaintainIsolation()
        {
            // Arrange
            var cache = CreateCache();
            string key1 = "key1";
            string key2 = "key2";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            int value1 = await cache.IncrementAsync(key1, expiration);
            int value2 = await cache.IncrementAsync(key2, expiration);
            int value1Again = await cache.IncrementAsync(key1, expiration);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(value1, Is.EqualTo(1));
                Assert.That(value2, Is.EqualTo(1));
                Assert.That(value1Again, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task Increment_ShouldBeThreadSafe_UnderConcurrentAccess()
        {
            // Arrange
            var cache = CreateCache();
            string key = "concurrentKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);
            int numTasks = 100;
            int incrementsPerTask = 10;
            var tasks = new Task[numTasks];

            // Act
            for (int i = 0; i < numTasks; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    for (int j = 0; j < incrementsPerTask; j++)
                    {
                        await cache.IncrementAsync(key, expiration);
                    }
                });
            }
            Task.WaitAll(tasks);
            var result = await cache.TryGetValueAsync<int>(key);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Found, Is.True);
                Assert.That(result.Value, Is.EqualTo(numTasks * incrementsPerTask));
            });
        }
    }
}