namespace UserService.Services.Interfaces;

public interface IUserService
{
    Task<string?> GetSuperAdminId();
}