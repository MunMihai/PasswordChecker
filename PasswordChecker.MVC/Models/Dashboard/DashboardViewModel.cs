namespace PasswordChecker.MVC.Models.Dashboard
{
    public class DashboardViewModel
    {
        public UserStats Users { get; set; }
        public SubscriptionStats Subscriptions { get; set; }
        public PlanStats Plans { get; set; }
    }

    public class UserStats
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Blocked { get; set; }
    }

    public class SubscriptionStats
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Expired { get; set; }
        public int ExpiringSoon { get; set; }
    }

    public class PlanStats
    {
        public int Total { get; set; }
        public Dictionary<string, int> UsageByPlan { get; set; }
    }
}
