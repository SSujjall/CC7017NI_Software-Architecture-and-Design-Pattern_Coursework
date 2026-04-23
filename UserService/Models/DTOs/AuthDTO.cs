using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs;

public class LoginDTO
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDTO
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}

public class ConfirmEmailDTO
{
    [EmailAddress]
    public string Email { get; set; }
    public string Token { get; set; }
}

public class RegisterResponseDTO
{
    public string EmailConfirmToken { get; set; } // This is just for local development, will remove later
}

public class LoginResponseDTO
{
    public string JwtToken { get; set; }
}