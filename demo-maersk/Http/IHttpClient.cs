using System.Net.Http;
using System.Threading.Tasks;

namespace demo.maersk.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string resource);
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string resource, T data);
    }
}