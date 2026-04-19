using System.Net;
using Microsoft.AspNetCore.Identity;
using Shared.Models;
using Shared.Models.Enums;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.DTOs;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class AuthService(
    UserManager<Users> _userMgr,
    IUserRepository _userRepo,
    ITokenService _tokenService
) : IAuthService
{
    public async Task<ApiResponse<RegisterResponseDTO>> Register(RegisterDTO dto)
    {
        var userModel = new Users()
        {
            UserName = dto.Email.Split("@").First(),
            Email = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        var createUserResult = await _userMgr.CreateAsync(userModel, dto.Password);
        if (!createUserResult.Succeeded)
        {
            var errors = createUserResult.Errors.ToDictionary(
                e => e.Code,
                e => e.Description
            );
            throw new ServiceException(errors, HttpStatusCode.Conflict);
        }
        
        // Add user to the 'User' role
        var addToRoleResult = await _userMgr.AddToRoleAsync(userModel, UserRoles.User.ToString());
        if (!addToRoleResult.Succeeded)
        {
            // COMPENSATION STEP ; kind of like a transactoon rollback (rollback user creation)
            await _userMgr.DeleteAsync(userModel);
            
            var errors = addToRoleResult.Errors.ToDictionary(
                e => e.Code,
                e => e.Description
            );
            throw new ServiceException(errors, HttpStatusCode.Conflict);
        }
        
        var response = new RegisterResponseDTO()
        {
            EmailConfirmToken = await _userMgr.GenerateEmailConfirmationTokenAsync(userModel),
        };
        return ApiResponse<RegisterResponseDTO>.Success(response, "User created successfully");
    }
    
    public async Task<ApiResponse<LoginResponseDTO>> Login(LoginDTO dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return InvalidLoginResponse();
        }
        // if (user.EmailConfirmed == false)
        // {
        //     var errors = new Dictionary<string, string> { { "UnverifiedEmail", "Email is not verified. Please verify email first and try again" } };
        //     return ApiResponse<LoginResponseDTO>.Failed(errors, "Login failed", HttpStatusCode.Unauthorized);
        // }

        var isPasswordCorrect = await _userMgr.CheckPasswordAsync(user, dto.Password);
        if (isPasswordCorrect == false)
        {
            return InvalidLoginResponse();
        }

        var response = new LoginResponseDTO()
        {
            JwtToken = await _tokenService.GenerateJwtToken(user)
        };
        return ApiResponse<LoginResponseDTO>.Success(response, "User validated successfully");
    }
    
    private ApiResponse<LoginResponseDTO> InvalidLoginResponse()
    {
        var errors = new Dictionary<string, string>
        {
            { "InvalidCredentials", "Invalid username or password." }
        };

        return ApiResponse<LoginResponseDTO>.Failed(errors, "Login failed", HttpStatusCode.Unauthorized);
    }
}