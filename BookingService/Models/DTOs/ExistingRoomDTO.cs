namespace BookingService.Models.DTOs;

public class ExistingRoomDTO
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string RoomNumber { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}