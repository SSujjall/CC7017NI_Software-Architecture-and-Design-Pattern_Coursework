namespace BuildingBlocks.Events;

public class BookingCreatedEvent
{
    public int BookingId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    
    public string OwnerUserId { get; set; }
}