using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletService.Models.DTOs;
using WalletService.Services.Interfaces;

namespace WalletService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController(
        IWalletService _walletService
    ) : ControllerBase
    {
        [Authorize(Roles = "Superadmin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllWallets()
        {
            var result = await _walletService.GetAllWallets();
            return StatusCode((int)result.StatusCode, result);
        }
        
        [Authorize]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var result = await _walletService.GetWalletByUserId(id);
            return StatusCode((int)result.StatusCode, result);
        }
        
        [Authorize(Roles = "Superadmin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet(CreateWalletDTO dto)
        {
            var result = await _walletService.CreateWallet(dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}