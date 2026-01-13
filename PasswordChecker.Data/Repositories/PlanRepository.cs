using Microsoft.EntityFrameworkCore;
using PasswordChecker.Data.Context;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;

namespace PasswordChecker.Data.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly PasswordCheckerDbContext_CodeFirst _context;

    public PlanRepository(PasswordCheckerDbContext_CodeFirst context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plan>> GetAllAsync()
    {
        return await _context.Plans
            .OrderBy(p => p.MaxChecksPerDay)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Plan> GetByIdAsync(Guid id)
    {
        return await _context.Plans.FindAsync(id);

    }

    public async Task<Plan> AddAsync(Plan plan)
    {
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<Plan> UpdateAsync(Plan plan)
    {
        _context.Plans.Update(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
            return false;

        _context.Plans.Remove(plan);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PlanExistsAsync(Guid id)
    {
        return await _context.Plans.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Plans.Where(p => p.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<bool> HasActiveSubscriptionsAsync(Guid planId)
    {
        return await _context.Subscriptions
            .AnyAsync(s => s.PlanId == planId && s.Status == "ACTIVE");
    }
}
