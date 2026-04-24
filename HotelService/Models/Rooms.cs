using System.ComponentModel.DataAnnotations.Schema;

namespace HotelService.Models;

public class Rooms
{
    public int Id { get; set; }
    public int HotelId { get; set; }

    public string RoomNumber { get; set; }
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    [ForeignKey(nameof(HotelId))]
    public Hotels Hotel { get; set; }
}