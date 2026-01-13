using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class CompleteUserProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(1, 120)]
        public int? Varsta { get; set; }

        [StringLength(10)]
        public string? Gen { get; set; }
    }
}
