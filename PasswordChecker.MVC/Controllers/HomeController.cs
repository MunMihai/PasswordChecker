using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.MVC.Models.Dashboard;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.MVC.Controllers
{
    [Authorize(
        Roles = "ADMIN",
        AuthenticationSchemes = "AdminCookie"
    )]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPlanService _planService;
        private readonly ISubscriptionService _subscriptionService;

        public HomeController(
            IUserService userService,
            IPlanService planService,
            ISubscriptionService subscriptionService)
        {
            _userService = userService;
            _planService = planService;
            _subscriptionService = subscriptionService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            var plans = await _planService.GetAllAsync();
            var subscriptions = await _subscriptionService.GetAllAsync();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var soonLimit = today.AddDays(7);

            var model = new DashboardViewModel
            {
                Users = new UserStats
                {
                    Total = users.Count(),
                    Active = users.Count(u => u.Status == "ACTIVE"),
                    Blocked = users.Count(u => u.Status == "BLOCKED")
                },

                Subscriptions = new SubscriptionStats
                {
                    Total = subscriptions.Count(),

                    Active = subscriptions.Count(s =>
                        s.Status == "ACTIVE" &&
                        (!s.EndDate.HasValue || s.EndDate.Value >= today)),

                    Expired = subscriptions.Count(s =>
                        s.Status == "EXPIRED" ||
                        (s.EndDate.HasValue && s.EndDate.Value < today)),

                    ExpiringSoon = subscriptions.Count(s =>
                        s.Status == "ACTIVE" &&
                        s.EndDate.HasValue &&
                        s.EndDate.Value >= today &&
                        s.EndDate.Value <= soonLimit)
                },

                Plans = new PlanStats
                {
                    Total = plans.Count(),
                    UsageByPlan = subscriptions
                        .GroupBy(s => s.PlanName)
                        .ToDictionary(g => g.Key, g => g.Count())
                }
            };

            return View(model);
        }


    }
}
