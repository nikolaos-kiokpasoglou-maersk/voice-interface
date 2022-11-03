using System;
using System.Threading.Tasks;
using Xunit;
using demo.Maersk;
using Alexa.NET.Response;
using FluentAssertions;
using Newtonsoft.Json;
using Alexa.NET.Request;

namespace demo.maersk.Test
{
    public class EntrypointTests
    {
        private readonly HttpClient _httpClient;
        private const string ExamplesPath = "Examples";
        private const string IntentRequestFile = "IntentRequest.json";

        public EntrypointTests()
        {
            _httpClient = new HttpClient();
        }

        [Fact]
        public async Task Run_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var requestUri = "http://localhost:7071/api/entrypoint";
            var convertedObj = GetObjectFromExample<SkillRequest>(IntentRequestFile);

            // Act
            var response = await _httpClient.PostAsJsonAsync(requestUri, convertedObj);
            var skillResponse = JsonConvert.DeserializeObject<SkillResponse>(response.Content.ToString());

            // Assert
            skillResponse.Should().NotBeNull();
        }

        private T GetObjectFromExample<T>(string filename)
        {
            var json = File.ReadAllText(Path.Combine(ExamplesPath, filename));
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
