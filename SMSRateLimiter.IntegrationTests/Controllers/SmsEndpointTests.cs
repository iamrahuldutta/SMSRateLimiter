using SMSRateLimiter.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.IntegrationTests.Controllers
{
    [TestFixture]
    public class SmsEndpointTests : IntegrationTestBase
    {
        [Test]
        public async Task SendSms_WithValidData_ReturnsOk()
        {
            // Arrange
            var smsRequest = new SmsRequestDto
            {
                PhoneNumber = "+1234567890"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/sms/send", smsRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task SendSms_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange: Invalid request (empty phone number)
            var smsRequest = new SmsRequestDto
            {
                PhoneNumber = ""
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/sms/send", smsRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
