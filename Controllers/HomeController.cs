using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Models.ViewModels;

namespace MyWishList.Web.Controllers;

public class HomeController(AppDbContext dbContext) : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new DashboardViewModel
        {
            Wishlists = dbContext.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => new WishlistSummary
                {
                    Id = w.Id,
                    Name = w.Name,
                    ItemCount = w.Items.Count
                })
                .OrderByDescending(w => w.Id)
                .ToList()
        };

        return View(model);
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
