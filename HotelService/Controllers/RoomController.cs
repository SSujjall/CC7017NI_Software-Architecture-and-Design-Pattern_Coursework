using HotelService.Models.DTOs;
using HotelService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(
        IRoomService _roomService
    ) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-by-hotel-room/{hotelId}/{roomId}")]
        public async Task<IActionResult> GetRoomByHotelAndId(int hotelId, int roomId)
        {
            var result = await _roomService.GetRoomByHotelAndId(hotelId, roomId);
            return StatusCode((int)result.StatusCode, result);
        }
        
        [Authorize("Superadmin, Admin")]
        [HttpPost("create-room")]
        public async Task<IActionResult> CreateRoomForHotel(CreateRoomDTO model)
        {
            var result = await _roomService.AddRoomForHotel(model);
            return Ok(result);
        }
    }
}