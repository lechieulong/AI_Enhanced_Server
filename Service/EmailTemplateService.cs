using IService;
using Model.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public EmailTemplate GetOtpEmailTemplate(string recipientName, string otpCode)
        {
            return new EmailTemplate
            {
                Subject = "Your OTP Code",
                Body = $@"
                    <h1>Hi {recipientName},</h1>
                    <h2>Your OTP Code is:</h2>
                    <strong>{otpCode}</strong>
                    <p>Please use this code to complete your action.</p>
                    <p>Best regards,<br>Your Team</p>
                "
            };
        }

        public EmailTemplate GetPasswordResetEmailTemplate(string recipientName, string resetLink)
        {
            return new EmailTemplate
            {
                Subject = "Password Reset Request",
                Body = $"Hi {recipientName},\n\nClick the link below to reset your password:\n{resetLink}"
            };
        }

        public EmailTemplate RemindMember(string recipientName, string reminder)
        {
            return new EmailTemplate
            {
                Subject = "Hỡi những con người lười biếng",
                Body = $@"
            <h1 style='color: #e74c3c;'>Chào {recipientName}</h1>
            <h2 style='font-weight: bold;'>Đã đến lúc hành động rồi!</h2>
            <p>
                Chúng tôi nhận thấy bạn chưa hoàn thành các nhiệm vụ của mình. Đây là lời nhắc nhở <b>khẩn cấp</b> để bạn bắt tay vào làm ngay:
            </p>
            <blockquote style='font-style: italic; border-left: 4px solid #e74c3c; padding-left: 10px;'>
                {reminder}
            </blockquote>
            <p style='font-weight: bold;'>
                Đừng chần chừ nữa, <b>hành động ngay</b> nếu bạn không muốn bỏ lỡ cơ hội này!
            </p>
            <h2 style='color: #e74c3c;'>Bạn sẽ làm được!</h2>
            <p>
                Chúng tôi tin rằng bạn sẽ hoàn thành tốt nhiệm vụ. Nếu bạn cần sự giúp đỡ, đừng ngần ngại liên hệ với chúng tôi.
            </p>
            <p style='font-style: italic;'>Chúc bạn thành công!</p>
        "
            };
        }

    }
}
