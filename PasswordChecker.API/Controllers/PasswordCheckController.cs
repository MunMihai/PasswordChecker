using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.Server.DTOs.PasswordCheck;
using PasswordChecker.Server.Services.Interfaces;
using System.Security.Claims;

namespace PasswordChecker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordCheckController : ControllerBase
    {
        private readonly IPasswordCheckService _passwordCheckService;

        public PasswordCheckController(IPasswordCheckService passwordCheckService)
        {
            _passwordCheckService = passwordCheckService;
        }

        [HttpPost("check")]
        [AllowAnonymous]
        public async Task<ActionResult<PasswordCheckResultDto>> CheckPassword([FromBody] CheckPasswordDto dto)
        {
            try
            {
                Guid? userId = null;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _passwordCheckService.CheckPasswordAsync(dto.Password, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while checking password");
            }
        }
    }
}
