using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
namespace AuthenticationService

{
    public class NoOpEmailSender:IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a no-op email sender, so it does nothing.
            return Task.CompletedTask;
        }
    }
}
