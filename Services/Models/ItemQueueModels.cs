namespace MyWishList.Web.Services.Models;

public sealed class AddItemQueueMessage
{
    public int UserId { get; init; }
    public int WishlistId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Link { get; init; }
    public string? Merchant { get; init; }
    public string? Type { get; init; }
    public DateTimeOffset QueuedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class DequeuedAddItemMessage
{
    public string MessageId { get; init; } = string.Empty;
    public string? PopReceipt { get; init; }
    public AddItemQueueMessage Payload { get; init; } = new();
}
