using System.Net;
using BuildingBlocks.Models;
using WalletService.Models;
using WalletService.Models.DTOs;
using WalletService.Repositories.Interfaces;
using WalletService.Services.Interfaces;

namespace WalletService.Services;

public class WalletService(
    IWalletRepository _walletRepo
) : IWalletService
{
    public async Task<ApiResponse<IEnumerable<Wallets>>> GetAllWallets()
    {
        var wallets = await _walletRepo.GetAllAsync();
        return ApiResponse<IEnumerable<Wallets>>.Success(wallets, "Wallets fetched");
    }

    public async Task<ApiResponse<Wallets>> GetWalletByUserId(int walletId)
    {
        var wallet = await _walletRepo.GetByIdAsync(walletId);
        if (wallet == null)
        {
            return ApiResponse<Wallets>.Success(null, "No wallets found", HttpStatusCode.NoContent);
        }
        return ApiResponse<Wallets>.Success(wallet, "Wallets fetched");
    }

    public async Task<ApiResponse<Wallets>> CreateWallet(CreateWalletDTO dto)
    {
        var walletModel = new Wallets
        {
            UserId = dto.UserId,
            Balance = dto.Balance,
        };
        var createWalletResult = await _walletRepo.AddAsync(walletModel);
        await _walletRepo.SaveChangesAsync();
        return ApiResponse<Wallets>.Success(createWalletResult, "Wallet created", HttpStatusCode.Created);
    }
}