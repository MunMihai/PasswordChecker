using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;

namespace PasswordChecker.Data.Repositories;

public class PasswordCheckRepository : IPasswordCheckRepository
{
    private readonly PasswordCheckerDbContext_CodeFirst _context;

    public PasswordCheckRepository(PasswordCheckerDbContext_CodeFirst context)
    {
        _context = context;
    }

    public async Task<PasswordCheck> AddAsync(PasswordCheck passwordCheck)
    {
        _context.PasswordChecks.Add(passwordCheck);
        await _context.SaveChangesAsync();
        return passwordCheck;
    }

    public async Task<int> GetTodayCountAsync(Guid subscriptionId)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.PasswordChecks
            .Where(pc => pc.SubscriptionId == subscriptionId && pc.CreatedAt >= today && pc.CreatedAt < tomorrow)
            .CountAsync();
    }
}
