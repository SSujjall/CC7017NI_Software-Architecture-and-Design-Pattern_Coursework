using BuildingBlocks.Events;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using UserService.Models;

namespace UserService.Helpers;

public class StartupSeeder
{
    private readonly IServiceProvider _serviceProvider;
    
    public StartupSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var user = await userManager.FindByEmailAsync("superadmin@cw.com");

        if (user == null) return;

        // Publish event so WalletService creates wallet
        await publishEndpoint.Publish(new UserRegisteredEvent
        {
            UserId = user.Id,
            // Email = user.Email,
            // Username = user.UserName
        });
    }
}