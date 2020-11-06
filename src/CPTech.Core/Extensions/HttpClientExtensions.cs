using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cptech.Extension
{
    public static class HttpClientExtensions
    {
        public static async Task<string> PostAsync(this HttpClient client, string url, HttpContent content)
        {
            var body = await client.PostAsync(url, content);
            return await body.Content.ReadAsStringAsync();
        }

        public static async Task<TOut> PostJsonAsync<TIn, TOut>(this HttpClient client, string url, TIn data)
        {
            var response = await client.PostAsync(url, JsonContent.Create<TIn>(data));
            return await response.Content.ReadFromJsonAsync<TOut>();
        }
    }
}
