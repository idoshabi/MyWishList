using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Models.ViewModels;

namespace MyWishList.Web.Controllers;

public class AccountController(AppDbContext dbContext) : Controller
{
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usernameExists = await dbContext.Users.AnyAsync(u => u.Username == model.Username);
        var emailExists = await dbContext.Users.AnyAsync(u => u.Email == model.Email);

        if (usernameExists || emailExists)
        {
            ModelState.AddModelError(string.Empty, "Username or email already exists.");
            return View(model);
        }

        var user = new User
        {
            Username = model.Username.Trim(),
            Email = model.Email.Trim().ToLowerInvariant(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            DateOfBirth = model.DateOfBirth,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userInput = model.UsernameOrEmail.Trim();
        var user = await dbContext.Users.FirstOrDefaultAsync(u =>
            u.Username == userInput || u.Email == userInput.ToLower());

        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("FirstName", user.FirstName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
