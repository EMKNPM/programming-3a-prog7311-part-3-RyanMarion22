using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Xunit;

namespace UnitTesting
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7205/");
        }

        [Fact]
        public async Task GetClients_ReturnsSuccess()
        {
            var response = await _client.GetAsync("api/clients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_ReturnsSuccess()
        {
            var response = await _client.GetAsync("api/contracts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsSuccess()
        {
            var response = await _client.GetAsync("api/servicerequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContract_InvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("api/contracts/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_ReturnsNotNullContent()
        {
            var response = await _client.GetAsync("api/contracts");
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task CreateClient_ThenGet_ReturnsCreatedClient()
        {
            var newClient = new
            {
                Name = "Test Client",
                ContactDetails = "test@test.com",
                Region = "Test Region"
            };

            var createResponse = await _client.PostAsJsonAsync("api/clients", newClient);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var getResponse = await _client.GetAsync("api/clients");
            var content = await getResponse.Content.ReadAsStringAsync();
            Assert.Contains("Test Client", content);
        }

        [Fact]
        public async Task CreateContract_ReturnsCreated()
        {
            var newContract = new
            {
                ClientId = 1,
                StartDate = "2026-01-01",
                EndDate = "2026-12-31",
                ServiceLevel = "Standard",
                Status = "Active"
            };

            var response = await _client.PostAsJsonAsync("api/contracts", newContract);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}