using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.Auth;
using PasswordChecker.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PasswordChecker.Server.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponse> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

        var user = await _userRepository.GetByEmailAsync(loginDto.Email.Trim().ToLower());
        if (user == null || user.Status != "ACTIVE")
        {
            _logger.LogWarning("Login failed - user not found or inactive: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            _logger.LogWarning("Login failed - user has no password set: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed - invalid password for: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = GenerateJwtToken(user.Id, user.Email, user.Role);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        _logger.LogInformation("Login successful for user: {Email}", loginDto.Email);

        return new TokenResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = expiresAt
        };
    }

    public async Task<TokenResponse> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            throw new ArgumentException("Email is required");
        }

        if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters long");
        }

        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email.Trim().ToLower());
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
            throw new InvalidOperationException("Email already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Varsta = registerDto.Varsta,
            Gen = registerDto.Gen,
            Status = "ACTIVE",
            Role = "CUSTOMER",
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);

        var token = GenerateJwtToken(user.Id, user.Email, user.Role);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new TokenResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = expiresAt
        };
    }

    public string GenerateJwtToken(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
