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
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                    <h1 style='color: #333;'>Registration Successful!</h1>
                    <p>Dear {recipientName},</p>
                    <p>Thank you for registering an account with us.</p>
                    <p>Your username is: <strong style='color: #007BFF;>{username}</strong></p>
                    <p style='color: #FF5722;><em>Please use this Username to login to the system.</em></p>
                    <p>We are excited to have you on board and hope you enjoy your experience with our platform!</p>
                    <p>If you have any questions, feel free to reach out to our support team.</p>
                    <br/>
                    <p>Best Regards,</p>
                    <p>The Platform Team</p>
                </div>
                "
            };
        }


        public EmailTemplate AccountRegistrationGGEmail(string recipientName, string username, string password)
        {
            return new EmailTemplate
            {
                Subject = "Welcome to Our Platform!",
                Body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
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
                            body {{ font-family: Arial, sans-serif; background-color: #f9fafb; color: #333; }}
                            .container {{ max-width: 600px; margin: 20px auto; background: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
                            h1 {{ font-size: 24px; color: #1a202c; }}
                            p {{ font-size: 16px; line-height: 1.6; }}
                            .btn {{ display: inline-block; margin-top: 20px; padding: 10px 20px; background: #1d72b8; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                            .footer {{ font-size: 12px; color: #888; text-align: center; margin-top: 30px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h1>Password Reset Confirmation</h1>
                            <p>Dear {recipientName},</p>
                            <p>Your password has been successfully reset. If you did not perform this action, please contact support.</p>
                            <a href='http://localhost:5173/' class='btn'>Login to Your Account</a>
                            <div class='footer'>
                                <p>&copy; 2024 Nguyen Van Sy Dep Trai. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                </html>"
            };
        }

        public EmailTemplate NotifyUnlockUserEmail(string recipientName, DateTime unlockDate)
        {
            return new EmailTemplate
            {
                Subject = "Account Unlocked Notification",
                Body = $@"
                    <html>
                    <body>
                        <p>Dear {recipientName},</p>
                        <p>We are pleased to inform you that your account has been successfully unlocked.</p>
                        <p>Your account was unlocked on: <strong>{unlockDate.ToString("f")}</strong>.</p>
                        <p>If you have any questions or need further assistance, feel free to contact us.</p>
                        <p>Thank you!</p>
                        <p>Best regards,<br/>
                        AIILs support Team</p>
                    </body>
                    </html>
                    "
            };
        }

        public EmailTemplate NotifyAcceptTeacherRequestTemplate(string recipientName, string description)
        {
            return new EmailTemplate
            {
                Subject = "Request Approved to Become a Teacher",
                Body = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2>Congratulations, {recipientName}!</h2>
                            <p>We are pleased to inform you that your request to become a teacher on our platform has been approved.</p>
                            <p><strong>Description:</strong> {description}</p>
                            <p>You may now access additional features available to teachers. We are excited to see your valuable contributions to our learning community.</p>
                            <p>If you have any questions, please feel free to reach out to our support team.</p>
                            <br />
                            <p>Best regards,<br />The AI-Enhanced IELTS Prep Team</p>
                        </body>
                    </html>"
            };
        }

        public EmailTemplate NotifyRejectTeacherRequestTemplate(string recipientName, string description)
        {
            return new EmailTemplate
            {
                Subject = "Request Rejected to Become a Teacher",
                Body = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2>Dear {recipientName},</h2>
                            <p>We regret to inform you that your request to become a teacher on our platform has been rejected.</p>
                            <p><strong>Description:</strong> {description}</p>
                            <p>We appreciate your interest in joining our teaching team. If you have any questions regarding this decision or would like feedback, please feel free to reach out to our support team.</p>
                            <br />
                            <p>Thank you for your understanding,<br />The AI-Enhanced IELTS Prep Team</p>
                        </body>
                    </html>"
            };
        }

    }
}
