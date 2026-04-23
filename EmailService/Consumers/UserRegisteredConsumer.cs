using BuildingBlocks.Events;
using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;

namespace EmailService.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;
    
    public UserRegisteredConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
        var emailMessage = new EmailMessage(
            new[] {message.Email}, 
            "Please confirm your email", 
            $"Hello {message.Username}, Please confirm your account with the following link: \n {message.EmailConfirmationLink}"
        );
        await _emailService.SendEmailAsync(emailMessage);
    }
}