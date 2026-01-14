using PasswordChecker.Server.DTOs.Auth;

namespace PasswordChecker.Server.Services.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginDto loginDto);
    Task<TokenResponse> RegisterAsync(RegisterDto registerDto);
    string GenerateJwtToken(Guid userId, string email, string role);
}
