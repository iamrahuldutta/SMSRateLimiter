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
        public async Task GetLogs_ReturnsOk()
        {
            // Arrange
            var requestUri = "/api/v1/metrics/get-logs?accountId=1";

            // Act
            var response = await Client.GetAsync(requestUri);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Get-logs endpoint should return 200 OK.");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(content, "The response content should not be empty.");
        }       
    }
}
