using System.ComponentModel.DataAnnotations;

namespace WalletService.Models;

public class Wallets
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal Balance { get; set; } = 0.0m;
}