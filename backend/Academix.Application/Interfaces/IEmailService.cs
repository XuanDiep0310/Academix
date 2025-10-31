using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendWelcomeEmailAsync(string email, string displayName);
    }
}
