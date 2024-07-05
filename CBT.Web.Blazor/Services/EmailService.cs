using Microsoft.AspNetCore.Identity.UI.Services;

namespace CBT.Web.Blazor.Services
{
    public class EmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
