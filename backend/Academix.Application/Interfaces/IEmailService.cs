using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
        Task SendWelcomeEmailAsync(string toEmail, string userName, string temporaryPassword);
    }
}
