using PasswordChecker.Server.DTOs.User;

namespace PasswordChecker.Server.Services.Interfaces
{
    public interface IUserService
    {
        // READ
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByEmailAsync(string email);

        // CREATE
        Task<UserDto> CreateAsync(CreateUserDto dto);

        // UPDATE
        Task<UserDto> UpdateAsync(UpdateUserDto dto);
        Task<UserDto> AddBalanceAsync(Guid userId, decimal amount);

        // DELETE
        Task DeleteAsync(Guid id);
    }
}
