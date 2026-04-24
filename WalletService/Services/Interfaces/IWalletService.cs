using BuildingBlocks.Models;
using WalletService.Models;
using WalletService.Models.DTOs;

namespace WalletService.Services.Interfaces;

public interface IWalletService
{
    Task<ApiResponse<IEnumerable<Wallets>>> GetAllWallets();
    // Task<ApiResponse<Wallets>> GetWalletByWalletId(int id);
    Task<ApiResponse<Wallets>> GetWalletByUserId(int walletId);
    Task<ApiResponse<Wallets>> CreateWallet(CreateWalletDTO dto);
}