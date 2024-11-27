namespace CBT.SharedComponents.Blazor.Model
{
    public class CommonResult
    {
        public bool Succeeded { get; set; }

        public string? RedirectUrl { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class CommonResult<TData> : CommonResult
    {
        public TData? Data { get; set; }
    }
}
