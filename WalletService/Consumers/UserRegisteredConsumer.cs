using BuildingBlocks.Events;
using MassTransit;
using WalletService.Models.DTOs;
using WalletService.Services.Interfaces;

namespace WalletService.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IWalletService _walletService;
    
    public UserRegisteredConsumer(IWalletService walletService)
    {
        _walletService = walletService;
    }
    
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var walletDto = new CreateWalletDTO
        {
            UserId = context.Message.UserId
        };
        var response = await _walletService.CreateWallet(walletDto);
        Console.WriteLine(response);
    }
}