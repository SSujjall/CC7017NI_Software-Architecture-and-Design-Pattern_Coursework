using EmailService.Models;

namespace EmailService.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailMessage message);
}