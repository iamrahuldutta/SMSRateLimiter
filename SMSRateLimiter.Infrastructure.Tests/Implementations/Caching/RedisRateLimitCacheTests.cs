using Moq;
using Newtonsoft.Json.Linq;
using SMSRateLimiter.Infrastructure.Implementations.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Infrastructure.Tests.Implementations.Caching
{
    [TestFixture]
    public class RedisRateLimitCacheTests
    {
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisRateLimitCache _cache;

        public RedisRateLimitCacheTests()
        {
            // Create a mock IDatabase.
            _databaseMock = new Mock<IDatabase>();

            // Create a mock IConnectionMultiplexer that returns our mocked database.
            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            connectionMultiplexerMock
                .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_databaseMock.Object);

            // Instantiate the RedisRateLimitCache using the mocked multiplexer.
            _cache = new RedisRateLimitCache(connectionMultiplexerMock.Object);
        }

        [Test]
        public async Task IncrementAsync_ShouldReturnIncrementedValue_AndSetExpiration_WhenValueIsOne()
        {
            // Arrange
            string key = "testKey";
            TimeSpan expiration = TimeSpan.FromSeconds(10);

            // Setup sequence: first call returns 1, second call returns 2.
            _databaseMock.SetupSequence(db => db.StringIncrementAsync(key, 1, CommandFlags.None))
                .ReturnsAsync(1)
                .ReturnsAsync(2);

            // Act
            int value1 = await _cache.IncrementAsync(key, expiration);
            int value2 = await _cache.IncrementAsync(key, expiration);

            // Assert
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(value1, Is.EqualTo(1));
                Assert.That(value2, Is.EqualTo(2));
            });

            // Verify that KeyExpireAsync is called only on the first increment (when value is 1).
            _databaseMock.Verify(
    db => db.KeyExpireAsync(key, expiration, StackExchange.Redis.ExpireWhen.Always, CommandFlags.None),
    Times.Once);
        }

        [Test]
        public async Task TryGetValueAsync_ShouldReturnFoundAndCorrectValue_WhenKeyExists()
        {
            // Arrange
            string key = "testKey";
            RedisValue redisValue = "42";
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(redisValue);

            // Act
            var result = await _cache.TryGetValueAsync<int>(key);

            // Assert
            Assert.True(result.Found);
            Assert.That(result.Value, Is.EqualTo(42));
        }

        [Test]
        public async Task TryGetValueAsync_ShouldReturnNotFound_WhenKeyDoesNotExist()
        {
            // Arrange
            string key = "nonExistentKey";
            RedisValue redisValue = RedisValue.Null;
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(redisValue);

            // Act
            var result = await _cache.TryGetValueAsync<int>(key);

            // Assert
            Assert.False(result.Found);
            Assert.That(result.Value, Is.EqualTo(0)); // default(int) is 0
        }
    }
}
