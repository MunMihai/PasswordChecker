using System;
using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class EditUserViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Age")]
        [Required(ErrorMessage = "Age is required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int? Varsta { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required")]
        public string? Gen { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("ACTIVE|INACTIVE|BLOCKED", ErrorMessage = "Invalid status value")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("ADMIN|CUSTOMER", ErrorMessage = "Invalid role value")]
        public string Role { get; set; } = string.Empty;
    }
}
