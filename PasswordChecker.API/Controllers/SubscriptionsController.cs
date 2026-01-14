using Microsoft.AspNetCore.Mvc;
using PasswordChecker.Server.DTOs.Subscription;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // GET: api/subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetAll()
        {
            var subscriptions = await _subscriptionService.GetAllAsync();
            return Ok(subscriptions);
        }

        // GET: api/subscriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionDto>> GetById(Guid id)
        {
            var subscription = await _subscriptionService.GetByIdAsync(id);
            if (subscription == null)
            {
                return NotFound($"Subscription with ID {id} not found.");
            }
            return Ok(subscription);
        }

        // POST: api/subscriptions
        [HttpPost]
        public async Task<ActionResult<SubscriptionDto>> Create([FromBody] CreateSubscriptionDto createSubscriptionDto)
        {
            try
            {
                var subscription = await _subscriptionService.CreateAsync(createSubscriptionDto);
                return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/subscriptions
        [HttpPut]
        public async Task<ActionResult<SubscriptionDto>> Update([FromBody] UpdateSubscriptionDto updateSubscriptionDto)
        {
            try
            {
                var subscription = await _subscriptionService.UpdateAsync(updateSubscriptionDto);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/subscriptions/{id}/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult<bool>> Deactivate(Guid id)
        {
            var result = await _subscriptionService.DeactivateAsync(id);
            if (!result)
            {
                return NotFound($"Subscription with ID {id} not found.");
            }
            return Ok(result);
        }

        // DELETE: api/subscriptions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _subscriptionService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Subscription with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
