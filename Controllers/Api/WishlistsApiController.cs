using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Controllers.Api;

[Authorize]
[Route("api/wishlists")]
public class WishlistsApiController(IWishlistService wishlistService, IItemService itemService) : ApiControllerBase
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

        var result = await wishlistService.CreateAsync(userId.Value, request.Name, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not create wishlist." });
        }

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, result.WishlistId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.WishlistId }, wishlist is null ? null : ToDetailsResponse(wishlist));
    }

    [HttpPost("{id:int}/items")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status201Created)]
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

        var result = await itemService.AddToWishlistAsync(userId.Value, id, new CreateItemCommand
        {
            ProductName = request.ProductName,
            Link = request.Link,
            Merchant = request.Merchant,
            Type = request.Type
        }, cancellationToken);

        if (!result.Succeeded)
        {
            if (string.Equals(result.ErrorMessage, "Wishlist not found.", StringComparison.Ordinal))
            {
                return NotFound(new ApiErrorResponse { Message = result.ErrorMessage ?? "Wishlist not found." });
            }

            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Could not add item." });
        }

        var wishlist = await wishlistService.GetDetailsAsync(userId.Value, id, cancellationToken);
        var item = wishlist?.Items.FirstOrDefault(i => i.Id == result.ItemId);

        if (item is null)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        return StatusCode(StatusCodes.Status201Created, new ItemResponse
        {
            Id = item.Id,
            ProductName = item.ProductName,
            Link = item.Link,
            Merchant = item.Merchant,
            Type = item.Type
        });
    }

    private static WishlistDetailsResponse ToDetailsResponse(WishlistDetailsDto wishlist)
    {
        return new WishlistDetailsResponse
        {
            Id = wishlist.WishlistId,
            Name = wishlist.WishlistName,
            Items = wishlist.Items.Select(i => new ItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Link = i.Link,
                Merchant = i.Merchant,
                Type = i.Type
            }).ToList()
        };
    }
}
