using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            ViewBag.UsersTotal = users.Count();
            ViewBag.UsersActive = users.Count(u => u.Status == "ACTIVE");
            ViewBag.UsersBlocked = users.Count(u => u.Status == "BLOCKED");

            ViewBag.PlansTotal = plans.Count();
            ViewBag.PlansActive = plans.Count(p => p.IsActive);

            ViewBag.SubscriptionsTotal = subscriptions.Count();
            ViewBag.SubscriptionsActive = subscriptions.Count(s => s.Status == "ACTIVE");
            ViewBag.SubscriptionsInactive = subscriptions.Count(s => s.Status != "ACTIVE");

            return View();
        }
    }
}
