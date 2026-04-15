using System.Text.Json;
using Azure.Storage.Queues;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class AzureStorageItemQueueService : IItemQueueService
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<AzureStorageItemQueueService> _logger;

    public AzureStorageItemQueueService(string connectionString, string queueName, ILogger<AzureStorageItemQueueService> logger)
    {
        _logger = logger;
        _queueClient = new QueueClient(connectionString, queueName);
        _queueClient.CreateIfNotExists();
    }

    public async Task EnqueueAsync(AddItemQueueMessage message, CancellationToken cancellationToken = default)
    {
        var body = JsonSerializer.Serialize(message);
        await _queueClient.SendMessageAsync(body, cancellationToken: cancellationToken);
    }

    public async Task<DequeuedAddItemMessage?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        var response = await _queueClient.ReceiveMessagesAsync(
            maxMessages: 1,
            visibilityTimeout: TimeSpan.FromSeconds(30),
            cancellationToken: cancellationToken);

        var raw = response.Value.FirstOrDefault();
        if (raw is null)
        {
            return null;
        }

        try
        {
            var payload = JsonSerializer.Deserialize<AddItemQueueMessage>(raw.MessageText);
            if (payload is null)
            {
                await _queueClient.DeleteMessageAsync(raw.MessageId, raw.PopReceipt, cancellationToken);
                _logger.LogWarning("Deleted malformed add-item queue message {MessageId}.", raw.MessageId);
                return null;
            }

            return new DequeuedAddItemMessage
            {
                MessageId = raw.MessageId,
                PopReceipt = raw.PopReceipt,
                Payload = payload
            };
        }
        catch (JsonException ex)
        {
            await _queueClient.DeleteMessageAsync(raw.MessageId, raw.PopReceipt, cancellationToken);
            _logger.LogWarning(ex, "Deleted unreadable add-item queue message {MessageId}.", raw.MessageId);
            return null;
        }
    }

    public async Task CompleteAsync(DequeuedAddItemMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message.PopReceipt))
        {
            return;
        }

        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
    }
}
