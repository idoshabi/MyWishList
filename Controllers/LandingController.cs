using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Services;

namespace MyWishList.Web.Controllers;

public class LandingController(IWishlistService wishlistService) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        var stats = await wishlistService.GetPlatformStatsAsync();
        ViewBag.UserCount = stats.UserCount;
        ViewBag.WishlistCount = stats.WishlistCount;
        ViewBag.ItemCount = stats.ItemCount;
        return View();
    }

    public IActionResult Find()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> FindResults(string? query, string? type)
    {
        var results = await wishlistService.SearchPublicAsync(query, type);
        ViewBag.Query = query;
        ViewBag.Type = type;
        return View("FindResults", results);
    }

    public IActionResult HowItWorks() => View();
    public IActionResult Features() => View();
    public IActionResult About() => View();
    public IActionResult Contact() => View();
    public IActionResult Privacy() => View();

    public IActionResult Occasion(string type)
    {
        var validTypes = new[] { "Wedding", "Baby", "Birthday", "Housewarming", "Nonprofit" };
        if (string.IsNullOrWhiteSpace(type) || !validTypes.Contains(type))
            return RedirectToAction(nameof(Index));

        ViewBag.OccasionType = type;
        return View();
    }
}
