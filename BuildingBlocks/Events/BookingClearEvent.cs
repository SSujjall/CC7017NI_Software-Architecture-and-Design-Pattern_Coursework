namespace BuildingBlocks.Events;

public class BookingClearEvent
{
    public int HotelId { get; set; }
    public int RoomId { get; set; }
}