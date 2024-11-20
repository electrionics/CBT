using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace CBT.SharedComponents.Blazor.Common
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResult<TResult>> PostResultAsJsonAsync<TResult, TBody>(this HttpClient client, string url, TBody body)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url);
            message = message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            message.Content = JsonContent.Create(body);

            var result = new HttpResult<TResult>();

            try
            {
                var response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();
                var jsonStr = await response.Content.ReadAsStringAsync();

                result.Headers = response.Headers;
                result.Value = JsonConvert.DeserializeObject<TResult>(jsonStr);
                result.Succeeded = true;
            }
            catch (HttpRequestException)
            {
                result.Succeeded = false;
            }

            return result;
        }

        public static Task<HttpResult<TResult>> PostResultAsJsonAsync<TResult, TBody>(this HttpClient client, Uri url, TBody body)
        {
            return PostResultAsJsonAsync<TResult, TBody>(client, url.AbsoluteUri, body);
        }

        public static async Task<HttpResult<TResult>> GetResultAsJsonAsync<TResult>(this HttpClient client, string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            message = message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var result = new HttpResult<TResult>();

            try
            {
                var response = await client.SendAsync(message);
                response.EnsureSuccessStatusCode();
                var jsonStr = await response.Content.ReadAsStringAsync();

                result.Headers = response.Headers;
                result.Value = JsonConvert.DeserializeObject<TResult>(jsonStr);
                result.Succeeded = true;
            }
            catch (HttpRequestException)
            {
                result.Succeeded = false;
            }

            return result;
        }

        public static Task<HttpResult<TResult>> GetResultAsJsonAsync<TResult>(this HttpClient client, Uri url)
        {
            return GetResultAsJsonAsync<TResult>(client, url.AbsoluteUri);
        }
    }
}
