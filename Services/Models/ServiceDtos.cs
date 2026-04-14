namespace MyWishList.Web.Services.Models;

public sealed class WishlistSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int ItemCount { get; init; }
}

public sealed class ItemDto
{
    public int Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Link { get; init; }
    public string? Merchant { get; init; }
    public string? Type { get; init; }
}

public sealed class WishlistDetailsDto
{
    public int WishlistId { get; init; }
    public string WishlistName { get; init; } = string.Empty;
    public IReadOnlyList<ItemDto> Items { get; init; } = [];
}

public sealed class WishlistPlatformStatsDto
{
    public int UserCount { get; init; }
    public int WishlistCount { get; init; }
    public int ItemCount { get; init; }
}
