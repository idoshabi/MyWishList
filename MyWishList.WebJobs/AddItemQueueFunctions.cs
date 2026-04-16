using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.WebJobs;

public class AddItemQueueFunctions(
    IItemService itemService,
    ILogger<AddItemQueueFunctions> logger)
{
    [Singleton]
    public async Task ProcessAddItemAsync(
        [QueueTrigger("%StorageQueue:AddItemQueueName%", Connection = "StorageQueue:ConnectionString")] AddItemQueueMessage message,
        CancellationToken cancellationToken)
    {
        var result = await itemService.AddToWishlistAsync(
            message.UserId,
            message.WishlistId,
            new CreateItemCommand
            {
                ProductName = message.ProductName,
                Link = message.Link,
                Merchant = message.Merchant,
                Type = message.Type
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Add-item message rejected for wishlist {WishlistId}: {Reason}",
                message.WishlistId,
                result.ErrorMessage ?? "unknown");
            return;
        }

        logger.LogInformation(
            "Added queue item '{ProductName}' to wishlist {WishlistId} for user {UserId}.",
            message.ProductName,
            message.WishlistId,
            message.UserId);
    }
}
