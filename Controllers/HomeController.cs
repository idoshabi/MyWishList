using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Models;
using MyWishList.Web.Models.ViewModels;
using MyWishList.Web.Services;

namespace MyWishList.Web.Controllers;

public class HomeController(IWishlistService wishlistService) : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlistSummaries = await wishlistService.GetDashboardWishlistsAsync(userId);
        var model = new DashboardViewModel
        {
            Wishlists = wishlistSummaries
                .Select(w => new WishlistSummary
                {
                    Id = w.Id,
                    Name = w.Name,
                    ItemCount = w.ItemCount
                })
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
