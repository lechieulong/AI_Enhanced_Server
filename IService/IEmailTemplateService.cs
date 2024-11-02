using Model.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IEmailTemplateService
    {
        EmailTemplate RemindMember(string recipientName, string reminder);
        EmailTemplate GetOtpEmailTemplate(string recipientName, string otpCode);
        EmailTemplate GetPasswordResetEmailTemplate(string recipientName, string resetLink);
        EmailTemplate AccountRegistrationEmail(string recipientName, string username);
        EmailTemplate AccountRegistrationGGEmail(string recipientName, string username, string password);
        EmailTemplate ResetPasswordEmail(string recipientName, string resetLink);
        EmailTemplate PasswordResetConfirmationEmail(string recipientName);
        EmailTemplate NotifyUnlockUserEmail(string recipientName, DateTime unlockDate);
        EmailTemplate NotifyAcceptTeacherRequestTemplate(string recipientName, string desciption);
        EmailTemplate NotifyRejectTeacherRequestTemplate(string recipientName, string desciption);
    }
}
