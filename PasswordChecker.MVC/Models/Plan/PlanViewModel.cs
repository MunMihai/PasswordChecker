using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Plan
{
    public class PlanViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Plan Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Max Checks / Day")]
        public int MaxChecksPerDay { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
