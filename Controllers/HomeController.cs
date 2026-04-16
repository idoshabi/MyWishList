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
            return RedirectToAction("Login", "Account");

        var firstName = User.FindFirstValue("FirstName") ?? "there";
        var wishlists = await wishlistService.GetDashboardWishlistsAsync(userId);

        var summaries = wishlists.Select(w => new WishlistSummary
        {
            Id = w.Id,
            Name = w.Name,
            RegistryType = w.RegistryType,
            Visibility = w.Visibility,
            ShareToken = w.ShareToken,
            ItemCount = w.ItemCount,
            Description = w.Description,
            EventDate = w.EventDate,
            CashFundGoal = w.CashFundGoal,
            CashFundRaised = w.CashFundRaised
        }).ToList();

        var model = new DashboardViewModel
        {
            FirstName = firstName,
            TotalLists = summaries.Count,
            TotalItems = summaries.Sum(s => s.ItemCount),
            Wishlists = summaries
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
