using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordChecker.MVC.Models.User;
using PasswordChecker.Server.DTOs.User;
using PasswordChecker.Server.Services.Interfaces;

[Authorize(Roles = "Admin")]
public class UsersController : Controller

{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();

        var viewModels = users.Select(u => new UserViewModel
        {
            Email = u.Email,
            Varsta = u.Varsta,
            Gen = u.Gen,
            Status = u.Status
        });

        return View(viewModels);
    }

    // Mai jos trebuie de trecut pe API

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

        return View(new CompleteUserProfileViewModel
        {
            Email = email
        });
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

        var userId = Guid.Parse(userIdString);

        var user = await _userService.GetByIdAsync(userId);

        if (user == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Identify");
        }

        var viewModel = new UserViewModel
        {
            Email = user.Email,
            Varsta = user.Varsta,
            Gen = user.Gen,
            Status = user.Status,
            Balance = user.Balance
        };

        return View(viewModel);
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
         
        var userId = Guid.Parse(userIdString);

        await _userService.AddBalanceAsync(userId, model.Amount);

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
