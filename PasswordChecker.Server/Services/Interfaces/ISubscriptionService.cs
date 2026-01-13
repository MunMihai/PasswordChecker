using PasswordChecker.Server.DTOs.Subscription;

namespace PasswordChecker.Server.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllAsync();
        Task<SubscriptionDto?> GetByIdAsync(Guid id);
        Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto);

        Task<SubscriptionDto> UpdateAsync(UpdateSubscriptionDto dto);
        Task<bool> DeactivateAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
