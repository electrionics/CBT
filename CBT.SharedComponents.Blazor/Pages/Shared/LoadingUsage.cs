namespace CBT.SharedComponents.Blazor.Pages.Shared
{
    public class LoadingUsage(Loading loading) : IAsyncDisposable
    {
        private readonly Loading loading = loading;

        public async Task<LoadingUsage> StartAsync()
        {
            await loading.StartAsync();

            return this;
        }

        public async ValueTask DisposeAsync()
        {
            await loading.StopAsync();
        }
    }
}
