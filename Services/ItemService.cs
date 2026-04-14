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
}
