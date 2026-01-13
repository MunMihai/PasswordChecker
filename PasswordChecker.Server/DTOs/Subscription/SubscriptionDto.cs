namespace PasswordChecker.Server.DTOs.Subscription
{
    public class SubscriptionDto
    {
        public Guid Id { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
