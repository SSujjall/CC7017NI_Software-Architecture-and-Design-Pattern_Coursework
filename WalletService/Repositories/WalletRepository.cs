using BuildingBlocks.GenericRepo;
using Microsoft.EntityFrameworkCore;
using WalletService.Data;
using WalletService.Models;
using WalletService.Repositories.Interfaces;

namespace WalletService.Repositories;

public class WalletRepository : GenericRepo<Wallets>, IWalletRepository
{
    public WalletRepository(WalletDbContext context) : base(context)
    {
    }
}