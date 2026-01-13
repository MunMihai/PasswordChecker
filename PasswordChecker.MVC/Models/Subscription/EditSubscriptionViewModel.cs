using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.Subscription
{
    public class EditSubscriptionViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        [RegularExpression(
            "ACTIVE|INACTIVE|EXPIRED",
            ErrorMessage = "Status must be ACTIVE, INACTIVE or EXPIRED"
        )]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "End Date")]
        public DateOnly? EndDate { get; set; }
    }
}
