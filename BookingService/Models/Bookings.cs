using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Models.Enums;

namespace BookingService.Models;

public class Bookings
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public decimal Amount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}