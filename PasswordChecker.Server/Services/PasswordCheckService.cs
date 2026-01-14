using Microsoft.Extensions.Logging;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.PasswordCheck;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services;

public class PasswordCheckService : IPasswordCheckService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordCheckRepository _passwordCheckRepository;
    private readonly ILogger<PasswordCheckService> _logger;

    public PasswordCheckService(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IPasswordCheckRepository passwordCheckRepository,
        ILogger<PasswordCheckService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _passwordCheckRepository = passwordCheckRepository;
        _logger = logger;
    }

    public async Task<PasswordCheckResultDto> CheckPasswordAsync(string password, Guid? userId = null)
    {
        _logger.LogInformation("Password check requested");

        var result = EvaluatePassword(password);
        int? remainingChecks = null;
        int? maxChecksPerDay = null;

        if (userId.HasValue)
        {
            _logger.LogInformation("Password check performed by user: {UserId}", userId.Value);

            var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(userId.Value);
            if (subscription != null)
            {
                maxChecksPerDay = subscription.Plan.MaxChecksPerDay;
                var todayCount = await _passwordCheckRepository.GetTodayCountAsync(subscription.Id);

                if (todayCount >= maxChecksPerDay.Value)
                {
                    _logger.LogWarning("Subscription {SubscriptionId} has exceeded daily limit ({MaxChecks}/{TodayCount})", 
                        subscription.Id, maxChecksPerDay.Value, todayCount);
                    throw new InvalidOperationException($"Daily limit exceeded. You have used all {maxChecksPerDay} checks for today.");
                }

                remainingChecks = maxChecksPerDay.Value - todayCount - 1; // -1 because we're about to add one

                // Save the password check
                var passwordCheck = new PasswordCheck
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    SubscriptionId = subscription.Id,
                    Score = result.Score,
                    Level = result.Level,
                    CreatedAt = DateTime.UtcNow
                };

                await _passwordCheckRepository.AddAsync(passwordCheck);
                _logger.LogInformation("Password check saved for subscription {SubscriptionId}. Remaining checks: {Remaining}", 
                    subscription.Id, remainingChecks);
            }
            else
            {
                _logger.LogWarning("User {UserId} does not have an active subscription", userId.Value);
                throw new InvalidOperationException("You need an active subscription to check passwords.");
            }
        }

        result.RemainingChecks = remainingChecks;
        result.MaxChecksPerDay = maxChecksPerDay;

        return result;
    }

    private PasswordCheckResultDto EvaluatePassword(string password)
    {
        var score = 0;
        var recommendations = new List<string>();

        // Length check
        if (password.Length >= 12)
            score += 25;
        else if (password.Length >= 8)
            score += 15;
        else
            recommendations.Add("Use at least 8 characters (12+ recommended)");

        // Uppercase check
        if (password.Any(char.IsUpper))
            score += 15;
        else
            recommendations.Add("Add uppercase letters");

        // Lowercase check
        if (password.Any(char.IsLower))
            score += 15;
        else
            recommendations.Add("Add lowercase letters");

        // Digit check
        if (password.Any(char.IsDigit))
            score += 15;
        else
            recommendations.Add("Add numbers");

        // Special character check
        if (password.Any(ch => !char.IsLetterOrDigit(ch)))
            score += 20;
        else
            recommendations.Add("Add special characters (!@#$%^&*)");

        // Complexity bonus
        var uniqueChars = password.Distinct().Count();
        if (uniqueChars >= password.Length * 0.7)
            score += 10;

        // Determine level
        string level;
        if (score >= 80)
            level = "VERY_STRONG";
        else if (score >= 60)
            level = "STRONG";
        else if (score >= 40)
            level = "MEDIUM";
        else if (score >= 20)
            level = "WEAK";
        else
            level = "VERY_WEAK";

        return new PasswordCheckResultDto
        {
            Score = Math.Min(score, 100),
            Level = level,
            Recommendations = recommendations,
            IsValid = score >= 40
        };
    }
}
