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
                    <blockquote style='font-style: italic; border-left: 4px solid #e74c3c; padding-left: 10px; color: #fcd722;'>
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

        public EmailTemplate AccountRegistrationEmail(string recipientName, string username)
        {
            return new EmailTemplate
            {
                Subject = "Welcome to Our Platform!",
                Body = $@"
                <h1>Registration Successful!</h1>
                <p>Dear {recipientName},</p>
                <p>Thank you for registering an account with us.</p>
                <p>Your username is: <strong>{username}</strong></p>
                <p>We are excited to have you on board and hope you enjoy your experience with our platform!</p>
                <p>If you have any questions, feel free to reach out to our support team.</p>
                <br/>
                <p>Best Regards,</p>
                <p>The Platform Team</p>
            "
            };
        }

        public EmailTemplate AccountRegistrationGGEmail(string recipientName, string username, string password)
        {
            return new EmailTemplate
            {
                Subject = "Welcome to Our Platform!",
                Body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h1 style='color: #4CAF50;'>Registration Successful!</h1>
                    <p>Dear <strong>{recipientName}</strong>,</p>
                    <p>Thank you for registering an account with us.</p>
                    <p>Your username is: <strong style='color: #007BFF;'>{username}</strong></p>
                    <p>This is your Password: <strong style='color: #FF5722;'>{password}</strong></p>
                    <p style='font-weight: bold; color: #FF5722;'>For security purposes, please change your password after registering an account!</p>
                    <p>We are excited to have you on board and hope you enjoy your experience with our platform!</p>
                    <p>If you have any questions, feel free to reach out to our support team.</p>
                    <br/>
                    <p>Best Regards,</p>
                    <p>The Platform Team</p>
                </div>
                "
            };
        }
        public EmailTemplate ResetPasswordEmail(string recipientName, string resetLink)
        {
            return new EmailTemplate
            {
                Subject = "Password Reset Request",
                Body = $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; background: #f4f4f4; padding: 20px; }}
                            .container {{ max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                            a {{ display: inline-block; padding: 10px 20px; background: #007BFF; color: #ffffff; text-decoration: none; border-radius: 5px; }}
                            a:hover {{ background: #0056b3; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h1>Hello {recipientName},</h1>
                            <p>Click the link below to reset your password:</p>
                            <p><a href='{resetLink}'>Reset your password</a></p>
                            <p><strong>Note:</strong> This link is valid for 15 minutes.</p>
                            <p>If you did not request this, you can safely ignore this email.</p>
                        </div>
                    </body>
                </html>"
            };
        }


        public EmailTemplate PasswordResetConfirmationEmail(string recipientName)
        {
            return new EmailTemplate
            {
                Subject = "Password Reset Confirmation",
                Body = $@"
                    <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; background: #f4f4f4; padding: 20px; }}
                                .container {{ max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                            </style>
                        </head>
                        <body>
                            <div>
                                <p>Dear {recipientName},</p>
                                <p>Your password has been successfully reset.</p>
                            </div>
                        </body>
                    </html>"
            };
        }


    }
}
