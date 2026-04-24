using System.Net;
using System.Text;
using BuildingBlocks.Events;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.DTOs;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class AuthService(
    UserManager<Users> _userMgr,
    IUserRepository _userRepo,
    ITokenService _tokenService,
    IPublishEndpoint _publishEndpoint
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

        var emailConfirmationToken = await _userMgr.GenerateEmailConfirmationTokenAsync(userModel);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
        #region publish message for email
        await _publishEndpoint.Publish(new UserRegisteredEvent
        {
            UserId = userModel.Id,
            Email = userModel.Email,
            Username = userModel.UserName,
            EmailConfirmationLink = $"https://localhost:5000/gateway/auth/confirm-email?email={userModel.Email}&token={encodedToken}"
        });
        #endregion

        var response = new RegisterResponseDTO()
        {
            EmailConfirmToken = emailConfirmationToken
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
        if (user.EmailConfirmed == false)
        {
            var errors = new Dictionary<string, string> { { "UnverifiedEmail", "Email is not verified. Please verify email first and try again" } };
            return ApiResponse<LoginResponseDTO>.Failed(errors, "Login failed", HttpStatusCode.Unauthorized);
        }

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

    public async Task<ApiResponse<string>> ConfirmEmail(ConfirmEmailDTO dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new ServiceException(
                new Dictionary<string, string> { { "UserNotFound", "No user found" } },
                HttpStatusCode.NoContent
            );
        }
        if (user.EmailConfirmed == true)
        {
            throw new ServiceException(
                new Dictionary<string, string> { { "EmailALreadyConfirmed", "Email for this user is already confirmed" } },
                HttpStatusCode.Conflict
            );
        }
        var result = await _userMgr.ConfirmEmailAsync(user, dto.Token);
        if (!result.Succeeded)
        {
            throw new ServiceException(
                result.Errors.ToDictionary(x => x.Code, x => x.Description),
                HttpStatusCode.NoContent
            );
        }
        return ApiResponse<string>.Success("Email confirmed successfully", "User confirmed successfully");
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