using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Subscription
{
    public class CreateSubscriptionViewModel
    {
        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Plan is required")]
        [Display(Name = "Plan")]
        public Guid PlanId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }
    }
}
