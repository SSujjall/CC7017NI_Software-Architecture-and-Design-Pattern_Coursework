using BuildingBlocks.Models.Enums;
using Microsoft.AspNetCore.Identity;
using UserService.Models;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class UserService(
    UserManager<Users> _userManager
) : IUserService
{
    public async Task<string?> GetSuperAdminId()
    {
        var superAdmins = await _userManager.GetUsersInRoleAsync(UserRoles.Superadmin.ToString());
        return superAdmins.FirstOrDefault()?.Id;
    }
}