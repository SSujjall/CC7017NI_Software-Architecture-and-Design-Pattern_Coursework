using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HotelService.Models;

public class Hotels
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    
    // Navigation property
    [JsonIgnore]
    public List<Rooms> Rooms { get; set; }
}