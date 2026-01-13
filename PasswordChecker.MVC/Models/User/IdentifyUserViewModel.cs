using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class IdentifyUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;
    }
}
