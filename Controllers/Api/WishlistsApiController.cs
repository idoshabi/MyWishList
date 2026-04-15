using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Controllers.Api;

[Authorize]
[Route("api/wishlists")]
public class WishlistsApiController(IWishlistService wishlistService, IItemService itemService, IItemQueueService itemQueueService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<WishlistSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var wishlists = await wishlistService.GetDashboardWishlistsAsync(userId.Value, cancellationToken);
        return Ok(wishlists.Select(w => new WishlistSummaryResponse
        {
            Id = w.Id,
            Name = w.Name,
            RegistryType = w.RegistryType,
            Visibility = w.Visibility,
            CashFundGoal = w.CashFundGoal,
            CashFundRaised = w.CashFundRaised,
            Description = w.Description,
            EventDate = w.EventDate,
            ShareToken = w.ShareToken,
            ItemCount = w.ItemCount
        }));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<WishlistDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, id, cancellationToken);
        if (wishlist is null)
        {
            return NotFound();
        }

        return Ok(ToDetailsResponse(wishlist));
    }

    [HttpPost]
    [ProducesResponseType<WishlistDetailsResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateWishlistApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await wishlistService.CreateAsync(userId.Value, new CreateWishlistCommand
        {
            Name = request.Name,
            RegistryType = request.RegistryType,
            Visibility = request.Visibility,
            CashFundGoal = request.CashFundGoal,
            Description = request.Description,
            EventDate = request.EventDate
        }, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not create wishlist." });
        }

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, result.WishlistId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.WishlistId }, wishlist is null ? null : ToDetailsResponse(wishlist));
    }

    [HttpPut("{id:int}/settings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSettings(int id, [FromBody] UpdateWishlistSettingsApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await wishlistService.UpdateSettingsAsync(userId.Value, id, new UpdateWishlistSettingsCommand
        {
            Name = request.Name,
            RegistryType = request.RegistryType,
            Visibility = request.Visibility,
            CashFundGoal = request.CashFundGoal,
            Description = request.Description,
            EventDate = request.EventDate
        }, cancellationToken);

        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Wishlist not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Wishlist not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not update wishlist settings." });
        }

        return NoContent();
    }

    [HttpPost("{id:int}/items")]
    [ProducesResponseType<QueueItemResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddItem(int id, [FromBody] CreateItemApiRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var ownsWishlist = await wishlistService.UserOwnsWishlistAsync(userId.Value, id, cancellationToken);
        if (!ownsWishlist)
        {
            return NotFound(new ApiErrorResponse { Message = "Wishlist not found." });
        }

        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            return BadRequest(new ApiErrorResponse { Message = "Product name is required." });
        }

        await itemQueueService.EnqueueAsync(new AddItemQueueMessage
        {
            UserId = userId.Value,
            WishlistId = id,
            ProductName = request.ProductName,
            Link = request.Link,
            Merchant = request.Merchant,
            Type = request.Type
        }, cancellationToken);

        return Accepted(new QueueItemResponse());
    }

    [AllowAnonymous]
    [HttpGet("~/api/public/wishlists/{shareToken}")]
    [ProducesResponseType<WishlistDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByShareToken(string shareToken, CancellationToken cancellationToken)
    {
        var wishlist = await wishlistService.GetPublicDetailsByShareTokenAsync(shareToken, cancellationToken);
        if (wishlist is null)
        {
            return NotFound();
        }

        return Ok(ToDetailsResponse(wishlist));
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/items/{itemId:int}/reserve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReserveItem(int id, int itemId, [FromBody] ReserveItemRequest request, CancellationToken cancellationToken)
    {
        var result = await itemService.ReserveAsync(id, itemId, request.ReservedByName, cancellationToken);
        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Item not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Item not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not reserve item." });
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpDelete("{id:int}/items/{itemId:int}/reserve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnreserveItem(int id, int itemId, CancellationToken cancellationToken)
    {
        var result = await itemService.UnreserveAsync(id, itemId, cancellationToken);
        if (!result.Succeeded)
        {
            return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Item not found." });
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/items/{itemId:int}/purchase")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkPurchased(int id, int itemId, [FromBody] PurchaseItemRequest request, CancellationToken cancellationToken)
    {
        var result = await itemService.MarkPurchasedAsync(id, itemId, request.PurchasedByName, cancellationToken);
        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Item not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Item not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not mark item as purchased." });
        }

        return NoContent();
    }

    [HttpPost("{id:int}/items/{itemId:int}/received")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkReceived(int id, int itemId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await itemService.MarkReceivedAsync(userId.Value, id, itemId, cancellationToken);
        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Item not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Item not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not mark item as received." });
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("~/api/public/discover")]
    [ProducesResponseType<IReadOnlyList<WishlistSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Discover([FromQuery] string? query, [FromQuery] string? type, CancellationToken cancellationToken)
    {
        var wishlists = await wishlistService.SearchPublicAsync(query, type, cancellationToken);
        return Ok(wishlists.Select(w => new WishlistSummaryResponse
        {
            Id = w.Id,
            Name = w.Name,
            RegistryType = w.RegistryType,
            Visibility = w.Visibility,
            CashFundGoal = w.CashFundGoal,
            CashFundRaised = w.CashFundRaised,
            Description = w.Description,
            EventDate = w.EventDate,
            ShareToken = w.ShareToken,
            ItemCount = w.ItemCount
        }));
    }

    [AllowAnonymous]
    [HttpPost("{id:int}/contributions")]
    [ProducesResponseType<CashContributionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Contribute(int id, [FromBody] CashContributionRequest request, CancellationToken cancellationToken)
    {
        var result = await wishlistService.ContributeAsync(id, new ContributeCashCommand
        {
            Provider = request.Provider,
            Amount = request.Amount,
            ContributorName = request.ContributorName,
            ContributorEmail = request.ContributorEmail,
            Message = request.Message
        }, cancellationToken);

        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Wishlist not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Wishlist not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not create contribution." });
        }

        var ownerId = GetCurrentUserId();
        var contributions = ownerId is null
            ? []
            : await wishlistService.GetContributionsAsync(ownerId.Value, id, cancellationToken);
        var contribution = contributions.FirstOrDefault(c => c.Id == result.ContributionId);

        if (contribution is null)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        return StatusCode(StatusCodes.Status201Created, ToContributionResponse(contribution));
    }

    [HttpGet("{id:int}/contributions")]
    [ProducesResponseType<IReadOnlyList<CashContributionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContributions(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var contributions = await wishlistService.GetContributionsAsync(userId.Value, id, cancellationToken);
        return Ok(contributions.Select(ToContributionResponse));
    }

    private static WishlistDetailsResponse ToDetailsResponse(WishlistDetailsDto wishlist)
    {
        return new WishlistDetailsResponse
        {
            Id = wishlist.WishlistId,
            Name = wishlist.WishlistName,
            RegistryType = wishlist.RegistryType,
            Visibility = wishlist.Visibility,
            CashFundGoal = wishlist.CashFundGoal,
            CashFundRaised = wishlist.CashFundRaised,
            Description = wishlist.Description,
            EventDate = wishlist.EventDate,
            ShareToken = wishlist.ShareToken,
            Items = wishlist.Items.Select(i => new ItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Link = i.Link,
                Merchant = i.Merchant,
                Type = i.Type,
                IsReserved = i.IsReserved,
                ReservedByName = i.ReservedByName,
                ReservedAtUtc = i.ReservedAtUtc,
                IsPurchased = i.IsPurchased,
                PurchasedByName = i.PurchasedByName,
                PurchasedAtUtc = i.PurchasedAtUtc,
                IsReceived = i.IsReceived,
                ReceivedAtUtc = i.ReceivedAtUtc
            }).ToList()
        };
    }

    private static CashContributionResponse ToContributionResponse(CashContributionDto value)
    {
        return new CashContributionResponse
        {
            Id = value.Id,
            Provider = value.Provider,
            Amount = value.Amount,
            ContributorName = value.ContributorName,
            ContributorEmail = value.ContributorEmail,
            Message = value.Message,
            Status = value.Status,
            CreatedAtUtc = value.CreatedAtUtc
        };
    }
}
