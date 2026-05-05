using BuildingBlocks.Events;
using HotelService.Repositories.Interfaces;
using MassTransit;

namespace HotelService.Consumers;

public class BookingClearConsumer(IRoomRepository _roomRepo) : IConsumer<BookingClearEvent>
{
    public async Task Consume(ConsumeContext<BookingClearEvent> context)
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
