using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailSenderService(IEmailSender emailSender, IEmailTemplateService emailTemplateService)
        {
            _emailSender = emailSender;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendOtpEmailAsync(string recipientEmail, string recipientName, string otpCode)
        {
            var otpEmail = _emailTemplateService.GetOtpEmailTemplate(recipientName, otpCode);
            await _emailSender.SendEmailAsync(recipientEmail, otpEmail.Subject, otpEmail.Body);
        }

        public async Task SendPasswordResetEmailAsync(string recipientEmail, string recipientName, string resetLink)
        {
            var resetEmail = _emailTemplateService.GetPasswordResetEmailTemplate(recipientName, resetLink);
            await _emailSender.SendEmailAsync(recipientEmail, resetEmail.Subject, resetEmail.Body);
        }

        public async Task SendEmailRemindMemberAsync(string recipientEmail, string reminder)
        {
            var remindEmail = _emailTemplateService.RemindMember(recipientEmail, reminder);
            await _emailSender.SendEmailAsync(recipientEmail, remindEmail.Subject, remindEmail.Body);
        }

        public async Task SendRegistrationSuccessEmail(string recipientEmail, string recipientName, string username)
        {
            var emailTemplate = _emailTemplateService.AccountRegistrationEmail(recipientName, username);
            await _emailSender.SendEmailAsync(recipientEmail, emailTemplate.Subject, emailTemplate.Body);
        }

        public async Task SendRegistrationGGSuccessEmail(string recipientEmail, string recipientName, string username, string password)
        {
            var emailTemplate = _emailTemplateService.AccountRegistrationGGEmail(recipientName, username, password);
            await _emailSender.SendEmailAsync(recipientEmail, emailTemplate.Subject, emailTemplate.Body);
        }
    }

}
