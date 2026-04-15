using System.Threading.Channels;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class InMemoryItemQueueService : IItemQueueService
{
    private readonly Channel<AddItemQueueMessage> _channel = Channel.CreateUnbounded<AddItemQueueMessage>();

    public async Task EnqueueAsync(AddItemQueueMessage message, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public async Task<DequeuedAddItemMessage?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        var hasItem = await _channel.Reader.WaitToReadAsync(cancellationToken);
        if (!hasItem)
        {
            return null;
        }

        if (!_channel.Reader.TryRead(out var item))
        {
            return null;
        }

        return new DequeuedAddItemMessage
        {
            MessageId = Guid.NewGuid().ToString("N"),
            Payload = item
        };
    }

    public Task CompleteAsync(DequeuedAddItemMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
