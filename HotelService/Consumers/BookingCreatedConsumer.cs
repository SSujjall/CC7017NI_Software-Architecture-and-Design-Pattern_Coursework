using BuildingBlocks.Events;
using HotelService.Repositories.Interfaces;
using MassTransit;

namespace HotelService.Consumers;

public class BookingCreatedConsumer(IRoomRepository _roomRepo) : IConsumer<BookingCreatedEvent>
{
    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var msg = context.Message;

        var room = await _roomRepo.FindSingleByConditionAsync(
            x => x.HotelId == msg.HotelId && x.Id == msg.RoomId
        );

        if (room == null) return;

        room.IsAvailable = false;
        await _roomRepo.UpdateAsync(room);
        await _roomRepo.SaveChangesAsync();
    }
}
