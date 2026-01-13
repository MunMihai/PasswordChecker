namespace PasswordChecker.Server.DTOs.Subscription
{
    public class UpdateSubscriptionDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly? EndDate { get; set; }
    }
}
