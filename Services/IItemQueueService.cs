using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IItemQueueService
{
    Task EnqueueAsync(AddItemQueueMessage message, CancellationToken cancellationToken = default);
    Task<DequeuedAddItemMessage?> DequeueAsync(CancellationToken cancellationToken = default);
    Task CompleteAsync(DequeuedAddItemMessage message, CancellationToken cancellationToken = default);
}
