using UserService.Models;

namespace UserService.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtToken(Users user);
}