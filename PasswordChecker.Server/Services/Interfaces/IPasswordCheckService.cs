using PasswordChecker.Server.DTOs.PasswordCheck;

namespace PasswordChecker.Server.Services.Interfaces;

public interface IPasswordCheckService
{
    Task<PasswordCheckResultDto> CheckPasswordAsync(string password, Guid? userId = null);
}
