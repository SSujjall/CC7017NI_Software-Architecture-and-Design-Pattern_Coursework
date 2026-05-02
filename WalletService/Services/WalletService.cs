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

    public async Task<ApiResponse<Wallets>> GetWalletById(string userId, int walletId)
    {
        var wallet = await _walletRepo.FindSingleByConditionAsync(
            x => x.Id == walletId && x.UserId == userId
        );
        if (wallet == null)
        {
            return ApiResponse<Wallets>.Success(null, "No wallets found", HttpStatusCode.NoContent);
        }

        return ApiResponse<Wallets>.Success(wallet, "Wallets fetched");
    }

    public async Task<ApiResponse<Wallets>> GetUserWallet(string userId)
    {
        var userWallet = await _walletRepo.FindSingleByConditionAsync(x => x.UserId == userId);
        if (userWallet == null)
        {
            return ApiResponse<Wallets>.Success(null, "No wallet found for user", HttpStatusCode.NoContent);
        }

        return ApiResponse<Wallets>.Success(userWallet, "User's Wallet fetched");
    }

    public async Task<ApiResponse<Wallets>> CreateWallet(CreateWalletDTO dto)
    {
        var walletModel = new Wallets
        {
            UserId = dto.UserId,
            Balance = dto.Balance,
        };
        var existingWallet = await _walletRepo.FindSingleByConditionAsync(x => x.UserId == walletModel.UserId
        );
        if (existingWallet != null)
        {
            return ApiResponse<Wallets>.Failed(
                new Dictionary<string, string>()
                    { { "WalletAlreadyExists", "This user already has an existing wallet" } },
                "Wallet already exists",
                HttpStatusCode.Conflict
            );
        }
        var createWalletResult = await _walletRepo.AddAsync(walletModel);
        await _walletRepo.SaveChangesAsync();
        return ApiResponse<Wallets>.Success(createWalletResult, "Wallet created", HttpStatusCode.Created);
    }

    public async Task<ApiResponse<Wallets>> AddMoneyInWallet(string userId, LoadMoneyDTO dto)
    {
        var wallet = await _walletRepo.FindSingleByConditionAsync(
            x => x.UserId == userId
        );

        wallet.Balance += dto.Balance; // add the balance

        var updatedWallet = await _walletRepo.UpdateAsync(wallet);
        await _walletRepo.SaveChangesAsync();
        return ApiResponse<Wallets>.Success(updatedWallet, "Wallet updated", HttpStatusCode.Accepted);
    }

    public async Task<ApiResponse<Wallets>> DeductBalance(string userId, decimal amount)
    {
        var wallet = await _walletRepo.FindSingleByConditionAsync(x => x.UserId == userId);
        if (wallet == null)
        {
            return ApiResponse<Wallets>.Failed(
                new Dictionary<string, string> { { "Wallet", "Wallet not found" } },
                "Payment failed",
                HttpStatusCode.NotFound
            );
        }

        if (wallet.Balance < amount)
        {
            return ApiResponse<Wallets>.Failed(
                new Dictionary<string, string> { { "Wallet", "Insufficient balance" } },
                "Payment failed",
                HttpStatusCode.BadRequest
            );
        }

        wallet.Balance -= amount;
        await _walletRepo.UpdateAsync(wallet);
        await _walletRepo.SaveChangesAsync();

        return ApiResponse<Wallets>.Success(wallet, "Payment successful");
    }

    public async Task<ApiResponse<Wallets>> AddBalance(string userId, decimal amount)
    {
        var wallet = await _walletRepo.FindSingleByConditionAsync(x => x.UserId == userId);
        if (wallet == null)
        {
            return ApiResponse<Wallets>.Failed(
                new Dictionary<string, string> { { "Wallet", "Wallet not found" } },
                "Transfer failed",
                HttpStatusCode.NotFound
            );
        }

        wallet.Balance += amount;
        await _walletRepo.UpdateAsync(wallet);
        await _walletRepo.SaveChangesAsync();

        return ApiResponse<Wallets>.Success(wallet, "Balance added");
    }
}