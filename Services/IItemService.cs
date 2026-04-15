using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IItemService
{
    Task<AddItemResult> AddToWishlistAsync(int userId, int wishlistId, CreateItemCommand command, CancellationToken cancellationToken = default);
    Task<ReserveItemResult> ReserveAsync(int wishlistId, int itemId, string reservedByName, CancellationToken cancellationToken = default);
    Task<ReserveItemResult> UnreserveAsync(int wishlistId, int itemId, CancellationToken cancellationToken = default);
    Task<ItemStatusResult> MarkPurchasedAsync(int wishlistId, int itemId, string purchasedByName, CancellationToken cancellationToken = default);
    Task<ItemStatusResult> MarkReceivedAsync(int userId, int wishlistId, int itemId, CancellationToken cancellationToken = default);
}
