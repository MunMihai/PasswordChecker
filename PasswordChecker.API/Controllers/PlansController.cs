using Microsoft.AspNetCore.Mvc;
using PasswordChecker.Server.DTOs.Plan;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        // GET: api/plans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetAll()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }

        // GET: api/plans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanDto>> GetById(Guid id)
        {
            var plan = await _planService.GetByIdAsync(id);
            if (plan == null)
            {
                return NotFound($"Plan with ID {id} not found.");
            }
            return Ok(plan);
        }

        // POST: api/plans
        [HttpPost]
        public async Task<ActionResult<PlanDto>> Create([FromBody] CreatePlanDto createPlanDto)
        {
            try
            {
                var plan = await _planService.CreateAsync(createPlanDto);
                return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/plans
        [HttpPut]
        public async Task<ActionResult<PlanDto>> Update([FromBody] UpdatePlanDto updatePlanDto)
        {
            try
            {
                var plan = await _planService.UpdateAsync(updatePlanDto);
                return Ok(plan);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/plans/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _planService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/plans/{id}/can-delete
        [HttpGet("{id}/can-delete")]
        public async Task<ActionResult<bool>> CanDelete(Guid id)
        {
            var canDelete = await _planService.CanDeleteAsync(id);
            return Ok(canDelete);
        }
    }
}
