using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IService;
using Model.Email;

namespace Service
{
    using System.Net;
    using System.Net.Mail;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;
    using Entity.Data;
    using Entity;

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly AppDbContext _dbContext;

        public EmailSender(IOptions<EmailSettings> emailSettings, AppDbContext dbContext)
        {
            _emailSettings = emailSettings.Value;
            _dbContext = dbContext;
        }

        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            var emailLog = new EmailLog
            {
                Recipient = recipient,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now,
                Success = false
            };

            try
            {
                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(recipient);

                await smtpClient.SendMailAsync(mailMessage);
                emailLog.Success = true;
            }
            catch (Exception ex)
            {
                emailLog.ErrorMessage = ex.Message;
                throw new InvalidOperationException($"Error sending email: {ex.Message}");
            }
            finally
            {
                _dbContext.EmailLogs.Add(emailLog);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
