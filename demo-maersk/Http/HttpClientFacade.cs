using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace demo.maersk.Http
{
    public class HttpClientFacade : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public HttpClientFacade(string baseAddress, string token)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress),
                Timeout = TimeSpan.FromMinutes(10)
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<HttpResponseMessage> GetAsync(string resource)
            => await _httpClient.GetAsync(resource);

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string resource, T data)
            => await _httpClient.PostAsJsonAsync(resource, data);
    }
}
