using EmailService.Models;
using EmailService.Models.Configs;
using EmailService.Services.Interfaces;
using MimeKit;

namespace EmailService.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailConfig emailConfig, ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig;
        _logger = logger;
    }
    
    public async Task<bool> SendEmailAsync(EmailMessage message)
    {
        var emailMessage = CreateEmailMessage(message);
        return await Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(EmailMessage message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("SujalCw", _emailConfig.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

        return emailMessage;
    }

    private async Task<bool> Send(MimeMessage mailMessage)
    {
        using var client = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            await client.SendAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Recipients}", string.Join(",", mailMessage.To));
            return false;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
            //client.Dispose(); // no need for dispose as we are using the 'using' keyword for 'client'
        }
    }
}