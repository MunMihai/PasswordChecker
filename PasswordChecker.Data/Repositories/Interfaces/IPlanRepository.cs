using PasswordChecker.Data.Models;


namespace PasswordChecker.Data.Repositories.Interfaces;

public interface IPlanRepository
{
    Task<IEnumerable<Plan>> GetAllAsync();
    Task<Plan> GetByIdAsync(Guid id);
    Task<Plan> AddAsync(Plan plan);
    Task<Plan> UpdateAsync(Plan plan);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> PlanExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string code, Guid? excludeId = null);
    Task<bool> HasActiveSubscriptionsAsync(Guid planId);
    Task<bool> HasAnySubscriptionsAsync(Guid planId);

}
