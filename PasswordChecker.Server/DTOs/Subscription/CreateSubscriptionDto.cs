namespace PasswordChecker.Server.DTOs.Subscription
{
    public class CreateSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
