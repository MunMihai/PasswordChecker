using PasswordChecker.Data.Models;

namespace PasswordChecker.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);

    }
}
