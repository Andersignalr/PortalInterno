using Microsoft.AspNetCore.Identity.UI.Services;

namespace PortalInterno.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Desenvolvimento: não envia nada
            return Task.CompletedTask;
        }
    }
}
