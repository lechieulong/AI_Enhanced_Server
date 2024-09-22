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
    }
}
