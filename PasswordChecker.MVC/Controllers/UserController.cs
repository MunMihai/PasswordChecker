using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.MVC.Models.User;
using PasswordChecker.Server.DTOs.User;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.MVC.Controllers
{
[Authorize(
    Roles = "ADMIN",
    AuthenticationSchemes = "AdminCookie"
)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // =========================
        // ADMIN CRUD
        // =========================

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();

            var viewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Varsta = u.Varsta,
                Gen = u.Gen,
                Status = u.Status,
                Role = u.Role,
                Balance = u.Balance
            });

            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Varsta = user.Varsta,
                Gen = user.Gen,
                Status = user.Status,
                Role = user.Role,
                Balance = user.Balance
            });
        }

        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.CreateAsync(new CreateUserDto
                {
                    Email = model.Email,
                    Varsta = model.Varsta,
                    Gen = model.Gen
                });

                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.Email), ex.Message);
                return View(model);
            }
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Varsta = user.Varsta,
                Gen = user.Gen,
                Status = user.Status,
                Role = user.Role
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _userService.UpdateAsync(new UpdateUserDto
                {
                    Id = model.Id,
                    Varsta = model.Varsta,
                    Gen = model.Gen,
                    Status = model.Status,
                    Role = model.Role
                });

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // TEMP CLIENT FLOW (va fi mutat în API)
        // =========================

        public IActionResult Identify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Identify(IdentifyUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetByEmailAsync(model.Email);

            if (user != null)
            {
                SetSession(user);
                return RedirectToAction("Dashboard");
            }

            TempData["Email"] = model.Email;
            return RedirectToAction("CompleteProfile");
        }

        public IActionResult CompleteProfile()
        {
            var email = TempData["Email"] as string;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Identify");

            return View(new CompleteUserProfileViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.CreateAsync(new CreateUserDto
            {
                Email = model.Email,
                Varsta = model.Varsta,
                Gen = model.Gen
            });

            SetSession(user);
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Identify");

            var user = await _userService.GetByIdAsync(Guid.Parse(userIdString));
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Identify");
            }

            return View(new UserViewModel
            {
                Email = user.Email,
                Varsta = user.Varsta,
                Gen = user.Gen,
                Status = user.Status,
                Balance = user.Balance
            });
        }

        public IActionResult AddBalance()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Identify");

            return View(new AddBalanceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(AddBalanceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Identify");

            await _userService.AddBalanceAsync(Guid.Parse(userIdString), model.Amount);

            TempData["SuccessMessage"] = "Balance updated successfully!";
            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private void SetSession(UserDto user)
        {
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
        }
    }
}