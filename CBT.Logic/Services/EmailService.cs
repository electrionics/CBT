using Microsoft.AspNetCore.Identity.UI.Services;

namespace CBT.Logic.Services
{
    public class EmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
