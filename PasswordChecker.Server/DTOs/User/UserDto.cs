namespace PasswordChecker.Server.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int? Varsta { get; set; }
        public string? Gen { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
