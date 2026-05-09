namespace BuildingBlocks.Events;

public class PaymentSuccessEvent
{
    public int BookingId { get; set; }
}

public class PaymentFailedEvent
{
    public int BookingId { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
}