using System.Net;
using BookingService.Models.DTOs;
using BookingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StackExchange.Redis;
using UserService.Helpers;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(
        IBookingService _bookingService
    ) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return Unauthorized();

            var result = await _bookingService.CreateBooking(userId, dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("user-bookings")]
        public async Task<IActionResult> GetUserBookings()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ServiceException(
                        new() { { "Unauthorized", "User not authorized" } },
                        HttpStatusCode.Unauthorized
                    );
                }
            }
            var result = await _bookingService.GetUserBookings(userId);
            return Ok(result);
        }
        
        [Authorize(Roles = "Superadmin, Admin")]
        [HttpGet("all-bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookings();
            return Ok(result);
        }
        
        [Authorize(Roles = "Superadmin, Admin")]
        [HttpPost("clear-booking")]
        public async Task<IActionResult> ClearBooking(ClearBookingDTO dto)
        {
            var result = await _bookingService.ClearBooking(dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}