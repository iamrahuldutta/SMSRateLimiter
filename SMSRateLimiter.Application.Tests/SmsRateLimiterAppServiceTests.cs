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
            mockDomainService.Setup(x => x.CanSendMessage(phoneNumber)).ReturnsAsync(true);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.CanSendMessage(phoneNumber);

            // Assert
            Assert.True(result);
            mockDomainService.Verify(x => x.CanSendMessage(phoneNumber), Times.Once);
        }

        [Test]
        public async Task GetGlobalMessageCount_DelegatesToDomainService()
        {
            // Arrange
            var expectedCount = 5;
            var mockDomainService = new Mock<ISmsRateLimiter>();
            mockDomainService.Setup(x => x.GetGlobalMessageCount()).ReturnsAsync(expectedCount);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.GetGlobalMessageCount();

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
            mockDomainService.Verify(x => x.GetGlobalMessageCount(), Times.Once);
        }

        [Test]
        public async Task GetMessageCountForNumber_DelegatesToDomainService()
        {
            // Arrange
            var phoneNumber = "+1234567890";
            var expectedCount = 3;
            var mockDomainService = new Mock<ISmsRateLimiter>();
            mockDomainService.Setup(x => x.GetMessageCountForNumber(phoneNumber)).ReturnsAsync(expectedCount);
            ISmsRateLimiterAppService appService = new SmsRateLimiterAppService(mockDomainService.Object);

            // Act
            var result = await appService.GetMessageCountForNumber(phoneNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
            mockDomainService.Verify(x => x.GetMessageCountForNumber(phoneNumber), Times.Once);
        }
    }
}