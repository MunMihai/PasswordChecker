using Microsoft.Extensions.Logging;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.Plan;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Data.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<PlanService> _logger;

    public PlanService(
        IPlanRepository planRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<PlanService> logger)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all plans");
        var plans = await _planRepository.GetAllAsync();
        var planDtos = plans.Select(MapToDto).ToList();
        _logger.LogInformation("Fetched {Count} plans from repository", planDtos.Count);
        return planDtos;
    }

    public async Task<PlanDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting plan by ID: {PlanId}", id);
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            _logger.LogWarning("Plan {PlanId} not found", id);
            return null;
        }
        return MapToDto(plan);
    }

    public async Task<PlanDto> CreateAsync(CreatePlanDto createPlanDto)
    {
        _logger.LogInformation("Creating new plan: {PlanName}", createPlanDto.Name);

        if (await _planRepository.ExistsByNameAsync(createPlanDto.Name))
        {
            _logger.LogWarning("Attempted to create plan with existing name: {PlanName}", createPlanDto.Name);
            throw new InvalidOperationException(
                $"Planul '{createPlanDto.Name}' există deja.");
        }

        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Name = createPlanDto.Name,
            Price = createPlanDto.Price,
            MaxChecksPerDay = createPlanDto.MaxChecksPerDay,
            IsActive = true
        };

        var createdPlan = await _planRepository.AddAsync(plan);
        _logger.LogInformation("Plan created successfully with ID: {PlanId}", createdPlan.Id);

        return MapToDto(createdPlan);
    }

    public async Task<PlanDto> UpdateAsync(UpdatePlanDto updatePlanDto)
    {
        _logger.LogInformation("Updating plan with ID: {PlanId}", updatePlanDto.Id);

        var existingPlan = await _planRepository.GetByIdAsync(updatePlanDto.Id);
        if (existingPlan == null)
        {
            _logger.LogWarning("Plan {PlanId} not found for update", updatePlanDto.Id);
            throw new InvalidOperationException(
                $"Planul cu ID '{updatePlanDto.Id}' nu a fost găsit.");
        }
        if (await _planRepository.ExistsByNameAsync(
                updatePlanDto.Name, updatePlanDto.Id))
        {
            _logger.LogWarning("Attempted to update plan {PlanId} with existing name: {PlanName}", 
                updatePlanDto.Id, updatePlanDto.Name);
            throw new InvalidOperationException(
                $"Planul cu numele '{updatePlanDto.Name}' există deja.");
        }

        existingPlan.Name = updatePlanDto.Name;
        existingPlan.Price = updatePlanDto.Price;
        existingPlan.MaxChecksPerDay = updatePlanDto.MaxChecksPerDay;
        existingPlan.IsActive = updatePlanDto.IsActive;

        var updatedPlan = await _planRepository.UpdateAsync(existingPlan);
        _logger.LogInformation("Plan {PlanId} updated successfully", updatePlanDto.Id);

        return MapToDto(updatedPlan);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting plan with ID: {PlanId}", id);

        if (!await CanDeleteAsync(id))
        {
            _logger.LogWarning("Plan {PlanId} cannot be deleted - has active subscriptions", id);
            var users = await GetUsersUsingPlanAsync(id);
            var usersList = string.Join(", ", users);
            throw new InvalidOperationException(
                $"Planul nu poate fi șters deoarece este utilizat de următorii utilizatori: {usersList}");
        }

        await _planRepository.DeleteAsync(id);
        _logger.LogInformation("Plan {PlanId} deleted successfully", id);

        return true;
    }

    public async Task<bool> CanDeleteAsync(Guid id)
    {
        _logger.LogInformation("Checking if plan {PlanId} can be deleted", id);
        // Verificăm dacă există orice abonamente (nu doar active) pentru că foreign key constraint-ul
        // va preveni ștergerea dacă există orice abonamente care referențiază planul
        var canDelete = !await _planRepository.HasAnySubscriptionsAsync(id);
        _logger.LogInformation("Plan {PlanId} can be deleted: {CanDelete}", id, canDelete);
        return canDelete;
    }

    public async Task<IEnumerable<string>> GetUsersUsingPlanAsync(Guid planId)
    {
        _logger.LogInformation("Getting users using plan {PlanId}", planId);
        var subscriptions = await _subscriptionRepository.GetByPlanIdAsync(planId);
        // Returnăm toți utilizatorii care au abonamente la acest plan (nu doar active)
        // pentru că orice abonament previne ștergerea planului
        var userEmails = subscriptions
            .Select(s => s.User.Email)
            .Distinct()
            .ToList();
        _logger.LogInformation("Found {Count} users using plan {PlanId}", userEmails.Count, planId);
        return userEmails;
    }

    private static PlanDto MapToDto(Plan plan)
    {
        return new PlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            MaxChecksPerDay = plan.MaxChecksPerDay,
            IsActive = plan.IsActive
        };
    }
}
