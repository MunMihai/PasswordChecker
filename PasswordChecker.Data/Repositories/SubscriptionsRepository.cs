using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;

namespace PasswordChecker.Data.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly PasswordCheckerDbContext_CodeFirst _context;

        public SubscriptionRepository(PasswordCheckerDbContext_CodeFirst context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Plan)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
                return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
