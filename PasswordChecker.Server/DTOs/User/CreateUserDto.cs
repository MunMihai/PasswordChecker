namespace PasswordChecker.Server.DTOs.User
{
    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public int? Varsta { get; set; }
        public string? Gen { get; set; }
    }
}
