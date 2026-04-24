using HotelService.Models.DTOs;
using HotelService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController(
        IHotelService _hotelService
    ) : ControllerBase
    {
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllHotels()
        {
            var result = await _hotelService.GetAllHotels();
            return StatusCode((int)result.StatusCode, result);
        }
        
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetAllHotels(int id)
        {
            var result = await _hotelService.GetHotelById(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize(Roles = "Superadmin")] // only superadmin gets to add hotel, this makes it so that there is only 1 hotel owner
        [HttpPost("add-hotel")]
        public async Task<IActionResult> AddHotel(AddHotelDTO model)
        {
            var result = await _hotelService.AddHotel(model);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}