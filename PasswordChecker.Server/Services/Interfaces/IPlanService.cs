using PasswordChecker.Server.DTOs.Plan;

namespace PasswordChecker.Server.Services.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<PlanDto>> GetAllAsync();
    Task<PlanDto?> GetByIdAsync(Guid id);
    Task<PlanDto> CreateAsync(CreatePlanDto createPlanDto);
    Task<PlanDto> UpdateAsync(UpdatePlanDto updatePlanDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> CanDeleteAsync(Guid id);


}
