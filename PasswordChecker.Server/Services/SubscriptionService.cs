using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.Subscription;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repository;

        public SubscriptionService(ISubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            var subs = await _repository.GetAllAsync();

            return subs.Select(MapToDto);
        }

        public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
        {
            var sub = await _repository.GetByIdAsync(id);
            return sub == null ? null : MapToDto(sub);
        }

        public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto)
        {
            // business rule
            var existingActive = (await _repository.GetAllAsync())
                .Any(s => s.UserId == dto.UserId && s.Status == "ACTIVE");

            if (existingActive)
                throw new InvalidOperationException("User already has an active subscription.");

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                StartDate = dto.StartDate,
                Status = "ACTIVE"
            };

            await _repository.AddAsync(subscription);

            // 🔥 RELOAD cu Include
            var reloaded = await _repository.GetByIdAsync(subscription.Id)
                ?? throw new InvalidOperationException("Subscription reload failed.");

            return MapToDto(reloaded);
        }


        public async Task<SubscriptionDto> UpdateAsync(UpdateSubscriptionDto dto)
        {
            var sub = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException("Subscription not found.");

            sub.Status = dto.Status;
            sub.EndDate = dto.EndDate;

            var updated = await _repository.UpdateAsync(sub);
            return MapToDto(updated);
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var sub = await _repository.GetByIdAsync(id);
            if (sub == null)
                return false;

            sub.Status = "INACTIVE";
            sub.EndDate ??= DateOnly.FromDateTime(DateTime.UtcNow);

            await _repository.UpdateAsync(sub);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        // ==========================
        // Mapping
        // ==========================
        private static SubscriptionDto MapToDto(Subscription s)
        {
            return new SubscriptionDto
            {
                Id = s.Id,
                UserEmail = s.User.Email,
                PlanName = s.Plan.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            };
        }
    }
}
