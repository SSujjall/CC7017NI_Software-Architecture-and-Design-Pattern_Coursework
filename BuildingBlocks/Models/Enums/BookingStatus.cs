using System.Text.Json.Serialization;

namespace BuildingBlocks.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    CompletedAndReturned = 3
}