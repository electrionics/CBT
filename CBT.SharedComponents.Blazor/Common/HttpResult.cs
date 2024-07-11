using System.Net.Http.Headers;

namespace CBT.SharedComponents.Blazor.Common
{
    public class HttpResult<TResult>
    {
        public TResult? Value { get; set; }

        public HttpResponseHeaders Headers { get; set; }
    }
}
