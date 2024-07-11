using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace CBT.SharedComponents.Blazor.Common
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResult<TResult>> PostResultAsJsonAsync<TResult, TBody>(this HttpClient client, string url, TBody body, bool ensureSuccess = true)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url);
            message = message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            message.Content = JsonContent.Create(body);

            var response = await client.SendAsync(message);
            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }

            var jsonStr = await response.Content.ReadAsStringAsync();
            var result = new HttpResult<TResult>()
            {
                Value = JsonConvert.DeserializeObject<TResult>(jsonStr),
                Headers = response.Headers
            };

            return result;
        }

        public static Task<HttpResult<TResult>> PostResultAsJsonAsync<TResult, TBody>(this HttpClient client, Uri url, TBody body, bool ensureSuccess = true)
        {
            return PostResultAsJsonAsync<TResult, TBody>(client, url.AbsoluteUri, body, ensureSuccess);
        }
    }
}
