using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.Server.Services.Interfaces;
using System.Security.Claims;

namespace PasswordChecker.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Email is required");
                return View();
            }

            var user = await _userService.GetByEmailAsync(email);

            if (user == null || user.Status != "ACTIVE")
            {
                ModelState.AddModelError("", "Invalid or inactive user");
                return View();
            }

            if (user.Role != "ADMIN")
            {
                ModelState.AddModelError("", "Administrator privileges required");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "AdminCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AdminCookie", principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
