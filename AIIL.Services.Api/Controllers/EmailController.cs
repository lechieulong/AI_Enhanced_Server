using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Email;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailSenderService _emailSenderService;

        public EmailController(IEmailSender emailSender, IEmailSenderService emailSenderService)
        { 
            _emailSender = emailSender;
            _emailSenderService = emailSenderService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            try
            {
                await _emailSender.SendEmailAsync(request.Recipient, request.Subject, request.Body);
                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }

        [HttpPost("remind")]
        public async Task<IActionResult> SendEmailRemindMemberAsync([FromBody] RemindMemberRequestDto request)
        {
            try
            {
                await _emailSenderService.SendEmailRemindMemberAsync(request.RecipientEmail, request.ReminderMessage);
                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
    }

}
