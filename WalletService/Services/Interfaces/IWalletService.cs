using BuildingBlocks.Models;
using WalletService.Models;
using WalletService.Models.DTOs;

namespace WalletService.Services.Interfaces;

public interface IWalletService
{
    Task<ApiResponse<IEnumerable<Wallets>>> GetAllWallets();
    // Task<ApiResponse<Wallets>> GetWalletByWalletId(int id);
    Task<ApiResponse<Wallets>> GetWalletById(string userId, int walletId);
    Task<ApiResponse<Wallets>> GetUserWallet(string userId);
    Task<ApiResponse<Wallets>> CreateWallet(CreateWalletDTO dto);
    Task<ApiResponse<Wallets>> AddMoneyInWallet(string userId, LoadMoneyDTO dto);
    Task<ApiResponse<Wallets>> DeductBalance(string userId, decimal amount);
    Task<ApiResponse<Wallets>> AddBalance(string userId, decimal amount);
}