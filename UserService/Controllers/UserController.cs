using BuildingBlocks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Services.Interfaces;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [Authorize]
        [HttpGet("superadmin-id")]
        public async Task<IActionResult> GetSuperAdminId()
        {
            var id = await _userService.GetSuperAdminId();
            if (id == null)
                return NotFound(ApiResponse<string>.Failed(
                    new Dictionary<string, string> { { "Superadmin", "Superadmin not found" } },
                    "Not found",
                    System.Net.HttpStatusCode.NotFound
                ));

            return Ok(ApiResponse<string>.Success(id, "Superadmin id fetched"));
        }
    }
}
