namespace WalletService.Models.DTOs;

public class CreateWalletDTO
{
    public string UserId { get; set; }
    public decimal Balance { get; set; } = 0.0m;
}

public class LoadMoneyDTO
{
    public decimal Balance { get; set; }
}