using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.MVC.Models.Plan;
using PasswordChecker.Server.DTOs.Plan;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.MVC.Controllers
{
[Authorize(
    Roles = "ADMIN",
    AuthenticationSchemes = "AdminCookie"
)]
    public class PlansController : Controller

    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllAsync();
            var viewModels = plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                MaxChecksPerDay = p.MaxChecksPerDay,
                IsActive = p.IsActive
            });
            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var viewModel = new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxChecksPerDay = plan.MaxChecksPerDay,
                IsActive = plan.IsActive
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CreatePlanViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlanViewModel createPlanViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new CreatePlanDto
                    {
                        Name = createPlanViewModel.Name,
                        Price = createPlanViewModel.Price,
                        MaxChecksPerDay = createPlanViewModel.MaxChecksPerDay,
                    };
                    await _planService.CreateAsync(serviceDto);
                    TempData["SuccessMessage"] = "Plan created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(createPlanViewModel);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            var updatePlanViewModel = new EditPlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxChecksPerDay = plan.MaxChecksPerDay,
                IsActive = plan.IsActive,
            };

            return View(updatePlanViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditPlanViewModel updatePlanViewModel)
        {
            if (id != updatePlanViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceDto = new UpdatePlanDto
                    {
                        Id = updatePlanViewModel.Id,
                        Name = updatePlanViewModel.Name,
                        Price = updatePlanViewModel.Price,
                        MaxChecksPerDay = updatePlanViewModel.MaxChecksPerDay,
                        IsActive = updatePlanViewModel.IsActive,
                    };
                    await _planService.UpdateAsync(serviceDto);
                    TempData["SuccessMessage"] = "Plan updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(updatePlanViewModel);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _planService.GetByIdAsync(id.Value);
            if (plan == null)
                return NotFound();

            ViewBag.CanDelete = await _planService.CanDeleteAsync(id.Value);

            var viewModel = new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxChecksPerDay = plan.MaxChecksPerDay,
                IsActive = plan.IsActive,
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _planService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Plan deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Mai jos trebuie de trecut pe API
        //[NonAction]
        public async Task<IActionResult> Select()
        {
            var plans = await _planService.GetAllAsync();

            var viewModels = plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                MaxChecksPerDay = p.MaxChecksPerDay,
                IsActive = p.IsActive
            });

            return View(viewModels);
        }
        //[NonAction]
        [HttpPost]
        public async Task<IActionResult> Choose(Guid id)
        {
            // aici vei lega planul de utilizator
            // ex: _subscriptionService.Subscribe(userId, id);

            TempData["SuccessMessage"] = "Plan selected successfully!";
            return RedirectToAction(nameof(Select));
        }


    }
}
