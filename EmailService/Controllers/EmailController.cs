using EmailService.Models;
using EmailService.Models.DTOs;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EmailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController(IEmailService _emailService) : ControllerBase
    {
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDTO model)
        {
            #region Model Binding
            var emailModel = new EmailMessage(
                new [] {model.To}, 
                model.Subject,
                model.Content
            );
            #endregion
            var response = await _emailService.SendEmailAsync(emailModel);
            if (response == true)
            {
                return Ok("Email Sent");
            }
            return BadRequest("ERROR");
        }
    }
}
