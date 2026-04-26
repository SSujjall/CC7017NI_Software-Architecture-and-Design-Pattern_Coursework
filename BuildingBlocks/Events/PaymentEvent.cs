namespace BuildingBlocks.Events;

public class PaymentSuccessEvent
{
    public int BookingId { get; set; }
}

public class PaymentFailedEvent
{
    public int BookingId { get; set; }
}