using BookingService.Repositories.Interfaces;
using BuildingBlocks.Events;
using BuildingBlocks.Models.Enums;
using MassTransit;

namespace BookingService.Consumers;

public class PaymentResultConsumer :
    IConsumer<PaymentSuccessEvent>,
    IConsumer<PaymentFailedEvent>
{
    private readonly IBookingRepository _bookingRepo;

    public PaymentResultConsumer(IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
    }

    public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
    {
        var booking = await _bookingRepo.GetByIdAsync(context.Message.BookingId);
        if (booking == null) return;

        booking.Status = BookingStatus.Confirmed;
        await _bookingRepo.UpdateAsync(booking);
        await _bookingRepo.SaveChangesAsync();
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var booking = await _bookingRepo.GetByIdAsync(context.Message.BookingId);
        if (booking == null) return;

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepo.UpdateAsync(booking);
        await _bookingRepo.SaveChangesAsync();
    }
}
