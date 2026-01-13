using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.Plan;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Data.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;

    public PlanService(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _planRepository.GetAllAsync();

        return plans.Select(MapToDto);
    }

    public async Task<PlanDto?> GetByIdAsync(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
            return null;

        return MapToDto(plan);
    }

    public async Task<PlanDto> CreateAsync(CreatePlanDto createPlanDto)
    {
        if (await _planRepository.ExistsByNameAsync(createPlanDto.Name))
        {
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
        return MapToDto(createdPlan);
    }

    public async Task<PlanDto> UpdateAsync(UpdatePlanDto updatePlanDto)
    {
        var existingPlan = await _planRepository.GetByIdAsync(updatePlanDto.Id);
        if (existingPlan == null)
        {
            throw new InvalidOperationException(
                $"Planul cu ID '{updatePlanDto.Id}' nu a fost găsit.");
        }
        if (await _planRepository.ExistsByNameAsync(
                updatePlanDto.Name, updatePlanDto.Id))
        {
            throw new InvalidOperationException(
                $"Planul cu numele '{updatePlanDto.Name}' există deja.");
        }

        existingPlan.Name = updatePlanDto.Name;
        existingPlan.Price = updatePlanDto.Price;
        existingPlan.MaxChecksPerDay = updatePlanDto.MaxChecksPerDay;
        existingPlan.IsActive = updatePlanDto.IsActive;

        var updatedPlan = await _planRepository.UpdateAsync(existingPlan);
        return MapToDto(updatedPlan);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await CanDeleteAsync(id))
        {
            throw new InvalidOperationException(
                "Planul nu poate fi șters deoarece are abonamente active.");
        }

        await _planRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> CanDeleteAsync(Guid id)
    {
        return !await _planRepository.HasActiveSubscriptionsAsync(id);
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
