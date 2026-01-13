using PasswordChecker.Server.DTOs.User;

namespace PasswordChecker.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> AddBalanceAsync(Guid userId, decimal amount);

    }
}
