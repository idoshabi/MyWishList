using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Models.ViewModels;
using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Controllers;

[Authorize]
public class WishlistsController(IWishlistService wishlistService, IItemQueueService itemQueueService) : Controller
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

        var result = await wishlistService.CreateAsync(userId.Value, new CreateWishlistCommand
        {
            Name = model.Name
        });
        if (!result.Succeeded)
        {
            SetFeedback("danger", result.ErrorMessage ?? "Couldn't create wishlist.");
            return RedirectToAction("Index", "Home");
        }

        SetFeedback("success", "Wishlist created.");
        return RedirectToAction(nameof(Details), new { id = result.WishlistId });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, id);
        if (wishlist is null)
        {
            return NotFound();
        }

        var model = new WishlistDetailsViewModel
        {
            WishlistId = wishlist.WishlistId,
            WishlistName = wishlist.WishlistName,
            Items = wishlist.Items
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

        if (!ModelState.IsValid)
        {
            SetFeedback("danger", "Couldn't add item. Please check the form values.");
            return RedirectToAction(nameof(Details), new { id });
        }

        var ownsWishlist = await wishlistService.UserOwnsWishlistAsync(userId.Value, id);
        if (!ownsWishlist)
        {
            return NotFound();
        }

        await itemQueueService.EnqueueAsync(new AddItemQueueMessage
        {
            UserId = userId.Value,
            WishlistId = id,
            ProductName = model.ProductName,
            Link = model.Link,
            Merchant = model.Merchant,
            Type = model.Type
        });

        SetFeedback("success", "Item queued and will appear shortly.");
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
