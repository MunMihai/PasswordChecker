using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Plan
{
    public class EditPlanViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [Display(Name = "Plan Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive value")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Max checks must be at least 1")]
        [Display(Name = "Max Checks / Day")]
        public int MaxChecksPerDay { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
