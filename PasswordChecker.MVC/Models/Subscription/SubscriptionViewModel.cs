using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Subscription
{
    public class SubscriptionViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "User Email")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Plan")]
        public string PlanName { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly? EndDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;
    }
}
