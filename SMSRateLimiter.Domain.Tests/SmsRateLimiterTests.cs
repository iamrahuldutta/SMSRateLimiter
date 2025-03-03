using Microsoft.Extensions.Options;
using Moq;
using SMSRateLimiter.Domain.Contracts.Caching;
using SMSRateLimiter.Domain.Implementations.Services;
using SMSRateLimiter.Domain.Models;
using SMSRateLimiter.Domain.Tests.Mocks;

namespace SMSRateLimiter.Domain.Tests
{
    [TestFixture]
    public class SmsRateLimiterTests
    {

        private Mock<IRateLimitCache> _rateLimitCacheMock = null!;
        private SmsRateLimiter _smsRateLimiter = null!;
        private RateLimitOptions _rateLimitOptions = null!;

        [SetUp]
        public void Setup()
        {
            _rateLimitCacheMock = new Mock<IRateLimitCache>();
            _rateLimitOptions = new RateLimitOptions
            {
                MaxPerAccountPerSecond = 5,
                MaxPerNumberPerSecond = 2
            };
            _smsRateLimiter = new SmsRateLimiter(_rateLimitCacheMock.Object, Options.Create(_rateLimitOptions));
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldAllow_WhenBelowLimits()
        {
            var accountId = 1;
            var phoneNumber = "+1234567890";
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.IncrementAsync(It.Is<string>(k => k.StartsWith("account")), It.IsAny<TimeSpan>()))
                .ReturnsAsync(3);

            _rateLimitCacheMock
                .Setup(x => x.IncrementAsync(It.Is<string>(k => k.StartsWith("number")), It.IsAny<TimeSpan>()))
                .ReturnsAsync(1);

            var result = await _smsRateLimiter.CanSendMessageAsync(accountId, phoneNumber, timestamp);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldBlock_WhenAccountLimitExceeded()
        {
            var accountId = 1;
            var phoneNumber = "+1234567890";
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.IncrementAsync(It.Is<string>(k => k.StartsWith("account")), It.IsAny<TimeSpan>()))
                .ReturnsAsync(6); // Exceeds MaxPerAccountPerSecond (5)

            var result = await _smsRateLimiter.CanSendMessageAsync(accountId, phoneNumber, timestamp);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CanSendMessageAsync_ShouldBlock_WhenNumberLimitExceeded()
        {
            var accountId = 1;
            var phoneNumber = "+1234567890";
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.IncrementAsync(It.Is<string>(k => k.StartsWith("account")), It.IsAny<TimeSpan>()))
                .ReturnsAsync(4); // Below account limit

            _rateLimitCacheMock
                .Setup(x => x.IncrementAsync(It.Is<string>(k => k.StartsWith("number")), It.IsAny<TimeSpan>()))
                .ReturnsAsync(3); // Exceeds MaxPerNumberPerSecond (2)

            var result = await _smsRateLimiter.CanSendMessageAsync(accountId, phoneNumber, timestamp);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetMessageCountForAccountAsync_ShouldReturnCount_WhenFound()
        {
            var accountId = 1;
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.TryGetValueAsync<int>(It.IsAny<string>()))
                .ReturnsAsync((true, 10));

            var count = await _smsRateLimiter.GetGlobalMessageCountAsync(accountId, timestamp);

            Assert.AreEqual(10, count);
        }

        [Test]
        public async Task GetMessageCountForAccountAsync_ShouldReturnZero_WhenNotFound()
        {
            var accountId = 1;
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.TryGetValueAsync<int>(It.IsAny<string>()))
                .ReturnsAsync((false, 0));

            var count = await _smsRateLimiter.GetGlobalMessageCountAsync(accountId, timestamp);

            Assert.AreEqual(0, count);
        }

        [Test]
        public async Task GetMessageCountForNumberAsync_ShouldReturnCount_WhenFound()
        {
            var accountId = 1;
            var phoneNumber = "+1234567890";
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.TryGetValueAsync<int>(It.IsAny<string>()))
                .ReturnsAsync((true, 5));

            var count = await _smsRateLimiter.GetMessageCountForNumberAsync(accountId, phoneNumber, timestamp);

            Assert.AreEqual(5, count);
        }

        [Test]
        public async Task GetMessageCountForNumberAsync_ShouldReturnZero_WhenNotFound()
        {
            var accountId = 1;
            var phoneNumber = "+1234567890";
            var timestamp = DateTime.UtcNow;

            _rateLimitCacheMock
                .Setup(x => x.TryGetValueAsync<int>(It.IsAny<string>()))
                .ReturnsAsync((false, 0));

            var count = await _smsRateLimiter.GetMessageCountForNumberAsync(accountId, phoneNumber, timestamp);

            Assert.AreEqual(0, count);
        }
    }
}
