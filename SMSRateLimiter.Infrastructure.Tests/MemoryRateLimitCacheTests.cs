using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Infrastructure.Implementations.Caching;

namespace SMSRateLimiter.Infrastructure.Tests
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
        public void Increment_ShouldReturn_IncrementedValue()
        {
            // Arrange
            var cache = CreateCache();
            string key = "testKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            int value1 = cache.Increment(key, expiration);
            int value2 = cache.Increment(key, expiration);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(value1, Is.EqualTo(1));
                Assert.That(value2, Is.EqualTo(2));
            });
        }

        [Test]
        public void TryGetValue_ShouldReturn_CorrectValue()
        {
            // Arrange
            var cache = CreateCache();
            string key = "testKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            cache.Increment(key, expiration);
            bool found = cache.TryGetValue<int>(key, out int value);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(found, Is.True);
                Assert.That(value, Is.EqualTo(1));
            });
        }

        [Test]
        public void TryGetValue_ShouldReturnFalse_ForNonExistentKey()
        {
            // Arrange
            var cache = CreateCache();
            string key = "nonExistentKey";

            // Act
            bool found = cache.TryGetValue<int>(key, out int value);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(found, Is.False);
                Assert.That(value, Is.EqualTo(0)); // default(int) is 0
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
            cache.Increment(key, expiration);
            bool foundBeforeDelay = cache.TryGetValue<int>(key, out int valueBefore);
            await Task.Delay(600); // Wait longer than expiration time
            bool foundAfterDelay = cache.TryGetValue<int>(key, out int valueAfter);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(foundBeforeDelay, Is.True);
                Assert.That(foundAfterDelay, Is.False);
            });
        }

        [Test]
        public void Increment_MultipleKeys_ShouldMaintainIsolation()
        {
            // Arrange
            var cache = CreateCache();
            string key1 = "key1";
            string key2 = "key2";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Act
            int value1 = cache.Increment(key1, expiration);
            int value2 = cache.Increment(key2, expiration);
            int value1Again = cache.Increment(key1, expiration);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(value1, Is.EqualTo(1));
                Assert.That(value2, Is.EqualTo(1));
                Assert.That(value1Again, Is.EqualTo(2));
            });
        }

        [Test]
        public void Increment_ShouldBeThreadSafe_UnderConcurrentAccess()
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
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < incrementsPerTask; j++)
                    {
                        cache.Increment(key, expiration);
                    }
                });
            }
            Task.WaitAll(tasks);
            bool found = cache.TryGetValue<int>(key, out int finalValue);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(found, Is.True);
                Assert.That(finalValue, Is.EqualTo(numTasks * incrementsPerTask));
            });
        }
    }
}