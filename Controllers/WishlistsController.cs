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
    [HttpGet]
    public IActionResult Create(string? type)
    {
        return View(new CreateWishlistViewModel
        {
            RegistryType = type ?? "General"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWishlistViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = GetCurrentUserId();
        if (userId is null)
            return RedirectToAction("Login", "Account");

        var result = await wishlistService.CreateAsync(userId.Value, new CreateWishlistCommand
        {
            Name = model.Name,
            RegistryType = model.RegistryType,
            Visibility = model.Visibility,
            Description = model.Description,
            EventDate = model.EventDate,
            CashFundGoal = model.CashFundGoal
        });

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not create wishlist.");
            return View(model);
        }

        SetFeedback("success", "Wishlist created successfully!");
        return RedirectToAction(nameof(Details), new { id = result.WishlistId });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return RedirectToAction("Login", "Account");

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, id);
        if (wishlist is null)
            return NotFound();

        return View(MapToViewModel(wishlist, true));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int id, CreateItemViewModel model)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            SetFeedback("danger", "Could not add item. Please check the form.");
            return RedirectToAction(nameof(Details), new { id });
        }

        var ownsWishlist = await wishlistService.UserOwnsWishlistAsync(userId.Value, id);
        if (!ownsWishlist)
            return NotFound();

        await itemQueueService.EnqueueAsync(new AddItemQueueMessage
        {
            UserId = userId.Value,
            WishlistId = id,
            ProductName = model.ProductName,
            Link = model.Link,
            Merchant = model.Merchant,
            Type = model.Type
        });

        SetFeedback("success", "Item added to your list!");
        return RedirectToAction(nameof(Details), new { id });
    }

    [AllowAnonymous]
    [HttpGet("/lists/{shareToken}")]
    public async Task<IActionResult> Public(string shareToken)
    {
        var wishlist = await wishlistService.GetPublicDetailsByShareTokenAsync(shareToken);
        if (wishlist is null)
            return NotFound();

        return View(MapToViewModel(wishlist, false));
    }

    private static WishlistDetailsViewModel MapToViewModel(WishlistDetailsDto dto, bool isOwner)
    {
        return new WishlistDetailsViewModel
        {
            WishlistId = dto.WishlistId,
            WishlistName = dto.WishlistName,
            RegistryType = dto.RegistryType,
            Visibility = dto.Visibility,
            ShareToken = dto.ShareToken,
            Description = dto.Description,
            EventDate = dto.EventDate,
            CashFundGoal = dto.CashFundGoal,
            CashFundRaised = dto.CashFundRaised,
            IsOwner = isOwner,
            Items = dto.Items.Select(i => new ItemSummary
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Link = i.Link,
                Merchant = i.Merchant,
                Type = i.Type,
                IsReserved = i.IsReserved,
                ReservedByName = i.ReservedByName,
                IsPurchased = i.IsPurchased,
                PurchasedByName = i.PurchasedByName,
                IsReceived = i.IsReceived
            }).ToList()
        };
    }

    private void SetFeedback(string alertType, string message)
    {
        TempData["FeedbackType"] = alertType;
        TempData["FeedbackMessage"] = message;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
