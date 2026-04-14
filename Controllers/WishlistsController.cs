using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Models.ViewModels;

namespace MyWishList.Web.Controllers;

[Authorize]
public class WishlistsController(AppDbContext dbContext) : Controller
{
    private const string FeedbackTypeKey = "FeedbackType";
    private const string FeedbackMessageKey = "FeedbackMessage";

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWishlistViewModel model)
    {
        if (!ModelState.IsValid)
        {
            SetFeedback("danger", "Couldn't create wishlist. Please enter a valid name.");
            return RedirectToAction("Index", "Home");
        }

        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlist = new Wishlist
        {
            Name = model.Name.Trim(),
            UserId = userId.Value
        };

        dbContext.Wishlists.Add(wishlist);
        await dbContext.SaveChangesAsync();

        SetFeedback("success", "Wishlist created.");
        return RedirectToAction(nameof(Details), new { id = wishlist.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlist = await dbContext.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId.Value);

        if (wishlist is null)
        {
            return NotFound();
        }

        var model = new WishlistDetailsViewModel
        {
            WishlistId = wishlist.Id,
            WishlistName = wishlist.Name,
            Items = wishlist.Items
                .OrderByDescending(i => i.Id)
                .Select(i => new ItemSummary
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Link = i.Link,
                    Merchant = i.Merchant,
                    Type = i.Type
                }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int id, CreateItemViewModel model)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlist = await dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId.Value);
        if (wishlist is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            SetFeedback("danger", "Couldn't add item. Please check the form values.");
            return RedirectToAction(nameof(Details), new { id });
        }

        var item = new Item
        {
            WishlistId = id,
            ProductName = model.ProductName.Trim(),
            Link = string.IsNullOrWhiteSpace(model.Link) ? null : model.Link.Trim(),
            Merchant = string.IsNullOrWhiteSpace(model.Merchant) ? null : model.Merchant.Trim(),
            Type = string.IsNullOrWhiteSpace(model.Type) ? null : model.Type.Trim()
        };

        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync();

        SetFeedback("success", "Item added.");
        return RedirectToAction(nameof(Details), new { id });
    }

    private void SetFeedback(string alertType, string message)
    {
        TempData[FeedbackTypeKey] = alertType;
        TempData[FeedbackMessageKey] = message;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
