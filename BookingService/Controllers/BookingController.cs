using BookingService.Models.DTOs;
using BookingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(
        IBookingService _bookingService
    ) : ControllerBase
    {
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return Unauthorized();

            var result = await _bookingService.CreateBooking(userId, dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}