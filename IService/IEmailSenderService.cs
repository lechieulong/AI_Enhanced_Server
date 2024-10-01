using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IEmailSenderService
    {
        Task SendEmailRemindMemberAsync(string recipientEmail, string reminder);
        Task SendRegistrationSuccessEmail(string recipientEmail, string recipientName, string username);
        Task SendRegistrationGGSuccessEmail(string recipientEmail, string recipientName, string username, string password);
        Task SendResetPasswordRequestEmail(string email, string userName, string resetLink);
        Task SendResetPasswordConfirmationEmail(string email, string userName);
    }
}
