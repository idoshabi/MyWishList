using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class WishlistService(AppDbContext dbContext) : IWishlistService
{
    public async Task<IReadOnlyList<WishlistSummaryDto>> GetDashboardWishlistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistSummaryDto
            {
                Id = w.Id,
                Name = w.Name,
                ItemCount = w.Items.Count
            })
            .OrderByDescending(w => w.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateWishlistResult> CreateAsync(int userId, string wishlistName, CancellationToken cancellationToken = default)
    {
        var normalizedName = wishlistName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return new CreateWishlistResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist name is required."
            };
        }

        var wishlist = new Wishlist
        {
            Name = normalizedName,
            UserId = userId
        };

        dbContext.Wishlists.Add(wishlist);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateWishlistResult
        {
            Succeeded = true,
            WishlistId = wishlist.Id
        };
    }

    public async Task<WishlistDetailsDto?> GetDetailsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default)
    {
        var wishlist = await dbContext.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);

        if (wishlist is null)
        {
            return null;
        }

        return new WishlistDetailsDto
        {
            WishlistId = wishlist.Id,
            WishlistName = wishlist.Name,
            Items = wishlist.Items
                .OrderByDescending(i => i.Id)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Link = i.Link,
                    Merchant = i.Merchant,
                    Type = i.Type
                }).ToList()
        };
    }

    public Task<bool> UserOwnsWishlistAsync(int userId, int wishlistId, CancellationToken cancellationToken = default)
    {
        return dbContext.Wishlists.AnyAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);
    }

    public async Task<WishlistPlatformStatsDto> GetPlatformStatsAsync(CancellationToken cancellationToken = default)
    {
        var userCount = await dbContext.Users.CountAsync(cancellationToken);
        var wishlistCount = await dbContext.Wishlists.CountAsync(cancellationToken);
        var itemCount = await dbContext.Items.CountAsync(cancellationToken);

        return new WishlistPlatformStatsDto
        {
            UserCount = userCount,
            WishlistCount = wishlistCount,
            ItemCount = itemCount
        };
    }
}
