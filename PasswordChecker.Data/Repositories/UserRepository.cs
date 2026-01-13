using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;

namespace PasswordChecker.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PasswordCheckerDbContext_CodeFirst _context;

        public UserRepository(PasswordCheckerDbContext_CodeFirst context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


    }
}
