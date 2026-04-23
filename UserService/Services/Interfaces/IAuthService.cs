using BuildingBlocks.Models;
using UserService.Models.DTOs;

namespace UserService.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<RegisterResponseDTO>> Register(RegisterDTO dto);
    Task<ApiResponse<LoginResponseDTO>> Login(LoginDTO dto);
    Task<ApiResponse<string>> ConfirmEmail(ConfirmEmailDTO dto);
}