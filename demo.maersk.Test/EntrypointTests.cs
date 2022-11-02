using System;
using System.Threading.Tasks;
using Xunit;
using demo.Maersk;
using Alexa.NET.Response;
using FluentAssertions;
using Newtonsoft.Json;

namespace demo.maersk.Test
{
    public class EntrypointTests
    {
        private readonly HttpClient _httpClient;

        public EntrypointTests()
        {
            _httpClient = new HttpClient();
        }

        [Fact]
        public async Task Run_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var requestUri = "http://localhost:7071/api/Entrypoint";

            // Act
            var response = await _httpClient.GetStringAsync(requestUri).ConfigureAwait(false);
            var skillResponse = JsonConvert.DeserializeObject<SkillResponse>(response);

            // Assert
            skillResponse.Should().NotBeNull();
        }
    }
}
