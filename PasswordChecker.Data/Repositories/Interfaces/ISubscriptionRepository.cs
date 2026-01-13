using PasswordChecker.Data.Models;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(Guid id);

    Task<Subscription> AddAsync(Subscription subscription);
    Task<Subscription> UpdateAsync(Subscription subscription);
    Task<bool> DeleteAsync(Guid id);
}
