using System;
using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class UserViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Age")]
        public int? Varsta { get; set; }

        [Display(Name = "Gender")]
        public string? Gen { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
    }
}
