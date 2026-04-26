using BuildingBlocks.Events;
using MassTransit;
using WalletService.Services.Interfaces;

namespace WalletService.Consumers;

public class BookingCreatedConsumer(
    IWalletService _walletService,
    IPublishEndpoint _publishEndpoint
) : IConsumer<BookingCreatedEvent>
{
    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var msg = context.Message;

        var result = await _walletService.DeductBalance(msg.UserId, msg.Amount);

        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(msg.OwnerUserId))
                await _walletService.AddBalance(msg.OwnerUserId, msg.Amount);

            await _publishEndpoint.Publish(new PaymentSuccessEvent { BookingId = msg.BookingId });
        }
        else
        {
            await _publishEndpoint.Publish(new PaymentFailedEvent { BookingId = msg.BookingId });
        }
    }
}
