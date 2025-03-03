using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.IntegrationTests.Controllers
{
    [TestFixture]
    public class MetricsEndpointTests : IntegrationTestBase
    {
        [Test]
        public async Task GetGlobalMetrics_ReturnsOk()
        {
            // Arrange
            var requestUri = "/api/v1/metrics/global";

            // Act
            var response = await Client.GetAsync(requestUri);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Global metrics endpoint should return 200 OK.");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(content, "The response content should not be empty.");
        }

        [Test]
        public async Task GetPerNumberMetrics_WithValidPhone_ReturnsOk()
        {
            // Arrange: Use a valid phone number (adjust as needed)
            var phoneNumber = "+1234567890";
            var requestUri = $"/api/v1/metrics/per-number?phoneNumber={phoneNumber}";

            // Act
            var response = await Client.GetAsync(requestUri);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Per-number metrics should return 200 OK for a valid phone number.");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(content, "The response content should not be empty.");
        }

        [Test]
        public async Task GetPerNumberMetrics_WithoutPhone_ReturnsBadRequest()
        {
            // Arrange: No phone number provided
            var requestUri = "/api/v1/metrics/per-number";

            // Act
            var response = await Client.GetAsync(requestUri);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "The per-number endpoint should return 400 Bad Request if no phone number is provided.");
        }
    }
}
