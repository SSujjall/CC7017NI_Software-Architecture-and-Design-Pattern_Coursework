using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;
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
        [HttpGet("get-by-id/{walletId}")]
        public async Task<IActionResult> GetWalletById(int walletId)
        {
            var userId = GetUserId();
            var result = await _walletService.GetWalletById(userId, walletId);
            return Ok(result);
        }

        [Authorize(Roles = "Superadmin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet(CreateWalletDTO dto)
        {
            var result = await _walletService.CreateWallet(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("load-money")]
        public async Task<IActionResult> LoadMoney(LoadMoneyDTO dto)
        {
            var userId = GetUserId();
            var result = await _walletService.AddMoneyInWallet(userId, dto);
            return StatusCode((int)result.StatusCode, result);
        }

        private string GetUserId()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new ServiceException(
                    new() { { "Unauthorized", "User not authorized" } },
                    HttpStatusCode.Unauthorized
                );
            }
            return userId;
        }
    }
}