using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int? Varsta { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gen { get; set; }
    }
}
