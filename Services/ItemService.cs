using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class ItemService(AppDbContext dbContext) : IItemService
{
    public async Task<AddItemResult> AddToWishlistAsync(int userId, int wishlistId, CreateItemCommand command, CancellationToken cancellationToken = default)
    {
        var wishlistExists = await dbContext.Wishlists.AnyAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);
        if (!wishlistExists)
        {
            return new AddItemResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist not found."
            };
        }

        var normalizedProductName = command.ProductName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedProductName))
        {
            return new AddItemResult
            {
                Succeeded = false,
                ErrorMessage = "Product name is required."
            };
        }

        var item = new Item
        {
            WishlistId = wishlistId,
            ProductName = normalizedProductName,
            Link = string.IsNullOrWhiteSpace(command.Link) ? null : command.Link.Trim(),
            Merchant = string.IsNullOrWhiteSpace(command.Merchant) ? null : command.Merchant.Trim(),
            Type = string.IsNullOrWhiteSpace(command.Type) ? null : command.Type.Trim()
        };

        dbContext.Items.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddItemResult
        {
            Succeeded = true,
            ItemId = item.Id
        };
    }

    public async Task<ReserveItemResult> ReserveAsync(int wishlistId, int itemId, string reservedByName, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId, cancellationToken);

        if (item is null)
        {
            return new ReserveItemResult
            {
                Succeeded = false,
                ErrorMessage = "Item not found."
            };
        }

        if (item.IsReserved)
        {
            return new ReserveItemResult
            {
                Succeeded = false,
                ErrorMessage = "Item is already reserved."
            };
        }

        if (item.IsPurchased)
        {
            return new ReserveItemResult
            {
                Succeeded = false,
                ErrorMessage = "Item is already purchased."
            };
        }

        var by = reservedByName.Trim();
        if (string.IsNullOrWhiteSpace(by))
        {
            return new ReserveItemResult
            {
                Succeeded = false,
                ErrorMessage = "Reserved by name is required."
            };
        }

        item.IsReserved = true;
        item.ReservedByName = by;
        item.ReservedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReserveItemResult { Succeeded = true };
    }

    public async Task<ReserveItemResult> UnreserveAsync(int wishlistId, int itemId, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId, cancellationToken);

        if (item is null)
        {
            return new ReserveItemResult
            {
                Succeeded = false,
                ErrorMessage = "Item not found."
            };
        }

        item.IsReserved = false;
        item.ReservedByName = null;
        item.ReservedAtUtc = null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReserveItemResult { Succeeded = true };
    }

    public async Task<ItemStatusResult> MarkPurchasedAsync(int wishlistId, int itemId, string purchasedByName, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.Items
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId, cancellationToken);

        if (item is null)
        {
            return new ItemStatusResult
            {
                Succeeded = false,
                ErrorMessage = "Item not found."
            };
        }

        var by = purchasedByName.Trim();
        if (string.IsNullOrWhiteSpace(by))
        {
            return new ItemStatusResult
            {
                Succeeded = false,
                ErrorMessage = "Purchased by name is required."
            };
        }

        if (item.IsPurchased)
        {
            return new ItemStatusResult
            {
                Succeeded = false,
                ErrorMessage = "Item already purchased."
            };
        }

        item.IsPurchased = true;
        item.PurchasedByName = by;
        item.PurchasedAtUtc = DateTimeOffset.UtcNow;
        item.IsReserved = true;
        item.ReservedByName ??= by;
        item.ReservedAtUtc ??= DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ItemStatusResult { Succeeded = true };
    }

    public async Task<ItemStatusResult> MarkReceivedAsync(int userId, int wishlistId, int itemId, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.Items
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId, cancellationToken);

        if (item is null || item.Wishlist?.UserId != userId)
        {
            return new ItemStatusResult
            {
                Succeeded = false,
                ErrorMessage = "Item not found."
            };
        }

        if (!item.IsPurchased)
        {
            return new ItemStatusResult
            {
                Succeeded = false,
                ErrorMessage = "Item must be purchased first."
            };
        }

        item.IsReceived = true;
        item.ReceivedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ItemStatusResult { Succeeded = true };
    }
}
