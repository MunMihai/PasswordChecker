using System.ComponentModel.DataAnnotations;

namespace PasswordChecker.MVC.Models.User
{
    public class AddBalanceViewModel
    {
        [Required]
        [Range(1, 10000, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
