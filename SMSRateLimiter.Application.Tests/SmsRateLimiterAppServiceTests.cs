using Moq;
using SMSRateLimiter.Application.Contracts;
using SMSRateLimiter.Application.Implementations.Services;
using SMSRateLimiter.Domain.Contracts.Services;

namespace SMSRateLimiter.Application.Tests
{
    [TestFixture]
    public class SmsRateLimiterAppServiceTests
    {
        [Test]
        public async Task CanSendMessage_DelegatesToDomainService()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var mockDomainService = new Mock<ISmsRateLimiter>();
            var utcTimeNow = DateTime.UtcNow;
            mockDomainService.Setup(x => x.CanSendMessageAsync(1, phoneNumber, utcTimeNow)).ReturnsAsync(true);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.CanSendMessageAsync(1, phoneNumber, utcTimeNow);

            // Assert
            Assert.True(result);
            mockDomainService.Verify(x => x.CanSendMessageAsync(1, phoneNumber, utcTimeNow), Times.Once);
        }

        [Test]
        public async Task GetGlobalMessageCount_DelegatesToDomainService()
        {
            // Arrange
            var utcTimeNow = DateTime.UtcNow;
            var expectedCount = 5;
            var mockDomainService = new Mock<ISmsRateLimiter>();
            mockDomainService.Setup(x => x.GetGlobalMessageCountAsync(1, utcTimeNow)).ReturnsAsync(expectedCount);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.GetGlobalMessageCountAsync(1, utcTimeNow);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
            mockDomainService.Verify(x => x.GetGlobalMessageCountAsync(1, utcTimeNow), Times.Once);
        }

        [Test]
        public async Task GetMessageCountForNumber_DelegatesToDomainService()
        {
            // Arrange
            var utcTimeNow = DateTime.UtcNow;
            var phoneNumber = "+1234567890";
            var expectedCount = 3;
            var mockDomainService = new Mock<ISmsRateLimiter>();
            mockDomainService.Setup(x => x.GetMessageCountForNumberAsync(1, phoneNumber, utcTimeNow)).ReturnsAsync(expectedCount);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.GetMessageCountForNumberAsync(1, phoneNumber, utcTimeNow);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
            mockDomainService.Verify(x => x.GetMessageCountForNumberAsync(1, phoneNumber, utcTimeNow), Times.Once);
        }
    }
}