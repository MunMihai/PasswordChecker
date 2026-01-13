using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PasswordChecker.MVC.Models.Subscription;
using PasswordChecker.Server.DTOs.Subscription;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.MVC.Controllers
{

[Authorize(
    Roles = "ADMIN",
    AuthenticationSchemes = "AdminCookie"
)]
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IUserService _userService;
        private readonly IPlanService _planService;

        public SubscriptionsController(
            ISubscriptionService subscriptionService,
            IUserService userService,
            IPlanService planService)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var subs = await _subscriptionService.GetAllAsync();

            var model = subs.Select(s => new SubscriptionViewModel
            {
                Id = s.Id,
                UserEmail = s.UserEmail,
                PlanName = s.PlanName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            });

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();

            return View(new CreateSubscriptionViewModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubscriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(model);
            }

            try
            {
                await _subscriptionService.CreateAsync(new CreateSubscriptionDto
                {
                    UserId = model.UserId,
                    PlanId = model.PlanId,
                    StartDate = model.StartDate
                });

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var sub = await _subscriptionService.GetByIdAsync(id);
            if (sub == null)
                return NotFound();

            var model = new EditSubscriptionViewModel
            {
                Id = sub.Id,
                Status = sub.Status,
                EndDate = sub.EndDate
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSubscriptionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _subscriptionService.UpdateAsync(new UpdateSubscriptionDto
            {
                Id = model.Id,
                Status = model.Status,
                EndDate = model.EndDate
            });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(Guid id)
        {
            await _subscriptionService.DeactivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _subscriptionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.Users = new SelectList(
                await _userService.GetAllAsync(),
                "Id",
                "Email"
            );

            ViewBag.Plans = new SelectList(
                (await _planService.GetAllAsync()).Where(p => p.IsActive),
                "Id",
                "Name"
            );
        }
    }
}