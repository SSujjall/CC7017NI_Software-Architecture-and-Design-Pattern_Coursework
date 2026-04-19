using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WalletService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Wallet");
        }

        [HttpPost("test-post")]
        public IActionResult Post()
        {
            return Ok("Wallet post working");
        }
    }
}
