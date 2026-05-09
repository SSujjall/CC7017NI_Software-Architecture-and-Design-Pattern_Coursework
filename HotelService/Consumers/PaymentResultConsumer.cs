using BuildingBlocks.Events;
using HotelService.Repositories.Interfaces;
using MassTransit;

namespace HotelService.Consumers;

public class PaymentResultConsumer(IRoomRepository _roomRepo) : IConsumer<PaymentFailedEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var msg = context.Message;

        var room = await _roomRepo.FindSingleByConditionAsync(
            x => x.HotelId == msg.HotelId && x.Id == msg.RoomId
        );

        if (room == null) return;

        room.IsAvailable = true;
        await _roomRepo.UpdateAsync(room);
        await _roomRepo.SaveChangesAsync();
    }
}