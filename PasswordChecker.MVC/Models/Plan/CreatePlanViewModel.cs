using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Plan
{
    public class CreatePlanViewModel
    {
        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [Display(Name = "Plan Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive value")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Max checks per day is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [Display(Name = "Max Checks / Day")]
        public int MaxChecksPerDay { get; set; }
    }
}
