using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.BackgroundJobs;

public class AddItemQueueWebJob(
    IItemQueueService queueService,
    IServiceScopeFactory scopeFactory,
    ILogger<AddItemQueueWebJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DequeuedAddItemMessage? queuedMessage;
            try
            {
                queuedMessage = await queueService.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to dequeue add-item message.");
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                continue;
            }

            if (queuedMessage is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                continue;
            }

            try
            {
                using var scope = scopeFactory.CreateScope();
                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

                var result = await itemService.AddToWishlistAsync(
                    queuedMessage.Payload.UserId,
                    queuedMessage.Payload.WishlistId,
                    new CreateItemCommand
                    {
                        ProductName = queuedMessage.Payload.ProductName,
                        Link = queuedMessage.Payload.Link,
                        Merchant = queuedMessage.Payload.Merchant,
                        Type = queuedMessage.Payload.Type
                    },
                    stoppingToken);

                if (!result.Succeeded)
                {
                    logger.LogWarning(
                        "Add-item queue message {MessageId} rejected: {Reason}",
                        queuedMessage.MessageId,
                        result.ErrorMessage ?? "unknown");
                }

                await queueService.CompleteAsync(queuedMessage, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing add-item queue message {MessageId}.", queuedMessage.MessageId);
            }
        }
    }
}
