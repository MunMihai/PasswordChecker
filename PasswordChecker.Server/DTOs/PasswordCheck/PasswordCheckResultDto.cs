namespace PasswordChecker.Server.DTOs.PasswordCheck;

public class PasswordCheckResultDto
{
    public int Score { get; set; }
    public string Level { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
    public bool IsValid { get; set; }
    public int? RemainingChecks { get; set; }
    public int? MaxChecksPerDay { get; set; }
}
