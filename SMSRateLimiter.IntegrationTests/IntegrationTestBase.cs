using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SMSRateLimiter.Startup;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.IntegrationTests
{
    /// <summary>
    /// Base class for integration tests. It initializes a single HttpClient using WebApplicationFactory.
    /// </summary>
    public abstract class IntegrationTestBase
    {
        protected HttpClient Client { get; private set; }
        private WebApplicationFactory<Program> _factory;

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            // Initialize the in-memory test server
            _factory = new WebApplicationFactory<Program>();
            // Create a shared HttpClient for all tests
            Client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void BaseOneTimeTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
        }
    }
}
