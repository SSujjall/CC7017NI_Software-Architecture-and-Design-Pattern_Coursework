namespace BookingService.Models.DTOs;

public class CreateBookingDTO
{
    public int HotelId { get; set; }
    public int RoomId { get; set; }
}

public class ClearBookingDTO
{
    public int HotelId { get; set; }
    public int RoomId { get; set; }
}