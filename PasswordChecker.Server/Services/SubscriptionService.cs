using Microsoft.Extensions.Logging;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.Subscription;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IMemoryCacheService _cacheService;
        private readonly ILogger<SubscriptionService> _logger;

        private const string CACHE_KEY_ALL_SUBSCRIPTIONS = "subscriptions_all";
        private const string CACHE_KEY_SUBSCRIPTION_BY_ID = "subscription_{0}";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(5);

        public SubscriptionService(
            ISubscriptionRepository repository,
            IMemoryCacheService cacheService,
            ILogger<SubscriptionService> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all subscriptions");

            return await _cacheService.GetOrSetAsync(
                CACHE_KEY_ALL_SUBSCRIPTIONS,
                async () =>
                {
                    _logger.LogInformation("Fetching all subscriptions from repository");
                    var subs = await _repository.GetAllAsync();
                    await CheckAndUpdateExpiredSubscriptionsAsync(subs);
                    var subDtos = subs.Select(MapToDto).ToList();
                    _logger.LogInformation("Fetched {Count} subscriptions from repository", subDtos.Count);
                    return subDtos;
                },
                CACHE_DURATION) ?? Enumerable.Empty<SubscriptionDto>();
        }

        public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting subscription by ID: {SubscriptionId}", id);

            var cacheKey = string.Format(CACHE_KEY_SUBSCRIPTION_BY_ID, id);

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    _logger.LogInformation("Fetching subscription {SubscriptionId} from repository", id);
                    var sub = await _repository.GetByIdAsync(id);
                    if (sub == null)
                    {
                        _logger.LogWarning("Subscription {SubscriptionId} not found", id);
                        return null;
                    }
                    await CheckAndUpdateExpiredSubscriptionAsync(sub);
                    return MapToDto(sub);
                },
                CACHE_DURATION);
        }

        public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto)
        {
            _logger.LogInformation("Creating new subscription for user {UserId} with plan {PlanId}", 
                dto.UserId, dto.PlanId);

            // If user has an active subscription, check if it's the same plan
            var allSubs = await _repository.GetAllAsync();
            await CheckAndUpdateExpiredSubscriptionsAsync(allSubs);
            var existingActive = allSubs.FirstOrDefault(s => s.UserId == dto.UserId && s.Status == "ACTIVE");

            if (existingActive != null)
            {
                if (existingActive.PlanId == dto.PlanId)
                {
                    _logger.LogWarning("User {UserId} already has an active subscription with plan {PlanId}", 
                        dto.UserId, dto.PlanId);
                    throw new InvalidOperationException("You already have an active subscription for this plan.");
                }

                _logger.LogInformation("User {UserId} has an active subscription {SubscriptionId}, deactivating it", 
                    dto.UserId, existingActive.Id);
                existingActive.Status = "INACTIVE";
                existingActive.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await _repository.UpdateAsync(existingActive);
                InvalidateSubscriptionCache(existingActive.Id);
            }

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                StartDate = dto.StartDate,
                Status = "ACTIVE"
            };

            await _repository.AddAsync(subscription);
            _logger.LogInformation("Subscription created with ID: {SubscriptionId}", subscription.Id);

            // 🔥 RELOAD cu Include
            var reloaded = await _repository.GetByIdAsync(subscription.Id)
                ?? throw new InvalidOperationException("Subscription reload failed.");

            // Invalidate cache
            InvalidateSubscriptionCache(subscription.Id);

            _logger.LogInformation("Subscription {SubscriptionId} created successfully", subscription.Id);
            return MapToDto(reloaded);
        }


        public async Task<SubscriptionDto> UpdateAsync(UpdateSubscriptionDto dto)
        {
            _logger.LogInformation("Updating subscription with ID: {SubscriptionId}", dto.Id);

            var sub = await _repository.GetByIdAsync(dto.Id);
            if (sub == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found for update", dto.Id);
                throw new InvalidOperationException("Subscription not found.");
            }

            sub.Status = dto.Status;
            sub.EndDate = dto.EndDate;

            // Check if subscription should be marked as EXPIRED after update
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (sub.Status == "ACTIVE" && sub.EndDate.HasValue && sub.EndDate.Value < today)
            {
                _logger.LogInformation("Subscription {SubscriptionId} has expired (EndDate: {EndDate}), updating status to EXPIRED", 
                    sub.Id, sub.EndDate.Value);
                sub.Status = "EXPIRED";
            }

            var updated = await _repository.UpdateAsync(sub);
            _logger.LogInformation("Subscription {SubscriptionId} updated successfully", dto.Id);

            // Invalidate cache
            InvalidateSubscriptionCache(updated.Id);

            return MapToDto(updated);
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            _logger.LogInformation("Deactivating subscription with ID: {SubscriptionId}", id);

            var sub = await _repository.GetByIdAsync(id);
            if (sub == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found for deactivation", id);
                return false;
            }

            sub.Status = "INACTIVE";
            sub.EndDate ??= DateOnly.FromDateTime(DateTime.UtcNow);

            await _repository.UpdateAsync(sub);
            _logger.LogInformation("Subscription {SubscriptionId} deactivated successfully", id);

            // Invalidate cache
            InvalidateSubscriptionCache(id);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting subscription with ID: {SubscriptionId}", id);

            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Subscription {SubscriptionId} deleted successfully", id);
                // Invalidate cache
                InvalidateSubscriptionCache(id);
            }
            else
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found for deletion", id);
            }

            return result;
        }

        private async Task CheckAndUpdateExpiredSubscriptionAsync(Subscription subscription)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (subscription.Status == "ACTIVE" && subscription.EndDate.HasValue && subscription.EndDate.Value < today)
            {
                _logger.LogInformation("Subscription {SubscriptionId} has expired (EndDate: {EndDate}), updating status to EXPIRED", 
                    subscription.Id, subscription.EndDate.Value);
                subscription.Status = "EXPIRED";
                await _repository.UpdateAsync(subscription);
                InvalidateSubscriptionCache(subscription.Id);
            }
        }

        private async Task CheckAndUpdateExpiredSubscriptionsAsync(IEnumerable<Subscription> subscriptions)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredSubIds = subscriptions
                .Where(s => s.Status == "ACTIVE" && s.EndDate.HasValue && s.EndDate.Value < today)
                .Select(s => s.Id)
                .ToList();

            if (expiredSubIds.Any())
            {
                _logger.LogInformation("Found {Count} expired subscriptions, updating their status", expiredSubIds.Count);
                foreach (var subId in expiredSubIds)
                {
                    var subToUpdate = await _repository.GetByIdAsync(subId);
                    if (subToUpdate != null)
                    {
                        subToUpdate.Status = "EXPIRED";
                        await _repository.UpdateAsync(subToUpdate);
                        InvalidateSubscriptionCache(subId);
                    }
                }
            }
        }

        private void InvalidateSubscriptionCache(Guid subscriptionId)
        {
            _cacheService.Remove(CACHE_KEY_ALL_SUBSCRIPTIONS);
            _cacheService.Remove(string.Format(CACHE_KEY_SUBSCRIPTION_BY_ID, subscriptionId));
            _logger.LogInformation("Cache invalidated for subscription {SubscriptionId}", subscriptionId);
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
