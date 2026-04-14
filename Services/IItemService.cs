using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IItemService
{
    Task<AddItemResult> AddToWishlistAsync(int userId, int wishlistId, CreateItemCommand command, CancellationToken cancellationToken = default);
}
