namespace CBT.SharedComponents.Blazor.Model.Identity
{
    public class ResetPasswordResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public string? RedirectRelativeUrl { get; set; }
    }
}
