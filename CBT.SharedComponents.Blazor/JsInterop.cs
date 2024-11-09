using Microsoft.JSInterop;

namespace CBT.SharedComponents.Blazor
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class JsInterop(
        IJSRuntime jsRuntime) : JsInteropBase(jsRuntime), IAsyncDisposable
    {
        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask WriteAuthCookie(string cookie)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("writeAuthCookie", cookie);
        }

        public async ValueTask<string> ReadAuthCookie()
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("readAuthCookie");
        }
    }
}
