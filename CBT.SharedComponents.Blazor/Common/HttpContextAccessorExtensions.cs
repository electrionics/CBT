using Microsoft.AspNetCore.Http;

namespace CBT.SharedComponents.Blazor.Common
{
    public static class HttpContextAccessorExtensions
    {
        public static string ToAbsoluteUri(this IHttpContextAccessor accessor, string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (accessor.HttpContext == null)
                return relativeUrl;

            relativeUrl = relativeUrl.TrimStart('~').TrimStart('/');

            var baseUrl = $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}"; 
            return new Uri(new Uri(baseUrl), relativeUrl).AbsoluteUri;
        }
    }
}
