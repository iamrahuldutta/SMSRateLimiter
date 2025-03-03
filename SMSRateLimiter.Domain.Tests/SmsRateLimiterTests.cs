using SMSRateLimiter.Domain.Implementations.Services;
using SMSRateLimiter.Domain.Tests.Mocks;

namespace SMSRateLimiter.Domain.Tests
{
    [TestFixture]
    public class SmsRateLimiterTests
    {

        [Test]
        public async Task CanSendMessage_ReturnsTrue_UnderLimit()
        {
            // Arrange
            var fakeCache = new MockRateLimitCache();
            // For this test, set a per-number limit of 5 and a global limit of 10.
            var limiter = new SmsRateLimiter(fakeCache, new MockSystemClock(DateTime.UtcNow), maxPerNumber: 5, maxGlobal: 10);
            string phoneNumber = "+1234567890";

            // Act: first call should be under the limit.
            bool result = await limiter.CanSendMessage(phoneNumber);

            // Assert: The message can be sent.
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanSendMessage_ReturnsFalse_WhenNumberLimitExceeded()
        {
            // Arrange
            var fakeCache = new MockRateLimitCache();
            // Use a small per-number limit for testing.
            var limiter = new SmsRateLimiter(fakeCache, new MockSystemClock(DateTime.UtcNow), maxPerNumber: 3, maxGlobal: 100);
            string phoneNumber = "+1234567890";

            // Act: Call the method 4 times within the same second.
            bool result1 = await limiter.CanSendMessage(phoneNumber);
            bool result2 = await limiter.CanSendMessage(phoneNumber);
            bool result3 = await limiter.CanSendMessage(phoneNumber);
            bool result4 = await limiter.CanSendMessage(phoneNumber);

            Assert.Multiple(() =>
            {
                // Assert: First three calls are within limit; the fourth exceeds the per-number limit.
                Assert.That(result1, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(result3, Is.True);
                Assert.That(result4, Is.False);
            });
        }

        [Test]
        public async Task CanSendMessage_ReturnsFalse_WhenGlobalLimitExceeded()
        {
            // Arrange
            var fakeCache = new MockRateLimitCache();
            // Use a small global limit.
            var limiter = new SmsRateLimiter(fakeCache, new MockSystemClock(DateTime.UtcNow), maxPerNumber: 100, maxGlobal: 3);
            string phoneNumber1 = "+1234567890";
            string phoneNumber2 = "+0987654321";

            // Act: Call across phone numbers to exceed the global limit.
            bool r1 = await limiter.CanSendMessage(phoneNumber1);
            bool r2 = await limiter.CanSendMessage(phoneNumber2);
            bool r3 = await limiter.CanSendMessage(phoneNumber1);
            bool r4 = await limiter.CanSendMessage(phoneNumber2);

            Assert.Multiple(() =>
            {
                // Assert: With a global limit of 3, the first three calls are accepted, the fourth is rejected.
                Assert.That(r1, Is.True);
                Assert.That(r2, Is.True);
                Assert.That(r3, Is.True);
                Assert.That(r4, Is.False);
            });
        }

        [Test]
        public async Task GetGlobalMessageCount_ReturnsCorrectCount()
        {
            // Arrange
            var fakeCache = new MockRateLimitCache();
            var limiter = new SmsRateLimiter(fakeCache, new MockSystemClock(DateTime.UtcNow), maxPerNumber: 5, maxGlobal: 100);
            string phoneNumber = "+1234567890";

            // Act: Call CanSendMessage twice; global count should reflect 2 increments.
            await limiter.CanSendMessage(phoneNumber);
            await limiter.CanSendMessage(phoneNumber);

            int globalCount = await limiter.GetGlobalMessageCount();

            // Assert
            Assert.That(globalCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMessageCountForNumber_ReturnsCorrectCount()
        {
            // Arrange
            var fakeCache = new MockRateLimitCache();
            var limiter = new SmsRateLimiter(fakeCache, new MockSystemClock(DateTime.UtcNow), maxPerNumber: 5, maxGlobal: 100);
            string phoneNumber = "+1234567890";

            // Act: Call CanSendMessage twice; per-number counter should reflect 2 increments.
            await limiter.CanSendMessage(phoneNumber);
            await limiter.CanSendMessage(phoneNumber);

            int numberCount = await limiter.GetMessageCountForNumber(phoneNumber);

            // Assert
            Assert.That(numberCount, Is.EqualTo(2));
        }

        [Test]
        public async Task CanSendMessage_ResetsAfterExpiration()
        {
            var fakeClock = new MockSystemClock(DateTime.UtcNow);
            var fakeCache = new MockRateLimitCache();
            var limiter = new SmsRateLimiter(fakeCache, fakeClock, maxPerNumber: 5, maxGlobal: 100);
            string phoneNumber = "+1234567890";

            // Act: Send messages in the first second.
            await limiter.CanSendMessage(phoneNumber);  // counter becomes 1.
            Assert.That(await limiter.GetMessageCountForNumber(phoneNumber), Is.EqualTo(1));

            // Simulate the passage of time beyond the 1-second expiration.
            fakeClock.UtcNow = fakeClock.UtcNow.AddSeconds(2);

            // Act: In the new time window, the counter should be reset.
            int newCount = await limiter.GetMessageCountForNumber(phoneNumber);

            // Assert: newCount should be 0 because the previous counter expired.
            Assert.That(newCount, Is.EqualTo(0));
        }
    }
}
