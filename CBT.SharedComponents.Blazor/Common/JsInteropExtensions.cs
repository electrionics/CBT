using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.SharedComponents.Blazor.Common
{
    public static class JsInteropExtensions
    {
        public static async Task WriteAuth<T>(this JsInterop jsInterop, HttpResult<T> result)
        {
            var cookies = result.Headers.Where(x => x.Key == "Set-Cookie");
            var authCookie = cookies.FirstOrDefault().Value?.FirstOrDefault();
            authCookie = authCookie?.Replace(" httponly", "");

            if (authCookie != null)
            {
                await jsInterop.WriteAuthCookie(authCookie);
            }
        }
    }
}
