using Microsoft.JSInterop;

namespace CBT.SharedComponents.Blazor
{
    public class JsInteropBase(IJSRuntime jsRuntime)
    {
        protected readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/CBT.SharedComponents.Blazor/jsInterop.js").AsTask());

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}