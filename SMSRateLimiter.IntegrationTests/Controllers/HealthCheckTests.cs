using Microsoft.AspNetCore.Mvc.Testing;
using SMSRateLimiter.Startup;
using System.Net;
using System.Text.Json;

namespace SMSRateLimiter.IntegrationTests.Controllers
{
    [TestFixture]
    public class HealthCheckTests : IntegrationTestBase
    {
        [Test]
        public async Task HealthEndpoint_ShouldReturn_HealthyStatus_WhenDependenciesAreOk()
        {
            // Arrange
            var requestUri = "/health";

            // Act
            var response = await Client.GetAsync(requestUri);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected a 200 OK status code");

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            // Check that the overall status is "Healthy"
            var status = root.GetProperty("status").GetString();
            Assert.That(status, Is.EqualTo("Healthy"), "The health status should be 'Healthy'");

        }
    }
}